using System;
using System.ComponentModel.DataAnnotations;
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
        public string? WalletAddress { get; set; }
        [Required]
        public string? TransactionHash { get; set; }
        [Required]
        public string? TransactionKey { get; set; }
        [Required]
        public string? TransactionMetadata { get; set; }
        [Required]
        public ulong? TipAmount { get; set; }
        [Required]
        public ulong? GrossPayoutAmount { get; set; }
        [Required]
        public ulong? NetPayoutAmount { get; set; }
        [Required]
        public ulong? PayoutTxFee { get; set; }
        [Required]
        public ulong? PayoutTxFeeShare { get; set; }
        [Required]
        public bool? HasBeenPayedOut { get; set; }
        [Required]
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? ModifiedDate { get; set; }
    }
}