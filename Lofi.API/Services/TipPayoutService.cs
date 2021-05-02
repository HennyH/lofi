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
    public class TipPayoutService : IHostedService
    {
        private const string DEFAULT_LOFI_WALLET_FILE = "testwallet";
        private const string DEFAULT_LOFI_WALLET_PASSWORD = "";
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TipPaymentConfirmationService> _logger;
        private readonly string _lofiWalletFile;
        private readonly string _lofiWalletPassword;

        public TipPayoutService(ILogger<TipPaymentConfirmationService> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            this._logger = logger;
            this._serviceScopeFactory = serviceScopeFactory;
            this._lofiWalletFile = configuration.GetValue<string>("LOFI_WALLET_FILE", DEFAULT_LOFI_WALLET_FILE);
            this._lofiWalletPassword = configuration.GetValue<string>("LOFI_WALLET_PASSWORD", DEFAULT_LOFI_WALLET_PASSWORD);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tip Payout service is running.");
            PayoutTips(cancellationToken);
            return Task.CompletedTask;
        }

        private static async Task<IEnumerable<Tip>> GetTipsWithMissingPayoutsInPayoutOrderForAvailableFunds(LofiContext lofiContext, ulong availableFunds, CancellationToken cancellationToken = default)
        {
            var tipsWithMissingPayoutsInPayoutOrder = lofiContext.Tips
                /* we only pay out tips which have been payed by the tipper! */
                .Where(tip =>
                    tip.Payment != null &&
                    tip.Payment.BlockHeight.HasValue &&
                    tip.Payment.Amount.HasValue &&
                    tip.Payment.Amount > 0)
                /* only look at tips where at least one artist hasn't recieved their
                 * payout for whatever reason. Typically this will be because an
                 * artist hasn't registerd their wallet address.
                 */
                .Where(tip => tip.Artists
                    .Any(artist => !tip.Payouts
                        .Any(p => p.ArtistId == artist.Id)))
                /* pay out the tips starting from the oldest */
                .OrderBy(tip => tip.Id)
                .AsQueryable();
            var x = lofiContext.Tips
                /* we only pay out tips which have been payed by the tipper! */
                .Where(tip =>
                    tip.Payment != null &&
                    tip.Payment.BlockHeight.HasValue &&
                    tip.Payment.Amount.HasValue &&
                    tip.Payment.Amount > 0)
                .Include(tip => tip.Payment)
                .Include(tip => tip.Artists)
                .Include(tip => tip.Payouts)
                .ToList(); 
            return await tipsWithMissingPayoutsInPayoutOrder
                .Select(tip => new
                {
                    Tip = tip,
                    RunningPayoutTotal = (ulong)tipsWithMissingPayoutsInPayoutOrder
                        .Where(otherTip => otherTip.Id <= tip.Id)
                        .Select(otherTip => new
                        {
                            TipAmount = otherTip.Payment!.Amount!.Value,
                            /* we use the gross payout amount here b/c the original $tip amount / # artist$ did not account
                             * for the transaction fee, and the transaction fees came from the pool of payout's
                             * tips, meaning part of the tx fee came from the tip.
                             */
                            PayedOutAmount = otherTip.Payouts.Sum(p => (decimal)p.GrossPayoutAmount!.Value)
                        })
                        .Select(pay => pay.TipAmount - pay.PayedOutAmount)
                        /* for whatever reason if we've payed out more than the tipper tipped then
                         * abandon paying out any other artists associated with the tip.
                         */
                        .Where(remainingPayoutAmount => remainingPayoutAmount > 0)
                        .Sum(remainingPayoutAmount => remainingPayoutAmount)
                })
                .Where(runningPayoutTip => runningPayoutTip.RunningPayoutTotal <= availableFunds)
                .Select(runningPayoutTip => runningPayoutTip.Tip)
                .ToListAsync(cancellationToken);
        }

        private static async Task<IEnumerable<(Tip Tip, Artist Artist, ulong Amount, string Address)>> GetArtistPayoutsForTips(LofiContext lofiContext, IEnumerable<Tip> tips, CancellationToken cancellationToken = default)
        {
            var tipIdToTip = tips.ToDictionary(tip => tip.Id);
            var payedTipIds = tips
                .Where(tip =>
                    tip.Payment != null &&
                    tip.Payment.Amount.HasValue &&
                    tip.Payment.Amount > 0)
                .Select(tip => tip.Id)
                .ToArray();
            var tipsToPayOut = await lofiContext.Tips
                .FromSqlInterpolated(
                    $@"
                        SELECT
                            tip.*
                        FROM tips AS tip
                        INNER JOIN artist_tip AS artist_tip
                            ON artist_tip.tips_id = tip.id
                        LEFT OUTER JOIN tip_payouts AS payout
                            ON payout.tip_id = tip.id AND payout.artist_id = artist_tip.artists_id
                        WHERE
                            EXISTS
                            (
                                SELECT
                                    *
                                FROM tip_payments AS payment
                                WHERE payment.tip_id = tip.id
                            )
                            AND tip.id = ANY ({payedTipIds})
                    "
                )
                .Include(x => x.Artists)
                .Include(x => x.Payouts) 
                .ToListAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var artistTipPayouts = tipsToPayOut
                .SelectMany(tip => tip.Artists
                    .Select(artist => new
                    {
                        Tip = tip,
                        Artist = artist,
                        Payout = tip.Payouts.Where(payout => payout.ArtistId == artist.Id).FirstOrDefault()
                    }))
                .ToList();
            var tipIdToArtistTipPayouts = artistTipPayouts
                .GroupBy(artistTipPayout => artistTipPayout.Tip.Id) 
                .ToDictionary(g => g.Key, g => g.ToList());

            var payouts = new List<(Tip Tip, Artist artist, ulong Amount, string Address)>();
            foreach (var tipId in payedTipIds)
            {
                if (!tipIdToTip.TryGetValue(tipId, out var thisTip))
                {
                    continue;
                }

                if (!tipIdToArtistTipPayouts.TryGetValue(tipId, out var thisTipsArtistPayouts))
                {
                    continue;
                }

                var tipAmount = (decimal)thisTip.Payment!.Amount!.Value;
                var alreadyPayedOutAmount = thisTipsArtistPayouts
                    .Sum(artistPayout => (decimal?)artistPayout.Payout?.GrossPayoutAmount ?? 0m);
                var remainingTipAmount = tipAmount - alreadyPayedOutAmount;
                var artistsToPayout = thisTipsArtistPayouts
                    .Where(artistPayout => artistPayout.Payout == null)
                    .Select(artistPayout => artistPayout.Artist)
                    .ToList();
                var numberOfArtistsLeftToPayout = artistsToPayout.Count;
                var remainingPerArtistShare = Convert.ToUInt64(Math.Floor((decimal)remainingTipAmount / (decimal)numberOfArtistsLeftToPayout));

                foreach (var artist in artistsToPayout)
                {
                    if (string.IsNullOrWhiteSpace(artist.WalletAddress))
                    {
                        continue;
                    }

                    payouts.Add((thisTip, artist, remainingPerArtistShare, artist.WalletAddress));
                }
            }

            return payouts;
        }

        private static async Task<ulong> EstimateTransactionFeeToSendPayouts(MoneroService moneroService, IEnumerable<(Tip Tip, Artist Artist, ulong Amount, string Address)> payouts, CancellationToken cancellationToken = default)
        {
            var destinations = payouts
                .GroupBy(payout => payout.Address)
                .Select(payoutsByAddress => new TransferRpcParameters.TransferDestination(
                    address: payoutsByAddress.Key,
                    amount: Convert.ToUInt64(payoutsByAddress.Sum(payout => (decimal)payout.Amount))))
                .ToList();
            var transfer = await moneroService.SplitTransfer(new SplitTransferRpcParameters(
                destinations: destinations,
                doNotRelay: true
            ));
            return (ulong)transfer.Result.Fees.Sum(fee => (decimal)fee);
        }

        private static IEnumerable<(Tip Tip, Artist Artist, ulong GrossAmount, ulong TransactionFeeShare, ulong NetPayout, string Address)> AdjustPayoutsForSharedTransactionFee(
                IEnumerable<(Tip Tip, Artist Artist, ulong Amount, string Address)> payouts,
                ulong sharedTransactionFee
            )
        {
            if (sharedTransactionFee <= 0) throw new ArgumentOutOfRangeException(nameof(sharedTransactionFee), "transaction fee must be greater than zero");

            var numberOfUniquePayoutAddresses = (decimal)payouts
                .Select(payout => payout.Address)
                .Distinct()
                .Count();
            var perAddressShareOfTransactionFee = Convert.ToUInt64(Math.Ceiling((decimal)sharedTransactionFee / (decimal)numberOfUniquePayoutAddresses));
            var addressToNumberOfPayouts = payouts
                .GroupBy(payout => payout.Address)
                .ToDictionary(g => g.Key, g => g.Count());
            var feeAdjustedPayouts = new List<(Tip Tip, Artist Artist, ulong GrossAmount, ulong TransactionFeeShare, ulong NetAmount, string Address)>();
            foreach (var payout in payouts)
            {
                if (!addressToNumberOfPayouts.TryGetValue(payout.Address, out var numberOfPayouts))
                {
                    continue;
                }

                var perPayoutShareOfSharedTransactionFee = Convert.ToUInt64(Math.Ceiling((decimal)perAddressShareOfTransactionFee / (decimal)numberOfPayouts));
                var netPayoutAmount = payout.Amount - perPayoutShareOfSharedTransactionFee;
                if (netPayoutAmount < 0)
                {
                    continue;
                }

                feeAdjustedPayouts.Add((payout.Tip, payout.Artist, payout.Amount, perPayoutShareOfSharedTransactionFee, netPayoutAmount, payout.Address));
            }

            return feeAdjustedPayouts;
        }

        public async Task PayoutTips(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var lofiContext = scope.ServiceProvider.GetRequiredService<LofiContext>();
            var moneroService = scope.ServiceProvider.GetRequiredService<MoneroService>();

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await moneroService.OpenWalletAsync(new OpenWalletRpcParameters(_lofiWalletFile, _lofiWalletPassword), cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    var balance = await moneroService.GetBalance(new GetBalanceRpcParameters(0), cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    var unlockedBalance = balance.Result.UnlockedBalance;
                    _logger.LogInformation($"Lofi wallet has a balance of {balance.Result.Balance} units of which {unlockedBalance} units are spendable for tip payouts");
                    if (unlockedBalance <= 0)
                    { 
                        _logger.LogInformation($"No funds in lofi wallet to payout tips");
                        continue;
                    }

                    var tipsWithMissingPayouts = await GetTipsWithMissingPayoutsInPayoutOrderForAvailableFunds(lofiContext, unlockedBalance, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    var nonFeeAdjustedPayouts = await GetArtistPayoutsForTips(lofiContext, tipsWithMissingPayouts, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!nonFeeAdjustedPayouts.Any()) continue;

                    var sharedTransactionFee = await EstimateTransactionFeeToSendPayouts(moneroService, nonFeeAdjustedPayouts, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    var feeAdjustedPayouts = AdjustPayoutsForSharedTransactionFee(nonFeeAdjustedPayouts, sharedTransactionFee);
                    var destinations = feeAdjustedPayouts
                        .GroupBy(payout => payout.Address)
                        .Select(payoutsByAddress => new TransferRpcParameters.TransferDestination(
                            address: payoutsByAddress.Key,
                            amount: Convert.ToUInt64(payoutsByAddress.Sum(payout => (decimal)payout.NetPayout))))
                        .ToList();
                    var splitTransfer = await moneroService.SplitTransfer(new SplitTransferRpcParameters(
                        destinations: destinations,
                        getTransactionHex: true,
                        getTransactionMetadata: true,
                        getTransactionKey: true
                    ));

                    var tipIds = feeAdjustedPayouts.Select(p => p.Tip.Id).ToArray();
                    var tipIdToTip = await lofiContext.Tips
                        .Where(t => tipIds.Contains(t.Id))
                        .ToDictionaryAsync(t => t.Id, cancellationToken);
                    var addressToTransfer = new Dictionary<string, GetTransferByTransactionIdRpcResult.GetTransferTransfer>();
                    foreach (var txId in splitTransfer.Result.TransactionHashes)
                    {
                        var transfer = await moneroService.GetTransferByTransactionId(new GetTransferByTransactionIdRpcParameters(
                            transactionId: txId
                        ));
                        foreach (var destination in transfer.Result.Transfer.Destinations)
                        {
                            addressToTransfer[destination.Address] = transfer.Result.Transfer;
                        }
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    foreach (var payout in feeAdjustedPayouts)
                    {
                        if (!tipIdToTip.TryGetValue(payout.Tip.Id, out var tip))
                        {
                            continue;
                        }

                        if (!addressToTransfer.TryGetValue(payout.Address, out var transfer))
                        {
                            continue;
                        }

                        var now = DateTime.Now;
                        tip.Payouts.Add(new TipPayout
                        {
                            Tip = payout.Tip,
                            Artist = payout.Artist,
                            WalletAddress = payout.Address,
                            TransactionId = transfer.TransactionId,
                            Timestamp = transfer.Timestamp,
                            GrossPayoutAmount = payout.GrossAmount,
                            PayoutTxFee = sharedTransactionFee,
                            PayoutTxFeeShare = payout.TransactionFeeShare,
                            NetPayoutAmount = payout.NetPayout,
                            CreatedDate = now,
                            ModifiedDate = now
                        });
                    }

                    await lofiContext.SaveChangesAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tip Payout service shutting down");
            return Task.CompletedTask;
        }
    }
}