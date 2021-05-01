using System;
using System.ComponentModel.DataAnnotations;

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
        public string? TransactionId { get; set; }
        [Required]
        public ulong? BlockHeight { get; set; }
        [Required]
        public ulong? Amount { get; set; }
        [Required]
        public ulong? Timestamp { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}