using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lofi.Database.Entities;

namespace Lofi.API.Database.Entities
{
    public class TipPayment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Tip? Tip { get; set; }
        [Required]
        public int? TipId { get; set; }
        [Required]
        public int? PaymentTransferId { get; set; }
        [Required]
        public Transfer? PaymentTransfer { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ICollection<TipPayout> Payouts { get; set; } = new List<TipPayout>();
    }
}