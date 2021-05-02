using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database.Entities;
using Lofi.API.Models.Requests;
using Lofi.API.Shared;
using Lofi.Database;
using Microsoft.EntityFrameworkCore;

namespace Lofi.API.Services
{
    public class AlbumService
    {
        private readonly LofiContext _lofiContext;
        private readonly TrackService _trackService;

        public AlbumService(LofiContext lofiContext, TrackService trackService)
        {
            this._lofiContext = lofiContext;
            this._trackService = trackService;
        }

        public async Task<Album> UpsertAlbum(int? albumId, UpsertAlbumRequest upsertAlbumRequest, DateTime? now = null, CancellationToken cancellationToken = default)
        {
            now ??= DateTime.Now;
            var album = albumId.HasValue
                ? await _lofiContext.FindAsync<Album>(new object[] { albumId.Value }, cancellationToken)
                : new Album() { UniqueId = Guid.NewGuid() };
            cancellationToken.ThrowIfCancellationRequested();

            album.Title = upsertAlbumRequest.Title ?? album.Title;
            album.Description = upsertAlbumRequest.Description ?? album.Description;
            album.CoverPhotoFile = upsertAlbumRequest.CoverPhotoFile == null
                ? album.CoverPhotoFile
                : await upsertAlbumRequest.CoverPhotoFile.AsUploadedFile(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            album.ReleaseDate = upsertAlbumRequest.ReleaseDate ?? album.ReleaseDate;
            album.CreatedDate = album.CreatedDate ?? now;
            album.ModifiedDate = now;

            if (upsertAlbumRequest.GenreIds != null)
            {
                await EfUtils.SynchronizeCollectionByKey(
                    _lofiContext,
                    album,
                    album => album.Genres,
                    genre => genre.Id,
                    upsertAlbumRequest.GenreIds,
                    cancellationToken
                );
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (upsertAlbumRequest.ArtistIds != null)
            {
                await EfUtils.SynchronizeCollectionByKey(
                    _lofiContext,
                    album,
                    album => album.Artists,
                    artist => artist.Id,
                    upsertAlbumRequest.ArtistIds,
                    cancellationToken
                );
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (album.Id == default)
            {
                _lofiContext.Albums.Add(album);
            }
            
            await _lofiContext.SaveChangesAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return album;
        }

        public async Task<Track> AddTrackToAlbum(int albumId, UpsertTrackRequest upsertTrackRequest, DateTime? now = null, CancellationToken cancellationToken = default)
        {
            now ??= DateTime.Now;
            var album = await _lofiContext.Albums.FindAsync(new object[] { albumId }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (album == null) throw new ArgumentException(nameof(albumId), $"No such album with Id = {albumId}");

            var track = await _trackService.UpsertTrack(upsertTrackRequest, now, saveChanges: false, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            track.TrackNumber = await _lofiContext.Entry(album)
                .Collection(a => a.Tracks)
                .Query()
                .Select(t => t.TrackNumber + 1)
                .MaxAsync(cancellationToken)
                ?? 1;
            cancellationToken.ThrowIfCancellationRequested();

            album.Tracks.Add(track);
            album.ModifiedDate = now;
            await _lofiContext.SaveChangesAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return track;
        }

        public async Task SetTrackNumber(int albumId, int trackId, int trackNumber, DateTime? now = null, CancellationToken cancellationToken = default)
        {
            if (trackNumber <= 0) throw new ArgumentOutOfRangeException(nameof(trackNumber), "track number cannot be negative");

            now ??= DateTime.Now;
            var album = await _lofiContext.FindAsync<Album>(new object[] { albumId }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (album == null) throw new ArgumentException(nameof(albumId), $"No such album with Id = {albumId}");
            
            await _lofiContext.Entry(album)
                .Collection(a => a.Tracks)
                .LoadAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (!album.Tracks.Any(t => t.Id == trackId)) throw new ArgumentException(nameof(trackId), $"No track with Id = {trackId} belongs to the album with Id = {albumId}");

            var currentHighestTrackNumber = album.Tracks.Max(t => t.TrackNumber);
            if (trackNumber > currentHighestTrackNumber) throw new ArgumentOutOfRangeException(nameof(trackNumber), $"The track number must not be greater than the current highest track number of {currentHighestTrackNumber}");
            foreach (var track in album.Tracks)
            {
                if (track.Id == trackId)
                {
                    track.TrackNumber = trackNumber;
                }
                else if (track.TrackNumber >= trackNumber)
                {
                    track.TrackNumber += 1;
                }
            }

            album.ModifiedDate = now;

            await _lofiContext.SaveChangesAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
        }

        public async Task<Album?> GetAlbum(int albumId, CancellationToken cancellationToken = default)
        {
            var album = await _lofiContext.Albums
                .Where(a => a.Id == albumId)
                .Include(a => a.Genres)
                .Include(a => a.Artists)
                .Include(a => a.Tracks)
                .FirstOrDefaultAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return album;
        }
    }
}