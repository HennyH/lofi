using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Lofi.API.Models.Requests
{
    public class UpsertAlbumRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? CoverPhotoFile { get; set; }
        public int[]? GenreIds { get; set; }
        public int[]? ArtistIds { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}