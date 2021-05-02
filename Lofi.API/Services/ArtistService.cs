using System;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database.Entities;
using Lofi.API.Models.Requests;
using Lofi.API.Models.Responses;
using Lofi.API.Shared;
using Lofi.Database;
using Microsoft.Extensions.Logging;

namespace Lofi.API.Services
{
    public class ArtistService
    {
        private readonly ILogger<ArtistService> _logger;
        private readonly LofiContext _lofiContext;

        public ArtistService(ILogger<ArtistService> logger, LofiContext lofiContext)
        {
            this._logger = logger;
            this._lofiContext = lofiContext;
        }

        public async Task<Artist> UpsertArtist(int? artistId, UpsertArtistRequest upsertArtistRequest, DateTime? now = null, CancellationToken cancellationToken = default)
        {
            now ??= DateTime.Now;

            var artist = artistId.HasValue
                ? await _lofiContext.Artists.FindAsync(new object[] { artistId.Value }, cancellationToken)
                : new Artist();
            cancellationToken.ThrowIfCancellationRequested();
            if (artist == null) throw new ArgumentException(nameof(artistId), $"No artist with id {artistId} exists");

            artist.Name = upsertArtistRequest.Name ?? artist.Name;
            artist.WalletAddress = upsertArtistRequest.WalletAddress ?? artist.WalletAddress;
            artist.ProfilePicture = upsertArtistRequest.ProfilePicture == null
                ? artist.ProfilePicture
                : await upsertArtistRequest.ProfilePicture.AsUploadedFile(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            artist.CreatedDate = artist.CreatedDate ?? now;
            artist.ModifiedDate = now;

            if (artist.Id == default)
            {
                _lofiContext.Artists.Add(artist);
            }

            await _lofiContext.SaveChangesAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return artist;
        }

        public async Task<Artist?> GetArtist(int artistId, CancellationToken cancellationToken = default)
        {
            var artist = await _lofiContext.Artists.FindAsync(new object[] { artistId }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return artist;
        }
    }
}