using System;
using Microsoft.AspNetCore.Http;

namespace Lofi.API.Models.Requests
{
    public class UpsertArtistRequest
    {
        public int? ArtistId { get; set; }
        public string? Name { get; set; }
        public string? Reference { get; set; }
        public string? WalletAddress { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}