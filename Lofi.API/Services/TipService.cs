using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database;
using Lofi.API.Database.Entities;
using Lofi.API.Models.MoneroRpc;
using Lofi.API.Models.MoneroRpc.Parameters;
using Lofi.API.Models.MoneroRpc.Results;
using Lofi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lofi.API.Services
{
    public class TipService
    {
        private readonly ILogger<TipService> _logger;
        private readonly LofiContext _lofiContext;
        private readonly MoneroService _moneroService;
        private readonly IEnumerable<Configuration.Wallet> _wallets;

        public TipService(ILogger<TipService> logger, IConfiguration configuration, LofiContext lofiContext, MoneroService moneroSerivce)
        {
            this._logger = logger;
            this._lofiContext = lofiContext;
            this._moneroService = moneroSerivce;
            this._wallets = configuration.GetSection("Wallets").Get<List<Configuration.Wallet>>();
        }

        public async Task<ushort?> GetAvailablePaymentId(CancellationToken cancellationToken = default)
        {
            var highestPaymentIdInUse = await _lofiContext.Tips
                .Select(t => t.PaymentId)
                .OrderByDescending(id => id)
                .FirstOrDefaultAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (highestPaymentIdInUse == null) return 1;
            if (highestPaymentIdInUse.Value < UInt16.MaxValue) return (ushort)(highestPaymentIdInUse.Value + 1);

            throw new NotImplementedException("Support for recycling payment ids has not been implemented!");
        }

        public async Task<Tip> CreateTip(int trackId, string? message = null, DateTime? now = null, CancellationToken cancellationToken = default)
        {
            now ??= DateTime.Now;

            var track = await _lofiContext.Tracks.FindAsync(new object[] { trackId }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (track == null) throw new ArgumentException(nameof(trackId), $"No such track with Id = {trackId}");
            await _lofiContext.Entry(track)
                .Collection(t => t.Artists)
                .LoadAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var tip = new Tip
            {
                Message = message,
                Track = track,
                CreatedDate = DateTime.Now
            };

            var paymentId = await GetAvailablePaymentId();
            var integratedAddressResponse = await _moneroService.MakeIntegratedAddressAsync(
                new MakeIntegratedAddressRpcParameters(
                    standardAddress: "9ygTQBiUYfN2Qh91HVyGjNUVbRJ1VJFnxWzG9LNJezKRVu4DVQZAJUmRMjBXqfrkUqggrvSNA5APWA37ivsCQWgaN13nyv9",
                    paymentId: paymentId
                ),
                walletFilename: _wallets.First().Filename,
                walletPassword: _wallets.First().Password,
                cancellationToken: cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            tip.PaymentId = paymentId;
            tip.PaymentIdHex = integratedAddressResponse.Result.PaymentId;
            tip.IntegratedPaymentAddress = integratedAddressResponse.Result.IntegratedAddress;
            tip.CreatedDate = tip.CreatedDate ?? now;
            tip.ModifiedDate = now;
            tip.Artists = track.Artists;

            _lofiContext.Tips.Add(tip);
            await _lofiContext.SaveChangesAsync();
            return tip;
        }

        public async Task<string> GetTipPaymentUrl(int tipId, ulong? amount = null, CancellationToken cancellationToken = default)
        {
            var tip = await _lofiContext.Tips.FindAsync(new object[] { tipId }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (tip == null) throw new ArgumentException(nameof(tipId), $"No such tip with Id = {tipId}");
            if (tip.IntegratedPaymentAddress == null) throw new Exception("Cannot get a payment URL for at tip with no integrated address");

            var artistNames = await _lofiContext.Entry(tip)
                .Reference(t => t.Track)
                .Query()
                .SelectMany(t => t!.Artists)
                .Select(a => a.Name)
                .ToListAsync();
            
            var uri = await _moneroService.MakeUri(
                parameters:new MakeUriRpcParameters(
                    address: tip.IntegratedPaymentAddress,
                    amount: amount,
                    recipientName: string.Join(", ", artistNames),
                    transactionDescription: tip.Message
                ),
                walletFilename: _wallets.First().Filename,
                walletPassword: _wallets.First().Password,
                cancellationToken: cancellationToken);
            return uri.Result.Uri;
        }
    }
}