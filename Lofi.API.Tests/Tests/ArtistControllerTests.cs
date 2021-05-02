using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Lofi.API.Models.Requests;
using Lofi.API.Models.Responses;
using Lofi.API.Tests.Actions;
using Lofi.API.Tests.Utilities;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Lofi.API.Tests
{
    public class ArtistControllerTests : ApiTestFixture
    {
        public ArtistControllerTests(ITestOutputHelper outputHelper, ApiWebApplicationFactory factory)
            : base(outputHelper, factory)
        { }


        [Fact, Order(1)]
        public async Task Can_Create_And_Update_Artist()
        {
            var artistId = await ArtistActions.CreateArtist(
                _client,
                name: "henry",
                walletAddress: "9vC4nbfq9m8ZkHxyEpHJPietBrUpRCMtK9zH4gAeZYW9W38rFGtjBKuAGN782nNz8P8xVcGZFVwXN7xedjHu1JqVBTfnnWw");

            var getNonExistentArtistResponse = await _client.GetAsync($"/api/artists/{int.MaxValue}");
            getNonExistentArtistResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var artist = await ArtistActions.GetArtist(_client, artistId);
            artist!.ArtistId.Should().Be(artistId);
            artist!.Name.Should().Be("henry");
            artist!.WalletAddress.Should().Be("9vC4nbfq9m8ZkHxyEpHJPietBrUpRCMtK9zH4gAeZYW9W38rFGtjBKuAGN782nNz8P8xVcGZFVwXN7xedjHu1JqVBTfnnWw");

            await ArtistActions.UpdateArtist(
                _client,
                artistId: artist!.ArtistId!.Value,
                name: "rosie");

            artist = await ArtistActions.GetArtist(_client, artistId);
            artist!.ArtistId.Should().Be(artistId);
            artist!.Name.Should().Be("rosie");
            artist!.WalletAddress.Should().Be("9vC4nbfq9m8ZkHxyEpHJPietBrUpRCMtK9zH4gAeZYW9W38rFGtjBKuAGN782nNz8P8xVcGZFVwXN7xedjHu1JqVBTfnnWw");
        }
    }
}