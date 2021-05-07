using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database.Entities;
using Lofi.API.Models.MoneroRpc.Parameters;
using Lofi.API.Models.MoneroRpc.Results;
using Lofi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lofi.API.Services
{
    public class TipPaymentConfirmationService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TipPaymentConfirmationService> _logger;
        private readonly IEnumerable<Configuration.Wallet> _wallets;
        

        public TipPaymentConfirmationService(
                ILogger<TipPaymentConfirmationService> logger,
                IServiceScopeFactory serviceScopeFactory,
                IConfiguration configuration)
        {
            this._logger = logger;
            this._serviceScopeFactory = serviceScopeFactory;
            this._wallets = configuration.GetSection("Wallets").Get<List<Configuration.Wallet>>();
        }

        private async Task ScanForTipPaymentsInWallet(string walletFilename, string walletPassword, DateTime? now = null, CancellationToken stoppingToken = default)
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
                _logger.LogError($"While scanning for tip payments to wallet {walletFilename} I was unable to work out the wallets address. My wallet RPC request failed with the error: {getAddressResponse.Error.Code} - {getAddressResponse.Error.Message}");
                _logger.LogError($"Aborting scanning for tip payments to wallet {walletFilename}");
                return;
            }
            var walletAddress = getAddressResponse.Result.Address!;

            var newTipPaymentTransfersToWallet = await lofiContext.Transfers
                .Where(transfer => transfer.ToWalletAddress == walletAddress)
                .Where(transfer => !lofiContext.TipPayments.Any(p => p.PaymentTransferId == transfer.Id))
                .ToListAsync(stoppingToken);
            stoppingToken.ThrowIfCancellationRequested();

            var paymentIdToTransfers = newTipPaymentTransfersToWallet
                .GroupBy(transfer => transfer.PaymentId.GetValueOrDefault())
                .ToDictionary(g => g.Key, g => g.ToList());
            var paymentIds = paymentIdToTransfers.Keys.ToArray();
            
            var tips = await lofiContext.Tips
                .Where(tip => tip.PaymentId != null && paymentIds.Contains(tip.PaymentId.Value))
                .Include(tip => tip.Track)
                    .ThenInclude(track => track!.Artists)
                .ToListAsync(stoppingToken);
            stoppingToken.ThrowIfCancellationRequested();

            foreach (var tip in tips)
            {
                if (tip.PaymentId == null)
                {
                    _logger.LogWarning($"Tip with id {tip.Id} did not have a payment id and hence payments cannot be matched up to it!");
                    continue;
                }

                if (!paymentIdToTransfers.TryGetValue(tip.PaymentId.Value, out var transfers))
                {
                    _logger.LogError($"We detected tip with id {tip.Id} as having recieved a payment, but when we went to pair them up we couldn't find it anymore... this is probably a coding error");
                    continue;
                }

                foreach (var transfer in transfers)
                {
                    var tipPayment = new TipPayment
                    {
                        Tip = tip,
                        PaymentTransfer = transfer,
                        CreatedDate = now,
                        ModifiedDate = now
                    };
                    var perArtistPayoutAmount = transfer.Amount / (ulong)tip.Track!.Artists!.Count;
                    tipPayment.Payouts = tip.Track!.Artists
                        .Select(artist => new TipPayout
                        {
                            TipPayment = tipPayment,
                            Artist = artist,
                            Amount = perArtistPayoutAmount,
                            CreatedDate = now,
                            ModifiedDate = now
                        })
                        .ToList();
                    tip.Payments.Add(tipPayment);
                    _logger.LogInformation($"Matched tip {tip.Id} with transfer {transfer.TransactionId} of amount {transfer.Amount}");
                    _logger.LogInformation($"Divided tip {tip.Id}'s transfer {transfer.TransactionId} of amount {transfer.Amount} between {tip.Track!.Artists!.Count} artists, resulting in a payout of {perArtistPayoutAmount} each.");
                }
            }

            await lofiContext.SaveChangesAsync();
        }

        private async Task ScanForTipPayments(TimeSpan? pause = null, DateTime? now = null, CancellationToken stoppingToken = default)
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
                    _logger.LogInformation($"Scanning for tip payments into wallet {wallet.Filename}");
                    await ScanForTipPaymentsInWallet(walletFilename: wallet.Filename, walletPassword: wallet.Password ?? string.Empty, now, stoppingToken);
                }
                catch (Exception error)
                {
                    _logger.LogError($"An unhandled exception occured when searching for tip payments into wallet {wallet.Filename}: {error}\n{error.StackTrace}");
                }

                await Task.Delay(delay);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    await ScanForTipPayments(pause: TimeSpan.FromSeconds(1), now: DateTime.Now, stoppingToken);
                }
                catch (Exception error)
                {
                    _logger.LogError($"An unhandled exception occured when searching for tip payments: {error}\n{error.StackTrace}");
                }
            }
        }
    }
}