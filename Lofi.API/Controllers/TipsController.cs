using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database.Entities;
using Lofi.API.Models.MoneroRpc;
using Lofi.API.Models.MoneroRpc.Results;
using Lofi.API.Models.Requests;
using Lofi.API.Models.Responses;
using Lofi.API.Services;
using Lofi.API.Shared;
using Lofi.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lofi.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class TipsController : ControllerBase
    {
        private readonly TipService _tipService;
        private readonly LofiContext _lofiContext;

        public TipsController(TipService tipService, LofiContext lofiContext)
        {
            this._tipService = tipService;
            this._lofiContext = lofiContext;
        }

        [HttpPost("tracks/{trackId}/tips")]
        public async Task<int> CreateTip([FromRoute] int trackId, [FromBody] CreateTipRequest createTipRequest, CancellationToken cancellationToken)
        {
            var tip = await _tipService.CreateTip(trackId, createTipRequest.Message, now: DateTime.Now, cancellationToken: cancellationToken);
            return tip.Id;
        }

        [HttpGet("tips/{tipId}")]
        public async Task<ActionResult> GetTip([FromRoute] int tipId, CancellationToken cancellationToken)
        {
            var tip = await _lofiContext.Tips
                .Where(t => t.Id == tipId)
                .Include(t => t.Payment)
                .Include(t => t.Payouts)
                .FirstOrDefaultAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (tip == null) return new NotFoundObjectResult(tipId);
            return new JsonResult(new TipDto
            {
                TipId = tip.Id,
                TipMessage = tip.Message,
                TipAmount = tip.Payment?.Amount,
                IntegratedPaymentAddress = tip.IntegratedPaymentAddress,
                PaymentTransactionId = tip.Payment?.TransactionId,
                PaymentBlockHeight = tip.Payment?.BlockHeight,
                PayoutAmount = tip.Payouts?.Any() == true
                    ? (ulong?)(tip.Payouts?.Select(p => p.NetPayoutAmount)?.Sum(a => (decimal?)a ?? 0m))
                    : null,
                PayoutTxFeeShare = tip.Payouts?.Any() == true
                    ? (ulong)(tip.Payouts?.Select(p => p.PayoutTxFeeShare)?.Sum(a => (decimal?)a) ?? 0m)
                    : null
            });
        }

        [HttpGet("tips/{tipId}/payment-url")]
        public async Task<string> GetTipPaymentUrl([FromRoute] int tipId, [FromQuery] ulong? amount = null,  CancellationToken cancellationToken = default)
        {
            return await _tipService.GetTipPaymentUrl(tipId, amount);
        }

        [HttpGet("tips/{tipId}/payment-url/qr")]
        public async Task<FileResult> GetTipPaymentUrlQrCode([FromRoute] int tipId, [FromQuery] ulong? amount = null, CancellationToken cancellationToken = default)
        {
            var paymentUrl = await GetTipPaymentUrl(tipId, amount, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var qrCode = QRCodeUtils.CreateBmpQrCode(paymentUrl);
            return File(qrCode, "image/bmp");
        }
    }
}