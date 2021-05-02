using System;

namespace Lofi.API.Models.Responses
{
    public class AlbumDto
    {
        public int AlbumId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? CoverPhotoFileId { get; set; }
        public int[] GenreIds { get; set; } = new int[0];
        public int[] ArtistIds { get; set; } = new int[0];
        public int[] TrackIds { get; set; } = new int[0];
    }
}