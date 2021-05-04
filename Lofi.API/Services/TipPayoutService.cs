using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database.Entities;
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
    public class TipPayoutService : IHostedService
    {
        private const string DEFAULT_LOFI_WALLET_FILE = "testwallet";
        private const string DEFAULT_LOFI_WALLET_PASSWORD = "";
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TipPayoutService> _logger;
        private readonly string _lofiWalletFile;
        private readonly string _lofiWalletPassword;

        public TipPayoutService(ILogger<TipPayoutService> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
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

        private static async Task<IEnumerable<TipPayout>> GetPendingPayoutsInOrderForAvailableFunds(LofiContext lofiContext, ulong availableFunds, CancellationToken cancellationToken = default)
        {
            var orderedPendingPayoutsQuery =  lofiContext.TipPayouts
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
                .Include(payout => payout.Tip)
                .ToListAsync(cancellationToken);
        }

        private static IReadOnlyList<(string Address, ulong TotalAmount, List<TipPayout> Payouts)> GroupPayoutsByDestination(IEnumerable<TipPayout> payouts)
        {
            return payouts
                .GroupBy(payout => payout.Artist!.WalletAddress!)
                .Select(g =>
                {
                    var artistsPayouts = g.ToList();
                    var totalPayout = (ulong)artistsPayouts.Sum(p => p.Amount ?? 0m);
                    return (g.Key, totalPayout, artistsPayouts);
                })
                .ToList();
        }

        public async Task PayoutTips(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var lofiContext = scope.ServiceProvider.GetRequiredService<LofiContext>();
            var moneroService = scope.ServiceProvider.GetRequiredService<MoneroService>();

            while (true)
            {
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

                    var pendingPayouts = await GetPendingPayoutsInOrderForAvailableFunds(lofiContext, unlockedBalance, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!pendingPayouts.Any()) continue;

                    var payoutsByDestination = GroupPayoutsByDestination(pendingPayouts);

                    foreach (var (address, amount, payouts) in payoutsByDestination)
                    {
                        var destinationArtistNames = string.Join(", ", payouts.Select(p => p.Artist.Name).Distinct());
                        var destinationTipIds = string.Join(", ", payouts.Select(p => p.Tip.Id.ToString()));
                        _logger.LogInformation($"Making payout of {amount} to wallet {address} for tip ids {destinationTipIds} on behalf of artists {destinationArtistNames}");
                    }

                    var estimatedSplitTransfer = await moneroService.SplitTransfer(new SplitTransferRpcParameters(
                        destinations: payoutsByDestination.Select(payouts => new TransferRpcParameters.TransferDestination(
                            amount: payouts.TotalAmount,
                            address: payouts.Address
                        )),
                        doNotRelay: true
                    ));
                    cancellationToken.ThrowIfCancellationRequested();
                    var sharedTransactionFee = (ulong)estimatedSplitTransfer.Result.Fees.Sum(fee => (decimal)fee);
                    var perDestinationFeeShare = (decimal)sharedTransactionFee / (decimal)payoutsByDestination.Count;

                    var feeAdjustedPayoutsByDestination = payoutsByDestination
                        .Select(payouts =>
                        (
                            Address: payouts.Address,
                            FeeAdjustedAmount: (ulong)(payouts.TotalAmount - perDestinationFeeShare),
                            TransactionFeeShare: sharedTransactionFee,
                            PerPayoutTransactionFeeShare: sharedTransactionFee / (ulong)payouts.Payouts.Count,
                            Payouts: payouts.Payouts
                        ))
                        .ToList();
                    
                    var splitTransfer = await moneroService.SplitTransfer(new SplitTransferRpcParameters(
                        destinations: feeAdjustedPayoutsByDestination.Select(payouts => new TransferRpcParameters.TransferDestination(
                            amount: payouts.FeeAdjustedAmount,
                            address: payouts.Address
                        )),
                        getTransactionHex: true,
                        getTransactionMetadata: true,
                        getTransactionKey: true
                    ));

                    /* past this area must be atomic */

                    var destinationAddressToTransfer = new Dictionary<string, GetTransferByTransactionIdRpcResult.GetTransferTransfer>();
                    foreach (var transactionId in splitTransfer.Result.TransactionHashes)
                    {
                        var transfer = await moneroService.GetTransferByTransactionId(new GetTransferByTransactionIdRpcParameters(
                            transactionId: transactionId
                        ));
                        foreach (var destination in transfer.Result.Transfer.Destinations)
                        {
                            destinationAddressToTransfer[destination.Address] = transfer.Result.Transfer;
                        }
                    }

                    var now = DateTime.Now;
                    foreach (var (address, feeAdjustedAmount, transactionFeeShare, perPayoutTransactionFeeShare, payouts) in feeAdjustedPayoutsByDestination)
                    {
                        if (!destinationAddressToTransfer.TryGetValue(address, out var transfer))
                        {
                            continue;
                        }

                        foreach (var payout in payouts)
                        {
                            payout.Receipt = new TipPayoutReceipt
                            {
                                TipPayout = payout,
                                NetPayoutAmount = payout.Amount! - perPayoutTransactionFeeShare,
                                PayoutTxFee = transactionFeeShare,
                                PayoutTxFeeShare = perPayoutTransactionFeeShare,
                                TransactionId = transfer.PaymentId,
                                /* TODO(HH): the block height here won't be confirmed - perhaps genericise the payment conf service */
                                /* a block height of 0 means that the payment hasn't been included in a block... yet! */
                                BlockHeight = transfer.Height == 0
                                    ? default
                                    : transfer.Height,
                                WalletAddress = address,
                                PayoutTimestamp = transfer.Timestamp,
                                CreatedDate = now,
                                ModifiedDate = now
                            };
                        }
                    }

                    await lofiContext.SaveChangesAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "\n" + ex.StackTrace);
                }
                finally
                {    
                    await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
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