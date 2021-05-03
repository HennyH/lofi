namespace Lofi.API.Models.Responses
{
    public class TipDto
    {
        public int? TipId { get; set; }
        public string? TipMessage { get; set; }
        public ulong? TipAmount { get; set; }
        public string? IntegratedPaymentAddress { get; set; }
        public string? PaymentTransactionId { get; set; }
        public ulong? PaymentBlockHeight { get; set; }
        public ulong? PayoutAmount { get; set; }
        public ulong? PayoutTxFeeShare { get; set; }
    }
}