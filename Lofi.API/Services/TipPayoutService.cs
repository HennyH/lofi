using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database.Entities;
using Lofi.API.Models.MoneroRpc;
using Lofi.API.Models.MoneroRpc.Parameters;
using Lofi.API.Models.MoneroRpc.Results;
using Lofi.Database;
using Lofi.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lofi.API.Services
{
    public class TipPayoutService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TipPayoutService> _logger;
        private readonly IEnumerable<Configuration.Wallet> _wallets;

        public TipPayoutService(
                ILogger<TipPayoutService> logger,
                IServiceScopeFactory serviceScopeFactory,
                IConfiguration configuration)
        {
            this._logger = logger;
            this._serviceScopeFactory = serviceScopeFactory;
            this._wallets = configuration.GetSection("Wallets").Get<List<Configuration.Wallet>>();
        }

        private static async Task<IReadOnlyCollection<TipPayout>> GetPendingPayoutsInOrderForAvailableFundsAsync(LofiContext lofiContext, ulong availableFunds, CancellationToken cancellationToken = default)
        {
            var orderedPendingPayoutsQuery = lofiContext.TipPayouts
                .AsQueryable()
                /* payout records exist prior to being payed out (they initially record an intention
                 * to pay out part of a tip), after they do get payed out they will not be in a
                 * 'IsPending' state. Therefore we get the non-payed out payouts by filtering to
                 * the payouts which _are_ pending.
                 */
                .Where(payout => payout.Receipt == null)
                /* We can only pay out payouts if we know how much to pay out! This property is
                 * set to be [Required] in the database model.
                 */
                .Where(payout => payout.Amount.HasValue)
                /* payouts can only be paid out when the artist has a wallet address! */
                .Where(payout => payout.Artist!.WalletAddress != null)
                .OrderBy(payout => payout.Id);
            return await orderedPendingPayoutsQuery
                .Select(payout => new
                {
                    Payout = payout,
                    RunningGrossPayoutAmount = orderedPendingPayoutsQuery
                        .Where(otherPayout => otherPayout.Id <= payout.Id)
                        .Sum(otherPayout => otherPayout.Amount ?? 0m)
                })
                .Where(runningPayout => runningPayout.RunningGrossPayoutAmount <= availableFunds)
                .Select(runningPayout => runningPayout.Payout)
                .Include(payout => payout.Artist)
                .ToListAsync(cancellationToken);
        }

        private static IEnumerable<PayoutSet> GroupPayoutsIntoSetsByWalletAddress(IEnumerable<TipPayout> payouts)
        {
            var payoutsByWalletAddress = payouts.GroupBy(payout => payout.Artist!.WalletAddress!);
            foreach (var payoutGroup in payoutsByWalletAddress)
            {
                var walletAddress = payoutGroup.Key;
                var payoutsInGroup = payoutGroup.ToList();
                var totalPayoutAmount = (ulong)payoutGroup.Sum(p => p.Amount ?? 0m);
                yield return new PayoutSet(walletAddress, totalPayoutAmount, payoutsInGroup);
            }
        }

        private async Task<IReadOnlyCollection<GetTransferByTransactionIdRpcResult.GetTransferTransfer>> GetTransfersForTransactionIds(string walletFilename, string walletPassword, MoneroService moneroService, IEnumerable<string> transactionIds)
        {
            var transfers = new List<GetTransferByTransactionIdRpcResult.GetTransferTransfer>();
            foreach (var transactionId in transactionIds)
            {
                var response = await moneroService.GetTransferByTransactionId(new GetTransferByTransactionIdRpcParameters(
                    transactionId: transactionId
                ), walletFilename: walletFilename, walletPassword: walletPassword);

                if (response.Error != null)
                {
                    _logger.LogWarning($"Failed to retrieve transfer for transaction id {transactionId}");
                }
                else
                {
                    _logger.LogInformation($"Retrieved ransfer for transaction id {transactionId}");
                    transfers.Add(response.Result.Transfer);
                }
            }

            return transfers;
        }

        private async Task<IEnumerable<FeeAdjustedPayoutSet>> AdjustPayoutSetsToAccountForTransactionFeesAsync(string walletFilename, string walletPassword, MoneroService moneroService, IReadOnlyCollection<PayoutSet> payoutSets, CancellationToken cancellationToken)
        {
            var estimatedSplitTransfer = await moneroService.SplitTransfer(new SplitTransferRpcParameters(
                destinations: payoutSets.Select(payoutSet => new TransferRpcParameters.TransferDestination(
                    amount: payoutSet.Amount,
                    address: payoutSet.Address
                )),
                doNotRelay: true
            ), walletFilename: walletFilename, walletPassword: walletPassword, cancellationToken: cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var sharedTransactionFee = (ulong)estimatedSplitTransfer.Result.Fees.Sum(fee => (decimal)fee);
            var perDestinationFeeShare = (ulong)((decimal)sharedTransactionFee / (decimal)payoutSets.Count);
            _logger.LogInformation($"Estimated transaction fee for current payout sets is {sharedTransactionFee}");

            var feeAdjustedPayoutSets = new List<FeeAdjustedPayoutSet>();
            foreach (var payoutSet in payoutSets)
            {
                feeAdjustedPayoutSets.Add(new FeeAdjustedPayoutSet(
                    address: payoutSet.Address,
                    feeAdjustedAmount: (ulong)(payoutSet.Amount - perDestinationFeeShare),
                    transactionFeeShare: perDestinationFeeShare,
                    payouts: payoutSet.Payouts
                ));
            }

            return feeAdjustedPayoutSets;
        }

        private async Task SyncPayoutsToMatchingTransactionsAsync(string walletFilename, string walletPassword, LofiContext lofiContext, MoneroService moneroService, IEnumerable<FeeAdjustedPayoutSet> feeAdjustedPayoutSets, IEnumerable<string> transactionIds)
        {
            var transfers = await GetTransfersForTransactionIds(walletFilename, walletPassword, moneroService, transactionIds);
            var destinationAddressToTransfer = transfers
                .SelectMany(transfer => transfer.Destinations.Select(destination => (Address: destination.Address, Transfer: transfer)))
                .ToDictionary(x => x.Address, x => x.Transfer);

            var now = DateTime.Now;
            foreach (var payoutSet in feeAdjustedPayoutSets)
            {
                if (!destinationAddressToTransfer.TryGetValue(payoutSet.Address, out var transfer))
                {
                    continue;
                }

                var receipt = new TipPayoutReceipt
                {
                    TransactionId = transfer.TransactionId,
                    NetPayoutAmount = payoutSet.FeeAdjustedAmount,
                    PayoutTxFee = transfer.Fee,
                    PayoutTxFeeShare = payoutSet.TransactionFeeShare,
                    CreatedDate = now,
                    ModifiedDate = now
                };
                lofiContext.TipPayoutReceipts.Add(receipt);

                foreach (var payout in payoutSet.Payouts)
                {
                    payout.Receipt = receipt;
                }
            }
        }

        private async Task PayoutTipsUsingWallet(string walletFilename, string walletPassword, DateTime? now = null, CancellationToken stoppingToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var lofiContext = scope.ServiceProvider.GetRequiredService<LofiContext>();
            var moneroService = scope.ServiceProvider.GetRequiredService<MoneroService>();

            var balance = await moneroService.GetBalance(new GetBalanceRpcParameters(0), walletFilename: walletFilename, walletPassword: walletPassword, stoppingToken);
            stoppingToken.ThrowIfCancellationRequested();

            var unlockedBalance = balance.Result.UnlockedBalance;
            if (unlockedBalance <= 0)
            {
                _logger.LogInformation($"No funds in wallet {walletFilename} to payout tips with");
                return;
            }

            _logger.LogInformation($"Wallet {walletFilename} has a balance of {balance.Result.Balance} units of which {unlockedBalance} units are spendable for tip payouts");
            var payouts = await GetPendingPayoutsInOrderForAvailableFundsAsync(lofiContext, unlockedBalance, stoppingToken);
            _logger.LogInformation($"There are {payouts.Count} pending payouts which can be paid out using the unlocked balance of {unlockedBalance} units");
            stoppingToken.ThrowIfCancellationRequested();

            if (!payouts.Any()) return;

            var payoutSets = GroupPayoutsIntoSetsByWalletAddress(payouts).ToList();
            var feeAdjustedPayoutSets = await AdjustPayoutSetsToAccountForTransactionFeesAsync(walletFilename, walletPassword, moneroService, payoutSets, stoppingToken);

            // TODO(HH): We should probably actually do this as a doNotRelay: true, and then save the hex data to
            // the the recipet entity, then have a seperate service send out the tx... maybe? The idea is that if
            // after submitting the transfer we fail to record that fact we'll keep paying the same people out till
            // the piggy bank has run dry!
            var transferResponse = await moneroService.SplitTransfer(new SplitTransferRpcParameters(
                destinations: feeAdjustedPayoutSets.Select(payoutSet => new TransferRpcParameters.TransferDestination(
                    amount: payoutSet.FeeAdjustedAmount,
                    address: payoutSet.Address
                )),
                getTransactionHex: true,
                getTransactionMetadata: true,
                getTransactionKey: true
            ), walletFilename: walletFilename, walletPassword: walletPassword);
            if (transferResponse.Error != null)
            {
                _logger.LogError($"There was an error submitting the payout transfer: {transferResponse.Error.Code} - {transferResponse.Error.Message}");
                return;
            }
            var transactionIds = transferResponse.Result.TransactionHashes;

            await SyncPayoutsToMatchingTransactionsAsync(walletFilename, walletPassword, lofiContext, moneroService, feeAdjustedPayoutSets, transactionIds);
            await lofiContext.SaveChangesAsync();
            _logger.LogInformation($"Paid out {payouts.Count} tips using wallet {walletFilename}");
        }

        private async Task PayoutTips(TimeSpan? pause = null, DateTime? now = null, CancellationToken stoppingToken = default)
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
                    _logger.LogInformation($"Trying to pay out tips using wallet {wallet.Filename}");
                    await PayoutTipsUsingWallet(walletFilename: wallet.Filename, walletPassword: wallet.Password ?? string.Empty, now, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    throw;
                }
                catch (Exception error)
                {
                    _logger.LogError($"An unhandled exception occured when trying to pay out tips using wallet {wallet.Filename}: {error}\n{error.StackTrace}");
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
                    await PayoutTips(pause: TimeSpan.FromSeconds(1), now: DateTime.Now, stoppingToken);
                }
                catch (Exception error)
                {
                    _logger.LogError($"An unhandled exception occured when trying to payout tips: {error}\n{error.StackTrace}");
                }
            }
        }
    }

    public struct PayoutSet
    {
        public PayoutSet(string address, ulong amount, IReadOnlyCollection<TipPayout> payouts)
        {
            this.Address = address;
            this.Amount = amount;
            this.Payouts = payouts;
        }

        public readonly string Address;
        public readonly ulong Amount;
        public readonly IReadOnlyCollection<TipPayout> Payouts;
    }

    public struct FeeAdjustedPayoutSet
    {
        public FeeAdjustedPayoutSet(string address, ulong feeAdjustedAmount, ulong transactionFeeShare, IReadOnlyCollection<TipPayout> payouts)
        {
            this.Address = address;
            this.FeeAdjustedAmount = feeAdjustedAmount;
            this.TransactionFeeShare = transactionFeeShare;
            this.Payouts = payouts;
        }

        public readonly string Address;
        public readonly ulong FeeAdjustedAmount;
        public readonly ulong TransactionFeeShare;
        public readonly IReadOnlyCollection<TipPayout> Payouts;
    }
}