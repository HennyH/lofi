using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lofi.API.Database.Entities
{
    public class Album
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Guid? UniqueId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public UploadedFile? CoverPhotoFile { get; set; }
        public int CoverPhotoFileId { get; set; }
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<Artist> Artists { get; set; } = new List<Artist>();
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
        public DateTime? ReleaseDate { get; set; }
        [Required]
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? ModifiedDate { get; set; }
    }
}