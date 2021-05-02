using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Models.Requests;
using Lofi.API.Models.Responses;
using Lofi.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lofi.API.Controllers
{
    [ApiController]
    [Route("api/albums")]
    public class AlbumsController : ControllerBase
    {
        private readonly AlbumService _albumService;

        public AlbumsController(AlbumService albumService)
        {
            this._albumService = albumService;
        }

        [HttpPatch("{albumId}")]
        public async Task UpdateAlbum([FromRoute] int albumId, [FromForm] UpsertAlbumRequest upsertAlbumRequest, CancellationToken cancellationToken)
        {
            await _albumService.UpsertAlbum(albumId: albumId, upsertAlbumRequest, now: DateTime.Now, cancellationToken: cancellationToken);
        }

        [HttpPost]
        public async Task<int> CreateAlbum([FromForm] UpsertAlbumRequest upsertAlbumRequest, CancellationToken cancellationToken)
        {
            var album = await _albumService.UpsertAlbum(albumId: null, upsertAlbumRequest, now: DateTime.Now, cancellationToken: cancellationToken);
            return album.Id;
        }

        [HttpPost]
        [Route("{albumId}/tracks")]
        public async Task AddTrackToAlbum([FromRoute] int albumId, [FromForm] UpsertTrackRequest upsertTrackRequest, CancellationToken cancellationToken)
        {
            await _albumService.AddTrackToAlbum(albumId, upsertTrackRequest, now: DateTime.Now, cancellationToken: cancellationToken);
        }

        [HttpPut]
        [Route("{albumId}/tracks/{trackId}/number")]
        public async Task SetAlbumTrackNumber([FromRoute] int albumId, [FromRoute] int trackId, [FromBody] int trackNumber, CancellationToken cancellationToken)
        {
            await _albumService.SetTrackNumber(albumId, trackId, trackNumber, now: DateTime.Now, cancellationToken: cancellationToken);
        }

        [HttpGet]
        [Route("{albumId}")]
        public async Task<ActionResult> GetAlbum([FromRoute] int albumId, CancellationToken cancellationToken = default)
        {
            var album = await _albumService.GetAlbum(albumId);
            if (album == null) return new NotFoundResult();
            return new JsonResult(new AlbumDto
            {
                AlbumId = album.Id,
                Title = album.Title,
                Description = album.Description,
                GenreIds = album.Genres.Select(g => g.Id).ToArray(),
                ArtistIds = album.Artists.Select(a => a.Id).ToArray(),
                TrackIds = album.Tracks.Select(t => t.Id).ToArray(),
                CoverPhotoFileId = album.CoverPhotoFileId
            });
        }
    }
}