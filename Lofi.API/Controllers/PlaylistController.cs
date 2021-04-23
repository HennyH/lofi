using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Lofi.API.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lofi.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class PlaylistController : ControllerBase
    {
        private readonly ILogger<PlaylistController> _logger;
        private readonly LofiContext _lofiContext;

        public PlaylistController(ILogger<PlaylistController> logger, LofiContext lofiContext)
        {
            this._logger = logger;
            this._lofiContext = lofiContext;
        }

        [HttpGet("radio")]
        public async Task<FileContentResult> GetRadioPlaylist()
        {
            // var songs = _lofiContext.Songs
            //     .Select(s => new
            //     {
            //         SongId = s.Id,
            //         s.Hash,
            //         s.Title,
            //         AlbumTitle = s.Album.Title,
            //         s.Description,
            //         s.Genre,
            //         s.Id,
            //         Artist = s.Artists.Select(a => a.Name).FirstOrDefault(),
            //         Wallet = s.Artists.Select(a => a.WalletAddress).FirstOrDefault()
            //     });
            var songs = new []
            {
                new { Id = 1, AlbumTitle = "Album", Title = "Song", Wallet = "111", Hash = "xb53d" },
                new { Id = 1, AlbumTitle = "Album", Title = "Song", Wallet = "111", Hash = "xb53d" },
                new { Id = 1, AlbumTitle = "Album", Title = "Song", Wallet = "111", Hash = "xb53d" }
            };

            var ms = new MemoryStream();
            using (var writer = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true))
            {
                foreach (var song in songs)
                {
                    continue;
                    await writer.WriteLineAsync($"annotate:id=\"{song.Id}\",album=\"{song.AlbumTitle}\",title=\"{song.Title}\",wallet=\"{song.Wallet}\":{song.Hash}.mp3");
                }
                await writer.FlushAsync();
            }
            ms.Seek(0, SeekOrigin.Begin);
            var bytes = ms.ToArray();
            var file = File(bytes, MediaTypeNames.Text.Plain, "playlist.pls");
            file.EnableRangeProcessing = true;
            return file;
        }
    }
}