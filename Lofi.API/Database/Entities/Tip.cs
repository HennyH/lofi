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
        public Track? Track { get; set; }
        [Required]
        public int? TrackId { get; set; }
        public string? IntegratedPaymentAddress { get; set; }
        public string? TransactionHash { get; set; }
        public bool IsPaymentConfirmed { get; set; } = false;
        [Required]
        public DateTime? TipDate { get; set; }
    }
}