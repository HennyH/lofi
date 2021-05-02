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
        public MoneroService(ILogger<MoneroService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this._logger = logger;
            this._httpClientFactory = httpClientFactory;
            this._deamonRpcUri = configuration.GetValue<string>("MONERO_DAEMON_RPC_URI", DEFAULT_MONERO_DAEMON_RPC_URI);
            this._walletRpcUri = configuration.GetValue<string>("MONERO_WALLET_RPC_URI", DEFAULT_MONERO_WALLET_RPC_URI);
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
            _logger.LogInformation(LogEvent.MONERO_RPC_REQUEST, await httpContent.ReadAsStringAsync());
            using var httpResponse = await httpClient.PostAsync(_walletRpcUri, httpContent, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            httpResponse.EnsureSuccessStatusCode();

            var json = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogInformation(LogEvent.MONERO_RPC_RESPONSE, json);
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

        public async Task<MoneroRpcResponse<RpcResults.GetPaymentsRpcResult>> GetPayments(RpcParameters.GetPaymentsRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetPaymentsRpcParameters, RpcResults.GetPaymentsRpcResult>(
                MoneroWalletRpcMethod.GET_PAYMENTS,
                parameters,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.GetTransfersRpcResult>> GetTransfers(RpcParameters.GetTransfersRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetTransfersRpcParameters, RpcResults.GetTransfersRpcResult>(
                MoneroWalletRpcMethod.GET_TRANSFERS,
                parameters,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.GetBalanceRpcResult>> GetBalance(RpcParameters.GetBalanceRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetBalanceRpcParameters, RpcResults.GetBalanceRpcResult>(
                MoneroWalletRpcMethod.GET_BALANCE,
                parameters,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.TransferRpcResult>> Transfer(RpcParameters.TransferRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.TransferRpcParameters, RpcResults.TransferRpcResult>(
                MoneroWalletRpcMethod.TRANSFER,
                parameters,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.SplitTransferRpcResult>> SplitTransfer(RpcParameters.SplitTransferRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.SplitTransferRpcParameters, RpcResults.SplitTransferRpcResult>(
                MoneroWalletRpcMethod.SPLIT_TRANSFER,
                parameters,
                cancellationToken: cancellationToken
            );
        }
        
        public async Task<MoneroRpcResponse<RpcResults.DescribeTransferRpcResult>> DescribeTransfer(RpcParameters.DescribeTransferRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.DescribeTransferRpcParameters, RpcResults.DescribeTransferRpcResult>(
                MoneroWalletRpcMethod.DESCRIBE_TRANSFER,
                parameters,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.SubmitTransferRpcResult>> SubmitTransfer(RpcParameters.SubmitTransferRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.SubmitTransferRpcParameters, RpcResults.SubmitTransferRpcResult>(
                MoneroWalletRpcMethod.SUBMIT_TRANSFER,
                parameters,
                cancellationToken: cancellationToken
            );
        }

        public async Task<MoneroRpcResponse<RpcResults.GetTransferByTransactionIdRpcResult>> GetTransferByTransactionId(RpcParameters.GetTransferByTransactionIdRpcParameters parameters, CancellationToken cancellationToken = default)
        {
            return await PerformWalletRpc<RpcParameters.GetTransferByTransactionIdRpcParameters, RpcResults.GetTransferByTransactionIdRpcResult>(
                MoneroWalletRpcMethod.GET_TRANSFER_BY_TXID,
                parameters,
                cancellationToken: cancellationToken
            );
        }
    }
}