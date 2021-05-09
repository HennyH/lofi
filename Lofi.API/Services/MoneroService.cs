using System;
using System.Net.Http;
using System.Threading.Tasks;
using Lofi.API.Models.MoneroRpc;
using RpcParameters = Lofi.API.Models.MoneroRpc.Parameters;
using RpcResults = Lofi.API.Models.MoneroRpc.Results;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Lofi.API.Shared;
using System.Collections.Concurrent;

namespace Lofi.API.Services
{
    public static class MoneroWalletRpcMethod
    {
        public const string OPEN_WALLET = "open_wallet";
        public const string MAKE_INTEGRATED_ADDRESS = "make_integrated_address";
        public const string MAKE_URI = "make_uri";
        public const string GET_ACCOUNTS = "get_accounts";
        public const string GET_ADDRESS = "get_address";
        public const string GET_PAYMENTS = "get_payments";
        public const string GET_TRANSFERS = "get_transfers";
        public const string GET_BALANCE = "get_balance";
        public const string TRANSFER = "transfer";
        public const string SPLIT_TRANSFER = "transfer_split";
        public const string DESCRIBE_TRANSFER = "describe_transfer";
        public const string SUBMIT_TRANSFER = "submit_transfer";
        public const string GET_TRANSFER_BY_TXID = "get_transfer_by_txid";
    }

    public class MoneroService
    {
        private const string DEFAULT_MONERO_DAEMON_RPC_URI = "http://localhost:28081/json_rpc";
        private const string DEFAULT_MONERO_WALLET_RPC_URI = "http://localhost:28083/json_rpc";
        private ILogger<MoneroService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _deamonRpcUri;
        private readonly string _walletRpcUri;
        private Task _previousWalletRpc = Task.CompletedTask;
        private readonly object _walletRpcLock = new object();
        private readonly HttpClient _httpClient;


        public MoneroService(ILogger<MoneroService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this._logger = logger;
            this._httpClientFactory = httpClientFactory;
            this._httpClient = httpClientFactory.CreateClient();
            this._deamonRpcUri = configuration.GetValue<string>("MONERO_DAEMON_RPC_URI", DEFAULT_MONERO_DAEMON_RPC_URI);
            this._walletRpcUri = configuration.GetValue<string>("MONERO_WALLET_RPC_URI", DEFAULT_MONERO_WALLET_RPC_URI);
        }

