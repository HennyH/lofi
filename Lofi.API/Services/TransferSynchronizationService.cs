using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Models.MoneroRpc.Parameters;
using Lofi.API.Models.MoneroRpc.Results;
using Lofi.Database;
using Lofi.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Configuration = Lofi.API.Configuration;

namespace Lofi.API.Services
{
    public class TransferSynchronizationService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TransferSynchronizationService> _logger;
        private readonly IEnumerable<Configuration.Wallet> _wallets;

        public TransferSynchronizationService(
                ILogger<TransferSynchronizationService> logger,
                IServiceScopeFactory serviceScopeFactory,
                IConfiguration configuration)
        {
            this._logger = logger;
            this._serviceScopeFactory = serviceScopeFactory;
            this._wallets = configuration.GetSection("Wallets").Get<List<Configuration.Wallet>>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    await SynchronizeTransfersAsync(pause: TimeSpan.FromSeconds(1), now: DateTime.Now, stoppingToken);
                }
                catch (Exception error)
                {
                    _logger.LogError($"An unhandled exception occured when synchronizing transfers: {error}\n{error.StackTrace}");
                }
            }
        }

        private async Task SynchronizeTransfersAsync(TimeSpan? pause = null, DateTime? now = null, CancellationToken stoppingToken = default)
        {
            var delay = pause.GetValueOrDefault(TimeSpan.FromSeconds(1));

            foreach (var wallet in _wallets)
            {
                if (string.IsNullOrWhiteSpace(wallet.Filename))
                {
                    _logger.LogWarning($"A wallet was configured with no filename");
                    continue;
                }

                try
                {
                    _logger.LogInformation($"Synchronizing transfers from wallet {wallet.Filename}");
                    await SynchronizeTransfersAsync(walletFilename: wallet.Filename, walletPassword: wallet.Password ?? string.Empty, now, stoppingToken);
                }
                catch (Exception error)
                {
                    _logger.LogError($"An unhandled exception occured when synchronizing transfers from wallet {wallet.Filename}: {error}\n{error.StackTrace}");
                }

                await Task.Delay(delay);
            }
        }

        private async Task SynchronizeTransfersAsync(string walletFilename, string walletPassword, DateTime? now = null, CancellationToken stoppingToken = default)
        {
            now ??= DateTime.Now;

            using var scope = _serviceScopeFactory.CreateScope();
            var lofiContext = scope.ServiceProvider.GetRequiredService<LofiContext>();
            var moneroService = scope.ServiceProvider.GetRequiredService<MoneroService>();

            var getAddressResponse = await moneroService.PerformWalletRpc<GetAddressRpcParameters, GetAddressRpcResult>(
                MoneroWalletRpcMethod.GET_ADDRESS,
                parameters: new GetAddressRpcParameters(0),
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: stoppingToken
            );
            stoppingToken.ThrowIfCancellationRequested();
            if (getAddressResponse.Error != null)
            {
                _logger.LogError($"While synchronizing wallet {walletFilename} I was unable to work out the wallets address. My wallet RPC request failed with the error: {getAddressResponse.Error.Code} - {getAddressResponse.Error.Message}");
                _logger.LogError($"Aborting synchronization of wallet {walletFilename}");
                return;
            }
            var walletAddress = getAddressResponse.Result.Address!;

            var getTransfersResponse = await moneroService.PerformWalletRpc<GetTransfersRpcParameters, GetTransfersRpcResult>(
                MoneroWalletRpcMethod.GET_TRANSFERS,
                parameters: new GetTransfersRpcParameters
                {
                    In = true,
                    Out = true,
                    Pool = true
                },
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: stoppingToken
            );
            stoppingToken.ThrowIfCancellationRequested();
            if (getTransfersResponse.Error != null)
            {
                _logger.LogError($"While synchronizing wallet {walletFilename} I was unable to get a list of the transfers. My wallet RPC request failed with the error: {getTransfersResponse.Error.Code} - {getTransfersResponse.Error.Message}");
                _logger.LogError($"Aborting synchronization of wallet {walletFilename}");
                return;
            }

            var transfers = Enumerable.Empty<GetTransfersRpcResult.Transfer>()
                .Concat(getTransfersResponse.Result.InTransfers ?? Enumerable.Empty<GetTransfersRpcResult.Transfer>())
                .Concat(getTransfersResponse.Result.OutTransfers ?? Enumerable.Empty<GetTransfersRpcResult.Transfer>())
                .Concat(getTransfersResponse.Result.PoolTransfers ?? Enumerable.Empty<GetTransfersRpcResult.Transfer>())
                .ToList();

            var transactionFragmentToTransfer = transfers
                .SelectMany(t => t.Destinations == null
                    ? new[] { (t.TransactionId, FromAddress: (string?)null, ToAddress: t.Address, Transfer: t) }
                    : t.Destinations.Select(d => (t.TransactionId, FromAddress: (string?)walletAddress, ToAddress: d.Address, Transfer: t)))
                .ToDictionary(x => (x.TransactionId, x.FromAddress, x.ToAddress), x => x.Transfer);

            // TODO(HH): consider subaddress here... + optimize query
            var transferEntities = await lofiContext.Transfers.ToListAsync(stoppingToken);
            stoppingToken.ThrowIfCancellationRequested();
            var transactionFragmentToTransferEntity = transferEntities
                .ToDictionary(e => (e.TransactionId, e.FromWalletAddress, e.ToWalletAddress), e => e);

            foreach (var ((transactionId, fromAddress, toAddress), transfer) in transactionFragmentToTransfer)
            {
                if (!transactionFragmentToTransferEntity.TryGetValue((transactionId, fromAddress, toAddress), out var entity))
                {
                    entity = new Transfer()
                    {
                        TransactionId = transactionId,
                        FromWalletAddress = fromAddress,
                        ToWalletAddress = toAddress,
                        CreatedDate = now,
                        ModifiedDate = now,
                        Amount = transfer.Amount,
                        PaymentId = Convert.ToUInt16(transfer.PaymentId, fromBase: 16),
                        Timestamp = transfer.Timestamp,
                        TransactionFee = transfer.Fee
                    };

                    lofiContext.Transfers.Add(entity);
                }

                if (transfer.Height > 0 && (entity.BlockHeight == null || entity.BlockHeight == 0))
                {
                    entity.BlockHeight = transfer.Height;
                    entity.ModifiedDate = now;
                }
            }

            await lofiContext.SaveChangesAsync(stoppingToken);
            stoppingToken.ThrowIfCancellationRequested();

            _logger.LogInformation($"Successfully synchronized transfers in wallet {walletFilename} of address {walletAddress}");
        }
    }
}