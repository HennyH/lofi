using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Lofi.API.Database;
using Lofi.API.Database.Entities;
using Lofi.API.Shared;
using Lofi.API.Shared.FileSizeUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lofi.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class AlbumController : ControllerBase
    { 
        private readonly ILogger<AlbumController> _logger;
        private readonly LofiContext _lofiContext;

        public AlbumController(ILogger<AlbumController> logger, LofiContext lofiContext)
        {
            this._logger = logger;
            this._lofiContext = lofiContext;
        }

        [HttpPost("album")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> Upload([FromForm] AlbumUploadRequestModel albumUpload)
        {
            if (string.IsNullOrEmpty(albumUpload.AlbumTitle))
            {
                throw new ArgumentException(nameof(albumUpload.AlbumTitle), "The title cannot be null");
            }

            if (string.IsNullOrEmpty(albumUpload.AlbumDescription))
            {
                throw new ArgumentException(nameof(albumUpload.AlbumDescription), "The description cannot be null");
            }

            if (string.IsNullOrEmpty(albumUpload.ArtistName))
            {
                throw new ArgumentException(nameof(albumUpload.ArtistName), "The artist name cannot be null");
            }

            if (string.IsNullOrEmpty(albumUpload.ArtistWallet))
            {
                throw new ArgumentException(nameof(albumUpload.ArtistWallet), "The artist wallet cannot be null");
            }

            if (albumUpload.AlbumCoverPhoto == null)
            {
                throw new ArgumentException(nameof(albumUpload.AlbumCoverPhoto), "The cover photo cannot be null");
            }

            if (albumUpload.Songs == null)
            {
                throw new ArgumentException(nameof(albumUpload.AlbumCoverPhoto), "There must be at least one song");
            }

            int maxFileSize = FileSizeUtils.MegaBytes(10);
            var artist =
                _lofiContext.Artists
                    .Where(a => a.Name == albumUpload.ArtistName)
                    .FirstOrDefault()
                ?? new Artist(albumUpload.ArtistName, albumUpload.ArtistWallet, "pgpkey");

            var songs = new List<Song>();
            foreach (var s in albumUpload.Songs)
            {
                var audioBytes = await s.ToArrayAsync();
                songs.Add(new Song(
                        title: s.FileName,
                        description: s.FileName,
                        genre: "music",
                        uploadedAt: DateTime.Now,
                        coverPhotoBytes: await albumUpload.AlbumCoverPhoto.ToArrayAsync(maxFileSize),
                        coverPhotoMimeType: albumUpload.AlbumCoverPhoto.ContentType,
                        artists: new List<Artist>(new [] { artist }),
                        audioBytes: audioBytes,
                        audioMimeType: s.ContentType,
                        hash: await HashUtils.Sha1HexDigestAsync(audioBytes)
                ));
            }

            var album = new Album(
                title: albumUpload.AlbumTitle,
                description: albumUpload.AlbumDescription,
                coverPhotoBytes: await albumUpload.AlbumCoverPhoto.ToArrayAsync(maxFileSize),
                coverPhotoMimeType: albumUpload.AlbumCoverPhoto.ContentType,
                releaseDate: DateTime.Now,
                uploadDate: DateTime.Now,
                artists:  new List<Artist>(new [] { artist }),
                songs: songs
            );
            _lofiContext.Albums.Add(album);
            await _lofiContext.SaveChangesAsync();
            return StatusCode((int)HttpStatusCode.OK);
        }
    }
}