        public Task<MoneroRpcResponse<TResult>> PerformWalletRpc<TParameters, TResult>(
                string method,
                TParameters parameters,
                string? id = null,
                string? jsonRpc = null,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
            where TResult : class
            where TParameters : class
        {
            id ??= MoneroRpcRequest<TParameters>.DEFAULT_ID;
            jsonRpc ??= MoneroRpcRequest<TParameters>.DEFAULT_JSON_RPC;

            lock (_walletRpcLock)
            {
                if (walletFilename != null)
                {
                    _previousWalletRpc = _previousWalletRpc.ContinueWith(
                        async (_) =>
                        {
                            var openWalletRequest = new MoneroRpcRequest<RpcParameters.OpenWalletRpcParameters>(
                                MoneroWalletRpcMethod.OPEN_WALLET,
                                parameters: new RpcParameters.OpenWalletRpcParameters(filename: walletFilename, password: walletPassword ?? string.Empty)
                            );
                            var openWalletRequestContent = openWalletRequest.AsHttpContent();
                            using var openWalletHttpResponse = await _httpClient.PostAsync(_walletRpcUri, openWalletRequestContent, cancellationToken);
                            cancellationToken.ThrowIfCancellationRequested();
                            var json = await openWalletHttpResponse.Content.ReadAsStringAsync();
                            var response = JsonSerializer.Deserialize<MoneroRpcResponse<RpcResults.EmptyRpcResult>>(json);
                            if (response == null || response.Error != null)
                            {
                                // throw new Exception("Unable to open wallet");
                            }

                            var recsan = new MoneroRpcRequest<RpcResults.EmptyRpcResult>("rescan_spent", new RpcResults.EmptyRpcResult());
                            var rescanResponse = await _httpClient.PostAsync(_walletRpcUri, recsan.AsHttpContent(), cancellationToken);
                        },
                        cancellationToken,
                        TaskContinuationOptions.None,
                        TaskScheduler.Default
                    ).Unwrap();
                }

                var task = _previousWalletRpc.ContinueWith(
                    async (_) =>
                    {
                        var rpcRequest = new MoneroRpcRequest<TParameters>(method, parameters, id, jsonRpc);
                        var rpcRequestContent = rpcRequest.AsHttpContent();
                        using var rpcHttpResponse = await _httpClient.PostAsync(_walletRpcUri, rpcRequestContent, cancellationToken);
                        cancellationToken.ThrowIfCancellationRequested();
                        rpcHttpResponse.EnsureSuccessStatusCode();

                        var json = await rpcHttpResponse.Content.ReadAsStringAsync();
                        var response = JsonSerializer.Deserialize<MoneroRpcResponse<TResult>>(json);
                        if (response == null
                                || (response.Error == null && response.Result == null)
                                || (response.Error != null && response.Result != null))
                        {
                            throw new Exception("The monero deamon returned an invalid response.");
                        }
                        return response;
                    },
                    cancellationToken,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Default
                ).Unwrap();

                _previousWalletRpc = task;
                return task;
            }
        }

        public async Task<MoneroRpcResponse<RpcResults.EmptyRpcResult>> OpenWalletAsync(
                RpcParameters.OpenWalletRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.OpenWalletRpcParameters, RpcResults.EmptyRpcResult>(
                MoneroWalletRpcMethod.OPEN_WALLET,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.MakeIntegratedAddressRpcResult>> MakeIntegratedAddressAsync(
                RpcParameters.MakeIntegratedAddressRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.MakeIntegratedAddressRpcParameters, RpcResults.MakeIntegratedAddressRpcResult>(
                MoneroWalletRpcMethod.MAKE_INTEGRATED_ADDRESS,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.MakeUriRpcResult>> MakeUri(
                RpcParameters.MakeUriRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.MakeUriRpcParameters, RpcResults.MakeUriRpcResult>(
                MoneroWalletRpcMethod.MAKE_URI,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.GetAccountsRpcResult>> GetAccounts(
                RpcParameters.GetAccountsRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetAccountsRpcParameters, RpcResults.GetAccountsRpcResult>(
                MoneroWalletRpcMethod.GET_ACCOUNTS,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.GetAddressRpcResult>> GetAddress(
                RpcParameters.GetAddressRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetAddressRpcParameters, RpcResults.GetAddressRpcResult>(
                MoneroWalletRpcMethod.GET_ADDRESS,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.GetPaymentsRpcResult>> GetPayments(
                RpcParameters.GetPaymentsRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetPaymentsRpcParameters, RpcResults.GetPaymentsRpcResult>(
                MoneroWalletRpcMethod.GET_PAYMENTS,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.GetTransfersRpcResult>> GetTransfers(
                RpcParameters.GetTransfersRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetTransfersRpcParameters, RpcResults.GetTransfersRpcResult>(
                MoneroWalletRpcMethod.GET_TRANSFERS,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.GetBalanceRpcResult>> GetBalance(
                RpcParameters.GetBalanceRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetBalanceRpcParameters, RpcResults.GetBalanceRpcResult>(
                MoneroWalletRpcMethod.GET_BALANCE,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.TransferRpcResult>> Transfer(
                RpcParameters.TransferRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.TransferRpcParameters, RpcResults.TransferRpcResult>(
                MoneroWalletRpcMethod.TRANSFER,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.SplitTransferRpcResult>> SplitTransfer(
                RpcParameters.SplitTransferRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.SplitTransferRpcParameters, RpcResults.SplitTransferRpcResult>(
                MoneroWalletRpcMethod.SPLIT_TRANSFER,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }
        
        public async Task<MoneroRpcResponse<RpcResults.DescribeTransferRpcResult>> DescribeTransfer(
                RpcParameters.DescribeTransferRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.DescribeTransferRpcParameters, RpcResults.DescribeTransferRpcResult>(
                MoneroWalletRpcMethod.DESCRIBE_TRANSFER,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.SubmitTransferRpcResult>> SubmitTransfer(
                RpcParameters.SubmitTransferRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.SubmitTransferRpcParameters, RpcResults.SubmitTransferRpcResult>(
                MoneroWalletRpcMethod.SUBMIT_TRANSFER,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.GetTransferByTransactionIdRpcResult>> GetTransferByTransactionId(
                RpcParameters.GetTransferByTransactionIdRpcParameters parameters,
                string? walletFilename = null,
                string? walletPassword = null,
                CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetTransferByTransactionIdRpcParameters, RpcResults.GetTransferByTransactionIdRpcResult>(
                MoneroWalletRpcMethod.GET_TRANSFER_BY_TXID,
                parameters,
                walletFilename: walletFilename,
                walletPassword: walletPassword,
                cancellationToken: cancellationToken
            );
        }
    }
}