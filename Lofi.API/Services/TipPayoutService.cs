using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database;
using Lofi.API.Database.Entities;
using Lofi.API.Models.MoneroRpc.Parameters;
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
                .OrderBy(tip => tip.Id);
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

        private static async Task<IEnumerable<(Tip Tip, ulong Amount, string Address)>> GetArtistPayoutsForTips(LofiContext lofiContext, IEnumerable<Tip> tips, CancellationToken cancellationToken = default)
        {
            var tipIdToTip = tips.ToDictionary(tip => tip.Id);
            var payedTipIds = tips
                .Where(tip =>
                    tip.Payment != null &&
                    tip.Payment.Amount.HasValue &&
                    tip.Payment.Amount > 0)
                .Select(tip => tip.Id);
            var artistTipPayouts = await lofiContext.Artists
                .SelectMany(artist => artist.Tips
                    .Select(tip => new
                    {
                        Artist = artist,
                        Tip = tip,
                        Payout = tip.Payouts
                            .Where(payout => payout.ArtistId == artist.Id)
                            .FirstOrDefault()
                    }))
                .Where(artistTipPayout => payedTipIds.Contains(artistTipPayout.Tip.Id))
                .ToListAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var tipIdToArtistTipPayouts = artistTipPayouts
                .GroupBy(artistTipPayout => artistTipPayout.Tip.Id)
                .ToDictionary(g => g.Key, g => g.ToList());

            var payouts = new List<(Tip Tip, ulong Amount, string Address)>();
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
                    .Sum(artistPayout => (decimal?)artistPayout!.Payout!.GrossPayoutAmount ?? 0m);
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

                    payouts.Add((thisTip, remainingPerArtistShare, artist.WalletAddress));
                }
            }
        }

        private static async Task<ulong> EstimateTransactionFeeToSendPayouts(MoneroService moneroService, IEnumerable<(Tip Tip, ulong Amount, string Address)> payouts, CancellationToken cancellationToken = default)
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

        private static IEnumerable<(Tip Tip, ulong GrossAmount, ulong TransactionFeeShare, ulong NetPayout, string Address)> AdjustPayoutsForSharedTransactionFee(
                IEnumerable<(Tip Tip, ulong Amount, string Address)> payouts,
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
            var feeAdjustedPayouts = new List<(Tip Tip, ulong GrossAmount, ulong TransactionFeeShare, ulong NetAmount, string Address)>();
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

                feeAdjustedPayouts.Add((payout.Tip, payout.Amount, perPayoutShareOfSharedTransactionFee, netPayoutAmount, payout.Address))
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

                var payoutsForTips = await GetArtistPayoutsForTips(lofiContext, tipsWithMissingPayouts);



                var destinationSummary = string.Join(", ", destinations.Select(d => $"({d.Address}, {d.Amount})"));
                _logger.LogInformation($"Lofi attempting to pay out tips with ids with the following destinations {destinationSummary}");
                var nonFeeAdjustedTransfer = await moneroService.Transfer(new TransferRpcParameters(
                    destinations: destinations,
                    doNotRelay: true,
                    getTransactionHex: true
                ));
                var fee = nonFeeAdjustedTransfer.Result.Fee;
                _logger.LogInformation($"Payout transaction fee is {fee}");
                if (unlockedBalance - fee <= 0)
                {
                    _logger.LogInformation($"After adjusting for a fee of {fee}, the unlocked balance was <= 0 so not payouts can occur");
                    continue;
                }

                var feePerDestination = (decimal)Math.Ceiling((decimal)fee / (decimal)destinations.Count);
                _logger.LogInformation($"Deducting ${feePerDestination} from each of the {destinations.Count} payout destinations to cover the transaction fee of {fee}");
                var feeAdjustedDestinations = destinations.Select(d => new TransferRpcParameters.TransferDestination((ulong)(d.Amount - feePerDestination), d.Address));
                var feeAdjustedTransfer = await moneroService.Transfer(new TransferRpcParameters(
                    destinations: feeAdjustedDestinations,
                    getTransactionMetadata: true,
                    getTransactionKey: true,
                    getTransactionHex: true
                ));
                _logger.LogInformation($"Payout mades from tips {commaSeperatedTipIds} to destinations {destinationSummary}");

                var tipIds = tipIdToPayouts.Select(g => g.Key).ToArray();
                var tipIdToTip = await lofiContext.Tips
                    .Where(t => tipIds.Contains(t.Id))
                    .ToDictionaryAsync(t => t.Id, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                fo
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tip Payout service shutting down");
            return Task.CompletedTask;
        }
    }
}