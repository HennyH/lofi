using System;
using System.ComponentModel.DataAnnotations;
using Lofi.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lofi.API.Database.Entities
{
    [Index(nameof(TipPaymentId), nameof(ArtistId), Name = "UIX_TipPayount_TipPayment_Artist", IsUnique = true)]
    public class TipPayout
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int? TipPaymentId { get; set; }
        [Required]
        public TipPayment? TipPayment { get; set; }
        [Required]
        public Artist? Artist { get; set; }
        [Required]
        public int? ArtistId { get; set; }
        [Required]
        public ulong? Amount { get; set; }
        public int? ReceiptId { get; set; }
        public TipPayoutReceipt? Receipt { get; set; }
        [Required]
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? ModifiedDate { get; set; }
    }
}