using System;
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
        [Required]
        public string? IntegratedPaymentAddress { get; set; }
        public string? TransactionId { get; set; }
        [Required]
        public DateTime? CreatedDate { get; set; }
        public ulong? BlockHeight { get; set; }
        public DateTime? PayedOutDate { get; set; }
    }
}