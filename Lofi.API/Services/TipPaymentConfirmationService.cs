using System;
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
    public class TipPaymentConfirmationService : IHostedService
    {
        private const string DEFAULT_LOFI_WALLET_FILE = "testwallet";
        private const string DEFAULT_LOFI_WALLET_PASSWORD = "";
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TipPaymentConfirmationService> _logger;
        private readonly string _lofiWalletFile;
        private readonly string _lofiWalletPassword;
        

        public TipPaymentConfirmationService(ILogger<TipPaymentConfirmationService> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            this._logger = logger;
            this._serviceScopeFactory = serviceScopeFactory;
            this._lofiWalletFile = configuration.GetValue<string>("LOFI_WALLET_FILE", DEFAULT_LOFI_WALLET_FILE);
            this._lofiWalletPassword = configuration.GetValue<string>("LOFI_WALLET_PASSWORD", DEFAULT_LOFI_WALLET_PASSWORD);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tip Payment Confirmation service is running.");
            UpdateTipStatuses(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task UpdateTipStatuses(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var lofiContext = scope.ServiceProvider.GetRequiredService<LofiContext>();
            var moneroService = scope.ServiceProvider.GetRequiredService<MoneroService>();

            ulong currentBlockHeight = await lofiContext.Tips
                .Where(t => t.Payment != null && t.Payment.BlockHeight.HasValue)
                .Select(t => (ulong?)t.Payment!.BlockHeight!.Value)
                .OrderByDescending(h => h)
                .FirstOrDefaultAsync(cancellationToken)
                ?? 1;
            try
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    _logger.LogInformation($"Searching for incoming tip transfers from block height {currentBlockHeight}");
                    await moneroService.OpenWalletAsync(new OpenWalletRpcParameters(_lofiWalletFile, _lofiWalletPassword), cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    var confirmedTransfersResponse = await moneroService.GetTransfers(new GetTransfersRpcParameters
                    {
                        In = true,
                        FilterByHeight = true,
                        MinHeight = currentBlockHeight
                    }, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    var mempoolTransfersResponse = await moneroService.GetTransfers(new GetTransfersRpcParameters
                    {
                        Pool = true
                    }, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    var transfers = Enumerable.Empty<GetTransfersRpcResult.Transfer>()
                        .Concat(confirmedTransfersResponse.Result.InTransfers ?? Enumerable.Empty<GetTransfersRpcResult.Transfer>())
                        .Concat(mempoolTransfersResponse.Result.PoolTransfers ?? Enumerable.Empty<GetTransfersRpcResult.Transfer>())
                        .ToList();
                    
                    if (!transfers.Any())
                    {
                        _logger.LogInformation($"Found no incoming transfers to lofi wallet");
                        continue;
                    }
                    _logger.LogInformation($"Found {transfers.Count} incoming transfers to lofi wallet");

                    var paymentIdToTransfer = transfers
                        .GroupBy(t => Convert.ToUInt16(t.PaymentId, 16))
                        .ToDictionary(g => g.Key, g => g.OrderByDescending(t => t.Timestamp).First());
                    var paymentIds = paymentIdToTransfer.Select(g => g.Key).ToArray();
                    _logger.LogInformation($"Looking for unpaid tips with payment ids: {(string.Join(", ", paymentIds.Select(i => i.ToString())))}");
                    var tips = await lofiContext.Tips
                        .Where(t => t.Payment == null && t.PaymentId.HasValue && paymentIds.Contains(t.PaymentId.Value))
                        .Include(t => t.Track)
                            .ThenInclude(t => t.Artists)
                        .ToListAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!tips.Any())
                    {
                        continue;
                    } 

                    foreach (var tip in tips)
                    {
                        var transfer = paymentIdToTransfer[tip.PaymentId!.Value];
                        var now = DateTime.Now;
                        tip.Payment = new TipPayment
                        {
                            BlockHeight = transfer.Height,
                            Amount = transfer.Amount,
                            TransactionId = transfer.TransactionId,
                            Timestamp = transfer.Timestamp,
                            Tip = tip,
                            CreatedDate = now,
                            ModifiedDate = now
                        };
                        tip.Payouts = tip.Track!.Artists
                            .Select(artist => new TipPayout
                            {
                                Tip = tip,
                                Artist = artist,
                                Amount = (ulong)Math.Floor((decimal)transfer.Amount / (decimal)tip.Track.Artists.Count),
                                CreatedDate = now,
                                ModifiedDate = now
                            })
                            .ToList();
                        _logger.LogInformation($"Matched tip id {tip.Id} with payment id {tip.PaymentId} ({tip.Payment.Amount} units) to transfer with txid {tip.Payment.TransactionId} at height {transfer.Height}");
                    }

                    currentBlockHeight = transfers.Max(t => t.Height);
                    _logger.LogInformation($"Incoming tip transfers up to {currentBlockHeight} have been synched");

                    await lofiContext.SaveChangesAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tip Payment Confirmation service is shutting down");
            return Task.CompletedTask;
        }
    }
}