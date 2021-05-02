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
    public static class ArtistActions
    {
        public static async Task<int> CreateArtist(HttpClient client, string name, string walletAddress)
        {
            var createArtistResponse = await client.PostAsync("/api/artists", new MultipartFormDataContent
            {
                { new StringContent(name), nameof(UpsertArtistRequest.Name) },
                { new StringContent(walletAddress), nameof(UpsertArtistRequest.WalletAddress) }
            });
            createArtistResponse.StatusCode.Should().Be((int)HttpStatusCode.OK);
            return await createArtistResponse.ParseContentAsync(text => int.Parse(text));
        }

        public static async Task UpdateArtist(HttpClient client, int artistId, string? name = null, string? walletAddress = null)
        {
            var formData = new MultipartFormDataContent();
            if (name != null) formData.Add(new StringContent(name), nameof(UpsertArtistRequest.Name));
            if (walletAddress != null) formData.Add(new StringContent(walletAddress), nameof(UpsertArtistRequest.WalletAddress));
            var updateArtistResponse = await client.PutAsync($"/api/artists/{artistId}", formData);
            updateArtistResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public static async Task<ArtistDto> GetArtist(HttpClient client, int artistId)
        {
            var getArtistResponse = await client.GetAsync($"/api/artists/{artistId}");
            var artistDto = await getArtistResponse.DeserializeAsJson<ArtistDto>();
            artistDto.Should().NotBeNull();
            if (artistDto == null) throw new InvalidOperationException();
            artistDto.ArtistId.Should().Be(artistId);
            return artistDto;
        }
    } 
}