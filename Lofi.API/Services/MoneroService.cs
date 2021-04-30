using System;
using System.Net.Http;
using System.Threading.Tasks;
using Lofi.API.Models.MoneroRpc;
using RpcParameters = Lofi.API.Models.MoneroRpc.Parameters;
using RpcResults = Lofi.API.Models.MoneroRpc.Results;
using System.Text.Json;
using System.Threading;

namespace Lofi.API.Services
{
    public static class MoneroWalletRpcMethod
    {
        public const string OPEN_WALLET = "open_wallet";
        public const string MAKE_INTEGRATED_ADDRESS = "make_integrated_address";
        public const string MAKE_URI = "make_uri";
        public const string GET_ACCOUNTS = "get_accounts";
        public const string GET_ADDRESS = "get_address";
    }

    public class MoneroService
    {
        private readonly string _daemonRpcUri;
        private readonly string _walletRpcUri;
        private readonly IHttpClientFactory _httpClientFactory;
        public MoneroService(IHttpClientFactory httpClientFactory, string daemonRpcUri, string walletRpcUri)
        {
            this._httpClientFactory = httpClientFactory;
            this._daemonRpcUri = daemonRpcUri;
            this._walletRpcUri = walletRpcUri;
        }

        public async Task<MoneroRpcResponse<TResult>> PerformWalletRpc<TParameters, TResult>(
                string method,
                TParameters parameters,
                string? id = null,
                string? jsonRpc = null,
                CancellationToken cancellationToken = default)
            where TResult : class
            where TParameters : class
        {
            id ??= MoneroRpcRequest<TParameters>.DEFAULT_ID;
            jsonRpc ??= MoneroRpcRequest<TParameters>.DEFAULT_JSON_RPC;
            var rpcRequest = new MoneroRpcRequest<TParameters>(method, parameters, id, jsonRpc);
            
            using var httpClient = _httpClientFactory.CreateClient();
            var httpContent = rpcRequest.AsHttpContent();
            Console.WriteLine(await rpcRequest.AsHttpContent().ReadAsStringAsync());
            using var httpResponse = await httpClient.PostAsync(_walletRpcUri, httpContent, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            httpResponse.EnsureSuccessStatusCode();

            var json = await httpResponse.Content.ReadAsStringAsync();
            Console.WriteLine(json);
            var response = JsonSerializer.Deserialize<MoneroRpcResponse<TResult>>(json);
            cancellationToken.ThrowIfCancellationRequested();
            if (response == null
                    || (response.Error == null && response.Result == null)
                    || (response.Error != null && response.Result != null))
            {
                throw new Exception("The monero deamon returned an invalid response.");
            }
            return response;
        }

        public async Task<MoneroRpcResponse<RpcResults.EmptyRpcResult>> OpenWalletAsync(RpcParameters.OpenWalletRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.OpenWalletRpcParameters, RpcResults.EmptyRpcResult>(
                MoneroWalletRpcMethod.OPEN_WALLET,
                parameters,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.MakeIntegratedAddressRpcResult>> MakeIntegratedAddressAsync(RpcParameters.MakeIntegratedAddressRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.MakeIntegratedAddressRpcParameters, RpcResults.MakeIntegratedAddressRpcResult>(
                MoneroWalletRpcMethod.MAKE_INTEGRATED_ADDRESS,
                parameters,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.MakeUriRpcResult>> MakeUri(RpcParameters.MakeUriRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.MakeUriRpcParameters, RpcResults.MakeUriRpcResult>(
                MoneroWalletRpcMethod.MAKE_URI,
                parameters,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.GetAccountsRpcResult>> GetAccounts(RpcParameters.GetAccountsRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetAccountsRpcParameters, RpcResults.GetAccountsRpcResult>(
                MoneroWalletRpcMethod.GET_ACCOUNTS,
                parameters,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.GetAddressRpcResult>> GetAddress(RpcParameters.GetAddressRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetAddressRpcParameters, RpcResults.GetAddressRpcResult>(
                MoneroWalletRpcMethod.GET_ADDRESS,
                parameters,
                cancellationToken: cancellationToken
            );
        }
    }
}