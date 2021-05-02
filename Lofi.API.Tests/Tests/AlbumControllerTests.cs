using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Lofi.API.Models.Requests;
using Lofi.API.Models.Responses;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Lofi.API.Tests
{
    public class AlbumControllerTests : ApiTestFixture
    {
        private AlbumDto? _adoremusHymnalAlbum;
        private ArtistDto? _ignatiusPressArtist;
        public AlbumControllerTests(ITestOutputHelper outputHelper, ApiWebApplicationFactory factory)
            : base(outputHelper, factory)
        { }


        [Fact, Order(1)]
        public async Task Can_Create_Adoremus_Hymnal_Album()
        {
            var createArtistResponse = await _client.PostAsync("/api/artists", new MultipartFormDataContent
            {
                { new StringContent("Ignatius Press"), nameof(UpsertArtistRequest.Name) },
                { new StringContent("9vC4nbfq9m8ZkHxyEpHJPietBrUpRCMtK9zH4gAeZYW9W38rFGtjBKuAGN782nNz8P8xVcGZFVwXN7xedjHu1JqVBTfnnWw"), nameof(UpsertArtistRequest.WalletAddress) }
            });
            createArtistResponse.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var ignatiusPressArtistId = await ParseResponseAsAsync(createArtistResponse, text => int.Parse(text));

            var getIgnatiusPressArtistResponse = await _client.GetAsync($"/api/artists/{ignatiusPressArtistId}");
            _ignatiusPressArtist = await DeserializeJsonAsync<ArtistDto>(getIgnatiusPressArtistResponse);
            _ignatiusPressArtist.Should().NotBeNull();
            if (_ignatiusPressArtist == null) throw new InvalidOperationException();
            _ignatiusPressArtist.ArtistId.Should().Be(ignatiusPressArtistId);

            var createAdoremusHymnalAlbumResponse = await _client.PostAsync($"/api/albums", new MultipartFormDataContent
            {
                { new StringContent("Adoremus Hymnal Album"), nameof(UpsertAlbumRequest.Title) },
                { new StringContent("Catholic music"), nameof(UpsertAlbumRequest.Description) },
                { FileUploadContent(path: "Assets/adoremus-hymnal-album-cover.png", mimeType: "image/png", nameof(UpsertAlbumRequest.CoverPhotoFile), "cover-photo.png") },
                { new StringContent("2020-01-01"), nameof(UpsertAlbumRequest.ReleaseDate )},
                { new StringContent(_ignatiusPressArtist.ArtistId!.ToString()!), $"{nameof(UpsertAlbumRequest.ArtistIds)}[0]" }
            });
            createAdoremusHymnalAlbumResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var adoremusHymnalAlbumId = await ParseResponseAsAsync(createAdoremusHymnalAlbumResponse, text => int.Parse(text));

            var getAdoremusHymnalAlbumResponse = await _client.GetAsync($"/api/albums/{adoremusHymnalAlbumId}");
            getAdoremusHymnalAlbumResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            _adoremusHymnalAlbum = await DeserializeJsonAsync<AlbumDto>(getAdoremusHymnalAlbumResponse);
            _adoremusHymnalAlbum.Should().NotBeNull();
            if (_adoremusHymnalAlbum == null) throw new InvalidOperationException();
            _adoremusHymnalAlbum.Title.Should().Be("Adoremus Hymnal Album");
            _adoremusHymnalAlbum.Description.Should().Be("Catholic music");
            _adoremusHymnalAlbum.CoverPhotoFileId.Should().NotBeNull();
            _adoremusHymnalAlbum.ArtistIds.Should().Contain(_ignatiusPressArtist.ArtistId.Value);

            var addTrackCollectaLiturgiaVerbiRequest = await _client.PostAsync($"/api/albums/{adoremusHymnalAlbumId}/tracks", new MultipartFormDataContent
            {
                { new StringContent("Collecta Lliturgia Verbi"), nameof(UpsertTrackRequest.Title) },
                { new StringContent("Other music from the Mass - Latin"), nameof(UpsertTrackRequest.Description) },
                { FileUploadContent(path: "Assets/collecta-liturgia-verbi.mp3", mimeType: "audio/mpeg", fieldName: nameof(UpsertTrackRequest.AudioFile), fileName: "collecta-liturgia-verbi.mp3") },
                { new StringContent(_ignatiusPressArtist.ArtistId!.ToString()!), $"{nameof(UpsertTrackRequest.ArtistIds)}[0]" }
            });
            addTrackCollectaLiturgiaVerbiRequest.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}