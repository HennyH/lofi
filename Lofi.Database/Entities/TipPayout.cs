using System;
using System.ComponentModel.DataAnnotations;
using Lofi.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lofi.API.Database.Entities
{
    [Index(nameof(TipId), nameof(ArtistId), Name = "UIX_TipPayount_Tip_Artist", IsUnique = true)]
    public class TipPayout
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Tip? Tip { get; set; }
        [Required]
        public int? TipId { get; set; }
        [Required]
        public Artist? Artist { get; set; }
        [Required]
        public int? ArtistId { get; set; }
        [Required]
        public ulong? Amount { get; set; }
        public TipPayoutReceipt? Receipt {get;set;}
        [Required]
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? ModifiedDate { get; set; }
    }
}