using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database;
using Lofi.API.Database.Entities;
using Lofi.API.Models.MoneroRpc.Parameters;
using Microsoft.EntityFrameworkCore;

namespace Lofi.API.Services
{
    public class TipService
    {
        private readonly LofiContext _lofiContext;
        private readonly MoneroService _moneroService;
        private readonly string _lofiWalletFile;
        private readonly string _lofiWalletPassword;

        public TipService(LofiContext lofiContext, MoneroService moneroSerivce, string lofiWalletFile, string lofiWalletPassword)
        {
            this._lofiContext = lofiContext;
            this._moneroService = moneroSerivce;
            this._lofiWalletFile = lofiWalletFile;
            this._lofiWalletPassword = lofiWalletPassword;
        }

        public async Task<ushort?> GetAvailablePaymentId(CancellationToken cancellationToken = default)
        {
            var availablePaymentId = await _lofiContext.Tips
                .Where(t => t.TransactionConfirmedDate.HasValue)
                .Select(t => t.PaymentId)
                .FirstOrDefaultAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (availablePaymentId != default) return availablePaymentId.Value;

            var highestPaymentIdInUser = await _lofiContext.Tips
                .Where(t => !t.TransactionConfirmedDate.HasValue)
                .Select(t => t.PaymentId)
                .OrderByDescending(id => id)
                .FirstOrDefaultAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (highestPaymentIdInUser == null) return UInt16.MinValue;
            if (highestPaymentIdInUser.Value < UInt16.MaxValue) return (ushort)(highestPaymentIdInUser.Value + 1);

            return null;
        }

        public async Task<Tip> CreateTip(int trackId, string? message = null, DateTime? now = null, CancellationToken cancellationToken = default)
        {
            now ??= DateTime.Now;

            var track = await _lofiContext.Tracks.FindAsync(new object[] { trackId }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (track == null) throw new ArgumentException(nameof(trackId), $"No such track with Id = {trackId}");

            var tip = new Tip
            {
                Message = message,
                Track = track,
                CreatedDate = DateTime.Now
            };

            await _moneroService.OpenWalletAsync(new OpenWalletRpcParameters(_lofiWalletFile, _lofiWalletPassword), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var paymentId = await GetAvailablePaymentId();
            var integratedAddressResponse = await _moneroService.MakeIntegratedAddressAsync(new MakeIntegratedAddressRpcParameters(paymentId: paymentId), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            tip.PaymentId = paymentId;
            tip.PaymentIdHex = integratedAddressResponse.Result.PaymentId;
            tip.IntegratedPaymentAddress = integratedAddressResponse.Result.IntegratedAddress;
            _lofiContext.Tips.Add(tip);
            await _lofiContext.SaveChangesAsync();
            return tip;
        }

        public async Task<string> GetTipPaymentUrl(int tipId, int? amount = null, CancellationToken cancellationToken = default)
        {
            var tip = await _lofiContext.Tips.FindAsync(new object[] { tipId }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (tip == null) throw new ArgumentException(nameof(tipId), $"No such tip with Id = {tipId}");

            await _moneroService.OpenWalletAsync(new OpenWalletRpcParameters(_lofiWalletFile, _lofiWalletPassword), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var address = await _moneroService.GetAddress(new GetAddressRpcParameters(0));
            var lofiWalletAddress = address.Result.Address;

            var artistNames = await _lofiContext.Entry(tip)
                .Reference(t => t.Track)
                .Query()
                .SelectMany(t => t!.Artists)
                .Select(a => a.Name)
                .ToListAsync();
            
            var uri = await _moneroService.MakeUri(new MakeUriRpcParameters(
                address: lofiWalletAddress,
                amount: amount,
                recipientName: string.Join(", ", artistNames),
                transactionDescription: tip.Message
            ));
            return uri.Result.Uri;
        }
    }
}