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
using Lofi.API.Tests.Actions;
using Lofi.API.Tests.Utilities.HttpContentUtilities;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Lofi.API.Tests
{
    public class AlbumControllerTests : ApiTestFixture
    {
        private AlbumDto? _adoremusHymnalAlbum;
        private ArtistDto? _ignatiusPressArtist;
        private int? _collectaLiturgiaVerbiTrackId;
        public AlbumControllerTests(ITestOutputHelper outputHelper, ApiWebApplicationFactory factory)
            : base(outputHelper, factory)
        { }


        [Fact, Order(1)]
        public async Task Can_Create_Adoremus_Hymnal_Album()
        {
            var ignatiusPressArtistId = await ArtistActions.CreateArtist(
                _client,
                name: "Ignatius Press",
                walletAddress: "9vC4nbfq9m8ZkHxyEpHJPietBrUpRCMtK9zH4gAeZYW9W38rFGtjBKuAGN782nNz8P8xVcGZFVwXN7xedjHu1JqVBTfnnWw");

            _ignatiusPressArtist = await ArtistActions.GetArtist(_client, ignatiusPressArtistId);

            var adoremusHymnalAlbumId = await AlbumActions.CreateAlbum(
                _client,
                title: "Adoremus Hymnal Album",
                description: "Catholic music",
                releaseDate: "2020-01-01",
                coverPhoto: HttpContentUtilities.FileUploadContent(path: "Assets/adoremus-hymnal-album-cover.png", mimeType: "image/png", fieldName: nameof(UpsertAlbumRequest.CoverPhotoFile), fileName: "cover-photo.png"),
                artistIds: new int[] { _ignatiusPressArtist.ArtistId!.Value }
            );

            _adoremusHymnalAlbum = await AlbumActions.GetAlbum(_client, adoremusHymnalAlbumId);
            _adoremusHymnalAlbum.Title.Should().Be("Adoremus Hymnal Album");
            _adoremusHymnalAlbum.Description.Should().Be("Catholic music");
            _adoremusHymnalAlbum.CoverPhotoFileId.Should().NotBeNull();
            _adoremusHymnalAlbum.ArtistIds.Should().Contain(_ignatiusPressArtist.ArtistId.Value);

            _collectaLiturgiaVerbiTrackId = await AlbumActions.AddTrackToAlbum(
                _client,
                adoremusHymnalAlbumId,
                title: "Collecta Lliturgia Verbi",
                description: "Other music from the Mass - Latin",
                audioFile: HttpContentUtilities.FileUploadContent(path: "Assets/collecta-liturgia-verbi.mp3", mimeType: "audio/mpeg", fieldName: nameof(UpsertTrackRequest.AudioFile), fileName: "collecta-liturgia-verbi.mp3"),
                artistIds: new int[] { _ignatiusPressArtist.ArtistId.Value }
            );
        }
    }
}