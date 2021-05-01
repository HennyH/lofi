using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lofi.API.Database.Entities
{
    public class Tip
    {
        [Key]
        public int Id { get; set; }
        public string? Message { get; set; }
        [Required]
        public ushort? PaymentId { get; set; }
        [Required]
        public string? PaymentIdHex { get; set; }
        [Required]
        public Track? Track { get; set; }
        [Required]
        public int? TrackId { get; set; }
        public ICollection<Artist> Artists { get; set; } = new List<Artist>();
        [Required]
        public string? IntegratedPaymentAddress { get; set; }
        public TipPayment? Payment { get; set; }
        public ICollection<TipPayout> Payouts { get; set; } = new List<TipPayout>();
        [Required]
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? ModifiedDate { get; set; }
    }
}