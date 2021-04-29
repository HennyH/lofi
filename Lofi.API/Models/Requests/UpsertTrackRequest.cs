using Microsoft.AspNetCore.Http;

namespace Lofi.API.Models.Requests
{
    public class UpsertTrackRequest
    {
        public int? TrackId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? CoverPhotoFile { get; set; }
        public IFormFile? AudioFile { get; set; }
        public int[]? GenreIds { get; set; }
        public int[]? ArtistIds { get; set; }
    }
}