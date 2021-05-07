using System;
using System.ComponentModel.DataAnnotations;

namespace Lofi.Database.Entities
{
    public class Transfer
    {
        public int? Id { get; set; }
        [Required]
        public string? TransactionId { get; set; }
        [Required]
        public string? FromWalletAddress { get; set; }
        [Required]
        public string? ToWalletAddress { get; set; }
        [Required]
        public ulong? Amount { get; set; }
        [Required]
        public ulong? TransactionFee { get; set; }
        [Required]
        public ushort? PaymentId { get; set; }
        public ulong? BlockHeight { get; set; }
        [Required]
        public ulong? Timestamp { get; set; }
        [Required]
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? ModifiedDate { get; set; }
    }
}