using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database;
using Lofi.API.Models.MoneroRpc.Parameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lofi.API.Services
{
    public class TipPaymentConfirmationService : IHostedService, IDisposable
    {
        private const string DEFAULT_LOFI_WALLET_FILE = "testwallet";
        private const string DEFAULT_LOFI_WALLET_PASSWORD = "";
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Timer? _timer;
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
                .Where(t => t.BlockHeight.HasValue)
                .Select(t => (ulong?)t.BlockHeight!.Value)
                .OrderByDescending(h => h)
                .FirstOrDefaultAsync(cancellationToken)
                ?? 1;
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                _logger.LogInformation($"Searching for incoming tip transfers from block height {currentBlockHeight}");
                await moneroService.OpenWalletAsync(new OpenWalletRpcParameters(_lofiWalletFile, _lofiWalletPassword), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                var transfers = await moneroService.GetTransfers(new GetTransfersRpcParameters
                {
                    In = true,
                    FilterByHeight = true,
                    MinHeight = currentBlockHeight
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogInformation($"Found {transfers.Result.InTransfers.Count()} incoming transfers to lofi wallet");

                // TODO(HH): payment id's get reused past ushort.MaxValue so we need logic
                // here which correctly assocaites a transfer to it's tip despite the
                // payment ids not being unique.
                var paymentIdToTransfer = transfers.Result.InTransfers
                    .GroupBy(t => Convert.ToUInt16(t.PaymentId, 16))
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(t => t.Timestamp).First());
                var paymentIds = paymentIdToTransfer.Select(g => g.Key).ToArray();
                _logger.LogInformation($"Looking for tips with payment ids: {(string.Join(", ", paymentIds.Select(i => i.ToString())))}");
                var tips = await lofiContext.Tips
                    .Where(t => t.PaymentId.HasValue && paymentIds.Contains(t.PaymentId.Value))
                    .ToListAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (!tips.Any())
                {
                    continue;
                }

                foreach (var tip in tips)
                {
                    var transfer = paymentIdToTransfer[tip.PaymentId!.Value];
                    tip.BlockHeight = transfer.Height;
                    tip.TransactionId = transfer.TransactionId;
                    _logger.LogInformation($"Matched tip id {tip.Id} with payment id {tip.PaymentId} to transfer with txid {tip.TransactionId} at height {transfer.Height}");
                }

                currentBlockHeight = transfers.Result.InTransfers.Max(t => t.Height);
                _logger.LogInformation($"Incoming tip transfers up to {currentBlockHeight} have been synched");

                await lofiContext.SaveChangesAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tip Payment Confirmation service is shutting down");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}