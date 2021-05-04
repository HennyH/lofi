using System;
using System.ComponentModel.DataAnnotations;
using Lofi.API.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lofi.Database.Entities
{
    [Index(nameof(TipPayoutId), Name = "UIX_TipPayoutReceipt_TipPayoutId", IsUnique = true)]
    public class TipPayoutReceipt
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int? TipPayoutId { get; set; }
        [Required]
        public TipPayout? TipPayout { get; set; }
        [Required]
        public ulong? NetPayoutAmount { get; set; }
        [Required]
        public ulong? PayoutTxFee { get; set; }
        [Required]
        public ulong? PayoutTxFeeShare { get; set; }
        [Required]
        public string? TransactionId { get; set; }
        public ulong? BlockHeight { get; set; }
        [Required]
        public string? WalletAddress { get; set; }
        [Required]
        public ulong? PayoutTimestamp { get; set; }
        [Required]
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? ModifiedDate { get; set; }
    }
}