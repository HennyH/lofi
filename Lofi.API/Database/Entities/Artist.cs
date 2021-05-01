using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lofi.API.Database.Entities
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? WalletAddress { get; set; }
        public UploadedFile? ProfilePicture { get; set; }
        [Required]
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ICollection<Album> Albums { get; set; } = new List<Album>();
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
        public ICollection<Tip> Tips { get; set; } = new List<Tip>();
    }
}