using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database.Entities;
using Lofi.API.Models.MoneroRpc;
using Lofi.API.Models.MoneroRpc.Results;
using Lofi.API.Models.Requests;
using Lofi.API.Services;
using Lofi.API.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lofi.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class TipsController : ControllerBase
    {
        private readonly TipService _tipService;

        public TipsController(TipService tipService)
        {
            this._tipService = tipService;
        }

        [HttpPost("tracks/{trackId}/tips")]
        public async Task<int> CreateAlbum([FromRoute] int trackId, [FromBody] CreateTipRequest createTipRequest, CancellationToken cancellationToken)
        {
            var tip = await _tipService.CreateTip(trackId, createTipRequest.Message, now: DateTime.Now, cancellationToken: cancellationToken);
            return tip.Id;
        }

        [HttpGet("tips/{tipId}/payment-url")]
        public async Task<string> GetTipPaymentUrl([FromRoute] int tipId, [FromQuery] int? amount = null,  CancellationToken cancellationToken = default)
        {
            return await _tipService.GetTipPaymentUrl(tipId, amount);
        }

        [HttpGet("tips/{tipId}/payment-url/qr")]
        public async Task<FileResult> GetTipPaymentUrlQrCode([FromRoute] int tipId, [FromQuery] int? amount = null, CancellationToken cancellationToken = default)
        {
            var paymentUrl = await GetTipPaymentUrl(tipId, amount, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var qrCode = QRCodeUtils.CreateBmpQrCode(paymentUrl);
            return File(qrCode, "image/bmp");
        }

        [HttpGet("transfers")]
        public async Task<MoneroRpcResponse<GetTransfersRpcResult>> GetTransfers(CancellationToken cancellationToken)
        {
            return await _tipService.GetTransfers(cancellationToken);
        }

        [HttpGet("payments")]
        public async Task<MoneroRpcResponse<GetPaymentsRpcResult>> GetPayments([FromQuery] ushort paymentId, CancellationToken cancellationToken)
        {
            return await _tipService.GetPayments(paymentId, cancellationToken);
        }
        
    }
}