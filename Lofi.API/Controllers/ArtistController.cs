using System;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Models.Requests;
using Lofi.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lofi.API.Controllers
{
    [ApiController]
    [Route("api/artists")]
    public class ArtistController : ControllerBase
    {
        private readonly ArtistService _artistService;

        public ArtistController(ArtistService artistService)
        {
            this._artistService = artistService;
        }

        [HttpPut("{artistId}")]
        public async Task UpdateArtist([FromRoute] int artistId, [FromForm] UpsertArtistRequest upsertArtistRequest, CancellationToken cancellationToken = default)
        {
            await _artistService.UpsertArtist(artistId: artistId, upsertArtistRequest, now: DateTime.Now, cancellationToken: cancellationToken);
        }

        [HttpPost]
        public async Task<int> CreateArtist([FromForm] UpsertArtistRequest upsertArtistRequest, CancellationToken cancellationToken = default)
        {
            var artist = await _artistService.UpsertArtist(artistId: null, upsertArtistRequest, now: DateTime.Now, cancellationToken: cancellationToken);
            return artist.Id;
        }      
    }
}