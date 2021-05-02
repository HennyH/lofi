using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Lofi.API.Models.Requests;
using Lofi.API.Models.Responses;
using Lofi.API.Tests.Utilities;

namespace Lofi.API.Tests.Actions
{
    public static class AlbumActions
    {
        public static async Task<int> CreateAlbum(
                HttpClient client,
                string title,
                string description,
                string releaseDate,
                StreamContent? coverPhoto = null,
                int[]? artistIds = null)
        {
            var formData = new MultipartFormDataContent
            {
                { new StringContent(title), nameof(UpsertAlbumRequest.Title) },
                { new StringContent(description), nameof(UpsertAlbumRequest.Description) },
                { new StringContent(releaseDate), nameof(UpsertAlbumRequest.ReleaseDate )},
            };

            if (coverPhoto != null)
            {
                formData.Add(coverPhoto, coverPhoto.Headers.ContentDisposition?.FileName ?? "cover-photo");
            }

            if (artistIds != null)
            {
                var i = 0;
                foreach (var artistId in artistIds)
                {
                    formData.Add(new StringContent(artistId.ToString()!), $"{nameof(UpsertAlbumRequest.ArtistIds)}[{i++}]");
                }
            }

            var createAlbumResponse = await client.PostAsync("/api/albums", formData);
            createAlbumResponse.StatusCode.Should().Be((int)HttpStatusCode.OK);
            return await createAlbumResponse.ParseContentAsync(text => int.Parse(text));
        }

        public static async Task<AlbumDto> GetAlbum(
                HttpClient client,
                int albumId)
        {
            var getAlbumResponse = await client.GetAsync($"/api/albums/{albumId}");
            getAlbumResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var albumDto = await getAlbumResponse.DeserializeAsJson<AlbumDto>();
            albumDto.Should().NotBeNull();
            return albumDto!;
        }

        public static async Task<int> AddTrackToAlbum(
                HttpClient client,
                int albumId,
                string title,
                StreamContent audioFile,
                int[]? artistIds = null,
                string? description = null)
        {
            var formData = new MultipartFormDataContent
            {
                { new StringContent(title), nameof(UpsertTrackRequest.Title) }
            };

            if (description != null)
            {
                formData.Add(new StringContent(description), nameof(UpsertTrackRequest.Description));
            }

            if (audioFile != null)
            {
                formData.Add(audioFile, audioFile.Headers.ContentDisposition?.FileName ?? "audio.mp3");
            }

            if (artistIds != null)
            {
                var i = 0;
                foreach (var artistId in artistIds)
                {
                    formData.Add(new StringContent(artistId.ToString()!), $"{nameof(UpsertAlbumRequest.ArtistIds)}[{i++}]");
                }
            }

            var addTrackResponse = await client.PostAsync($"/api/albums/{albumId}/tracks", formData);
            addTrackResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            return await addTrackResponse.ParseContentAsync(text => int.Parse(text))!;
        }
    } 
}