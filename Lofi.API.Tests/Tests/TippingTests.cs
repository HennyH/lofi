using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using Lofi.API.Models.MoneroRpc.Parameters;
using Lofi.API.Models.MoneroRpc.Results;
using Lofi.API.Models.Requests;
using Lofi.API.Models.Responses;
using Lofi.API.Services;
using Lofi.API.Tests.Actions;
using Lofi.API.Tests.Utilities;
using Lofi.API.Tests.Utilities.HttpContentUtilities;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Lofi.API.Tests
{
    public class TippingTests : ApiTestFixture
    {
        private AlbumDto? _adoremusHymnalAlbum;
        private ArtistDto? _ignatiusPressArtist;
        private int? _collectaLiturgiaVerbiTrackId;
        public TippingTests(ITestOutputHelper outputHelper, ApiWebApplicationFactory factory)
            : base(outputHelper, factory)
        { }

        [Fact, Order(1)]
        public async Task Can_Tip_Track()
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
            _collectaLiturgiaVerbiTrackId = await AlbumActions.AddTrackToAlbum(
                _client,
                adoremusHymnalAlbumId,
                title: "Collecta Lliturgia Verbi",
                description: "Other music from the Mass - Latin",
                audioFile: HttpContentUtilities.FileUploadContent(path: "Assets/collecta-liturgia-verbi.mp3", mimeType: "audio/mpeg", fieldName: nameof(UpsertTrackRequest.AudioFile), fileName: "collecta-liturgia-verbi.mp3"),
                artistIds: new int[] { _ignatiusPressArtist.ArtistId.Value }
            );

            var createTipResponse = await _client.PostAsync(
                $"api/tracks/{_collectaLiturgiaVerbiTrackId}/tips",
                HttpContentUtilities.JsonContent(new CreateTipRequest
                {
                    Message = "Love your work"
                })
            );
            var tipId = await createTipResponse.ParseContentAsync(text => int.Parse(text));

            var moneroService = GetRequiredService<MoneroService>();
            var spendableBalance = await moneroService.PerformWalletRpc<GetBalanceRpcParameters, GetBalanceRpcResult>(
                MoneroWalletRpcMethod.GET_BALANCE,
                new GetBalanceRpcParameters(0),
                walletFilename: "lofiartist-test",
                walletPassword: "");
            spendableBalance.Error.Should().BeNull();
            var tipAmount = spendableBalance.Result.UnlockedBalance / 1000;

            var getPaymentUrlResponse = await _client.GetAsync($"api/tips/{tipId}/payment-url?amount={tipAmount}");
            var paymentUrl = await getPaymentUrlResponse.ParseContentAsync(text => new Uri(text));
            paymentUrl.Should().NotBeNull();
            paymentUrl.Segments.Should().HaveCount(1);
            paymentUrl.Query.Should().NotBeNullOrWhiteSpace();
            var address = paymentUrl.Segments[0];
            var queryParameters = HttpUtility.ParseQueryString(paymentUrl.Query);
            queryParameters.Get("recipient_name").Should().Be(_ignatiusPressArtist.Name);
            queryParameters.Get("tx_description").Should().Be("Love your work");

            
            var transfer = await moneroService.PerformWalletRpc<TransferRpcParameters, TransferRpcResult>(
                MoneroWalletRpcMethod.TRANSFER,
                new TransferRpcParameters(
                    destinations: new TransferRpcParameters.TransferDestination[]
                    {
                        new TransferRpcParameters.TransferDestination(tipAmount, address)
                    }),
                walletFilename: "lofiartist-test",
                walletPassword: "");
            transfer.Error.Should().BeNull();
            transfer.Result.Amount.Should().Be(tipAmount);
            transfer.Result.Fee.Should().BeGreaterThan(0);

            var (wasTipPayed, tip, _) = await WaitUtilities.WaitUntil<TipDto>(async () =>
            {
                var getTipResponse = await _client.GetAsync($"api/tips/{tipId}");
                var tip = await getTipResponse.DeserializeAsJson<TipDto>();
                if (tip == null || tip.PaymentTransactionId == null) return (false, default);
                return (true, tip);
            }, timeout: TimeSpan.FromMinutes(2));
            wasTipPayed.Should().BeTrue();
            tip!.PaymentTransactionId.Should().NotBeNullOrWhiteSpace();

            var (wasArtistPayed, payedOutTip, _) = await WaitUtilities.WaitUntil<TipDto>(async () =>
            {
                var getTipResponse = await _client.GetAsync($"api/tips/{tipId}");
                var tip = await getTipResponse.DeserializeAsJson<TipDto>();
                if (tip == null || tip.PaymentTransactionId == null) return (false, default);
                if (tip.PayoutAmount == null) return (false, default);
                return (true, tip);
            }, timeout: TimeSpan.FromMinutes(2));
            wasArtistPayed.Should().BeTrue();
            payedOutTip!.PayoutAmount.Should().BeGreaterThan(0);
        }
    }
}