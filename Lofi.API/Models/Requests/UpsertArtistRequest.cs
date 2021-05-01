using System;
using Microsoft.AspNetCore.Http;

namespace Lofi.API.Models.Requests
{
    public class UpsertArtistRequest
    {
        public string? Name { get; set; }
        public string? WalletAddress { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}