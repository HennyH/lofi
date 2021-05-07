using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database;
using Lofi.API.Models.Responses;
using Lofi.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lofi.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class TrackController : ControllerBase
    {
        private readonly ILogger<TrackController> _logger;
        private readonly LofiContext _lofiContext;

        public TrackController(ILogger<TrackController> logger, LofiContext lofiContext)
        {
            this._logger = logger;
            this._lofiContext = lofiContext;
        }

        [HttpGet("tracks/random")]
        public async Task<ActionResult> GetRandomTrack(CancellationToken cancellationToken)
        {
            var track = await _lofiContext.Tracks
                .FromSqlInterpolated(
                    $@"
                        SELECT
                            *
                        FROM tracks
                        ORDER BY random() ASC
                        LIMIT 1
                    "
                )
                .Include(track => track.Album)
                .SingleOrDefaultAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (track == null) return new NotFoundResult();
            return new JsonResult(new TrackDto
            {
                TrackId = track.Id,
                TrackName = track.Title,
                TrackDescription = track.Description,
                AlbumId = track.AlbumId,
                AlbumDescription = track.Album?.Description,
                AlbumName = track.Album?.Title
            });
        }

        [HttpGet("tracks/{trackId}/cover")]
        public async Task<ActionResult> GetTrackCover([FromRoute] int trackId, CancellationToken cancellationToken)
        {
            var track = await _lofiContext.Tracks
                .Where(t => t.Id == trackId)
                .Include(t => t.CoverPhotoFile)
                .Include(t => t.Album)
                    .ThenInclude(a => a.CoverPhotoFile)
                .FirstOrDefaultAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (track == null) return new NotFoundResult();
            cancellationToken.ThrowIfCancellationRequested();
            if (track.CoverPhotoFile != null) return File(track.CoverPhotoFile!.Bytes, track.CoverPhotoFile.MimeType);
            if (track.Album?.CoverPhotoFile != null) return File(track.Album.CoverPhotoFile!.Bytes, track.Album.CoverPhotoFile!.MimeType);
            return new NotFoundResult();
        }

        [HttpGet("tracks/{trackId}/cover/mime-type")]
        public async Task<ActionResult> GetTrackCoverMimeType([FromRoute] int trackId, CancellationToken cancellationToken)
        {
            var track = await _lofiContext.Tracks.FindAsync(new object[] { trackId }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (track == null) return new NotFoundResult();
            await _lofiContext.Entry(track)
                .Reference(track => track.CoverPhotoFile)
                .LoadAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return new ObjectResult(track.CoverPhotoFile!.MimeType);
        }

        [HttpGet("tracks/{trackId}/audio")]
        public async Task<ActionResult> GetTrackAudio([FromRoute] int trackId, CancellationToken cancellationToken)
        {
            var track = await _lofiContext.Tracks.FindAsync(new object[] { trackId }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (track == null) return new NotFoundResult();
            await _lofiContext.Entry(track)
                .Reference(track => track.AudioFile)
                .LoadAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return new FileContentResult(track.AudioFile!.Bytes, track.AudioFile.MimeType);
        }       

        [HttpGet("tracks/{trackId}/audio/mime-type")]
        public async Task<ActionResult> GetTrackAudioMimeType([FromRoute] int trackId, CancellationToken cancellationToken)
        {
            var track = await _lofiContext.Tracks.FindAsync(new object[] { trackId }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (track == null) return new NotFoundResult();
            await _lofiContext.Entry(track)
                .Reference(track => track.AudioFile)
                .LoadAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return new ObjectResult(track.AudioFile!.MimeType);
        }
    }
}