using System.Net;
using System.Net.Http;
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
    public class ArtistControllerTests : ApiTestFixture
    {
        public ArtistControllerTests(ITestOutputHelper outputHelper, ApiWebApplicationFactory factory)
            : base(outputHelper, factory)
        { }


        [Fact, Order(1)]
        public async Task Can_Create_And_Update_Artist()
        {
            var createArtistResponse = await _client.PostAsync("/api/artists", new MultipartFormDataContent
            {
                { new StringContent("henry"), nameof(UpsertArtistRequest.Name) },
                { new StringContent("9vC4nbfq9m8ZkHxyEpHJPietBrUpRCMtK9zH4gAeZYW9W38rFGtjBKuAGN782nNz8P8xVcGZFVwXN7xedjHu1JqVBTfnnWw"), nameof(UpsertArtistRequest.WalletAddress) }
            });
            createArtistResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            if (!int.TryParse(await createArtistResponse.Content.ReadAsStringAsync(), out var artistId)) { }
            artistId.Should().BeGreaterThan(0);

            var getNonExistentArtistResponse = await _client.GetAsync($"/api/artists/{int.MaxValue}");
            getNonExistentArtistResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var getArtistWithHenryNameResponse = await _client.GetAsync($"/api/artists/{artistId}");
            getArtistWithHenryNameResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var artistWithHenryName = await DeserializeJsonAsync<ArtistDto>(getArtistWithHenryNameResponse);
            artistWithHenryName.Should().NotBeNull();
            artistWithHenryName!.ArtistId.Should().Be(artistId);
            artistWithHenryName!.Name.Should().Be("henry");
            artistWithHenryName!.WalletAddress.Should().Be("9vC4nbfq9m8ZkHxyEpHJPietBrUpRCMtK9zH4gAeZYW9W38rFGtjBKuAGN782nNz8P8xVcGZFVwXN7xedjHu1JqVBTfnnWw");

            var updatedArtistResponse = await _client.PutAsync($"/api/artists/{artistId}", new MultipartFormDataContent
            {
                { new StringContent("rosie"), nameof(UpsertArtistRequest.Name) }
            });
            updatedArtistResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var getArtistWithRoiseNameResponse = await _client.GetAsync($"/api/artists/{artistId}");
            getArtistWithRoiseNameResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var artistWithRosieName = await DeserializeJsonAsync<ArtistDto>(getArtistWithRoiseNameResponse);
            artistWithRosieName.Should().NotBeNull();
            artistWithRosieName!.ArtistId.Should().Be(artistId);
            artistWithRosieName!.Name.Should().Be("rosie");
            artistWithRosieName!.WalletAddress.Should().Be("9vC4nbfq9m8ZkHxyEpHJPietBrUpRCMtK9zH4gAeZYW9W38rFGtjBKuAGN782nNz8P8xVcGZFVwXN7xedjHu1JqVBTfnnWw");
        }
    }
}