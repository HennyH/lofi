using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Lofi.API.Database;
using Lofi.API.Database.Entities;
using Lofi.API.Models.Requests;
using Lofi.API.Shared;

namespace Lofi.API.Services
{
    public class TrackService
    {
        private readonly LofiContext _lofiContext;

        public TrackService(LofiContext lofiContext)
        {
            this._lofiContext = lofiContext;
        }

        public async Task<Track> UpsertTrack(UpsertTrackRequest upsertTrackRequest, DateTime? now = null, bool? saveChanges = null, CancellationToken cancellationToken = default)
        {
            now ??= DateTime.Now;
            var track = upsertTrackRequest.TrackId.HasValue
                ? await _lofiContext.FindAsync<Track>(new object[] { upsertTrackRequest.TrackId.Value }, cancellationToken)
                : new Track();
            cancellationToken.ThrowIfCancellationRequested();
            if (track == null) throw new ArgumentException(nameof(upsertTrackRequest.TrackId), $"No such track exists with Id = {upsertTrackRequest.TrackId}");

            track.Title = upsertTrackRequest.Title ?? track.Title;
            track.Description = upsertTrackRequest.Description ?? track.Description;
            track.CoverPhotoFile = upsertTrackRequest.CoverPhotoFile == null
                ? track.CoverPhotoFile
                : await UploadedFile.FromFormFile(upsertTrackRequest.CoverPhotoFile);
            track.AudioFile = upsertTrackRequest.AudioFile == null
                ? track.AudioFile
                : await UploadedFile.FromFormFile(upsertTrackRequest.AudioFile);
            
            if (upsertTrackRequest.GenreIds != null)
            {
                await EfUtils.SynchronizeCollectionByKey(
                    _lofiContext,
                    track,
                    track => track.Genres,
                    genre => genre.Id,
                    upsertTrackRequest.GenreIds,
                    cancellationToken
                );
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (upsertTrackRequest.ArtistIds != null)
            {
                await EfUtils.SynchronizeCollectionByKey(
                    _lofiContext,
                    track,
                    track => track.Artists,
                    artist => artist.Id,
                    upsertTrackRequest.ArtistIds,
                    cancellationToken
                );
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (track.Id == default)
            {
                _lofiContext.Tracks.Add(track);
            }

            if (saveChanges == true || (saveChanges == null && track.Id != default))
            {
                await _lofiContext.SaveChangesAsync(cancellationToken);
            }
            cancellationToken.ThrowIfCancellationRequested();
            return track;
        }
    }
}