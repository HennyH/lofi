using Lofi.API.Database;
using Lofi.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lofi.API.Controllers
{
    [ApiController]
    [Route("api/playlist")]
    public class PlaylistController : ControllerBase
    {
        private readonly ILogger<PlaylistController> _logger;
        private readonly LofiContext _lofiContext;

        public PlaylistController(ILogger<PlaylistController> logger, LofiContext lofiContext)
        {
            this._logger = logger;
            this._lofiContext = lofiContext;
        }

        // [HttpGet("radio")]
        // public async Task<FileContentResult> GetRadioPlaylist(CancellationToken cancellationToken)
        // {
        //     // var songs = await _lofiContext.Songs
        //     //     .Select(s => new
        //     //     {
        //     //         s.Hash,
        //     //         s.Title,
        //     //         s.Description,
        //     //         s.ReleaseDate,
        //     //         s.AudioMimeType
        //     //     })
        //     //     .ToListAsync();
        //     // cancellationToken.ThrowIfCancellationRequested();
            
        //     // var ms = new MemoryStream();
        //     // using (var writer = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true))
        //     // {
        //     //     foreach (var song in songs)
        //     //     {
        //     //         await writer.WriteLineAsync($"annotate:hash=\"{song.Hash}\",title=\"{song.Title}\",description=\"{song.Description}\",released=\"{song.ReleaseDate}\":/music/{song.Hash}{MimeTypeUtils.TryGetExtension(song.AudioMimeType ?? "audio/mp3")}");
        //     //     }
        //     //     await writer.FlushAsync();
        //     // }
        //     // ms.Seek(0, SeekOrigin.Begin);
        //     // var bytes = ms.ToArray();
        //     // var file = File(bytes, "audio/scpls", "playlist.pls");
        //     // file.EnableRangeProcessing = true;
        //     // return file;
        // }
    }
}