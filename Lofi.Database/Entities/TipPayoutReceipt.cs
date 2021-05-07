using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lofi.API.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lofi.Database.Entities
{
    public class TipPayoutReceipt
    {
        [Key]
        public int Id { get; set; }
        public ICollection<TipPayout> TipPayouts { get; set; } = new List<TipPayout>();
        [Required]
        public ulong? NetPayoutAmount { get; set; }
        [Required]
        public ulong? PayoutTxFee { get; set; }
        [Required]
        public ulong? PayoutTxFeeShare { get; set; }
        [Required]
        public string? TransactionId { get; set; }
        [Required]
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? ModifiedDate { get; set; }
    }
}