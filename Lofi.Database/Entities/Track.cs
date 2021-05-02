using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lofi.API.Database.Entities
{
    public class Track
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int? TrackNumber { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<Artist> Artists { get; set; } = new List<Artist>();
        public UploadedFile? CoverPhotoFile { get; set; }
        public int? CoverPhotoFileId { get; set; }
        public UploadedFile? AudioFile { get; set; }
        public int? AudioFileId { get; set; }
        [Required]
        public Album? Album { get; set; }
        [Required]
        public int? AlbumId { get; set; }
    }
}