using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lofi.API.Models.MoneroRpc.Parameters
{
    public class OpenWalletRpcParameters
    {
        public OpenWalletRpcParameters(string filename, string password)
        {
            this.Filename = filename;
            this.Password = password;
        }

        [JsonPropertyName("filename")]
        public string Filename { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class MakeIntegratedAddressRpcParameters
    {
        public MakeIntegratedAddressRpcParameters(string? standardAddress = null, ushort? paymentId = null)
        {
            this.StandardAddress = standardAddress;
            this.PaymentId = paymentId.HasValue
                ? paymentId.Value.ToString("x16")
                : default;
        }

        [JsonPropertyName("standard_address")]
        public string? StandardAddress { get; set; }
        [JsonPropertyName("payment_id")]
        public string? PaymentId { get; set; }
    }

    public class MakeUriRpcParameters
    {
        public MakeUriRpcParameters(string address, ulong? amount = null, string? recipientName = null, string? transactionDescription = null)
        {
            this.Address = address;
            this.Amount = amount;
            this.RecipientName = recipientName;
            this.TransactionDescription = transactionDescription;
        }

        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("amount")]
        public ulong? Amount { get; set; }
        [JsonPropertyName("recipient_name")]
        public string? RecipientName { get; set; }
        [JsonPropertyName("tx_description")]
        public string? TransactionDescription { get; set; }
    }

    public class GetAccountsRpcParameters
    {
        public GetAccountsRpcParameters(string? tag = null)
        {
            this.Tag = tag;
        }

        [JsonPropertyName("tag")]
        public string? Tag { get; set; }
    }

    public class GetAddressRpcParameters
    {
        public GetAddressRpcParameters(ulong accountIndex, ulong[]? addressIndex = null)
        {
            this.AccountIndex = accountIndex;
            this.AddressIndex = addressIndex;
        }

        [JsonPropertyName("account_index")]
        public ulong AccountIndex { get; set; }

        [JsonPropertyName("address_index")]
        public ulong[]? AddressIndex { get; set; }
    }

    public class GetPaymentsRpcParameters
    {
        public GetPaymentsRpcParameters(ushort paymentId)
        {
            this.PaymentId = paymentId.ToString("x16");
        }

        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; }
    }

    public class GetTransfersRpcParameters
    {
        [JsonPropertyName("in")]
        public bool? In { get; set; }
        [JsonPropertyName("out")]
        public bool? Out { get; set; }
        [JsonPropertyName("pending")]
        public bool? Pending { get; set; }
        [JsonPropertyName("failed")]
        public bool? Failed { get; set; }
        [JsonPropertyName("pool")]
        public bool? Pool { get; set; }
        [JsonPropertyName("filter_by_height")]
        public bool? FilterByHeight { get; set; }
        [JsonPropertyName("min_height")]
        public ulong? MinHeight { get; set; }
        [JsonPropertyName("max_height")]
        public ulong? MaxHeight { get; set; }
        [JsonPropertyName("account_index")]
        public ulong? AccountIndex { get; set; }
        [JsonPropertyName("subaddr_indices")]
        public ulong[]? SubaddressIndexes { get; set; }
    }

    public class GetBalanceRpcParameters
    {
        public GetBalanceRpcParameters(ulong accountIndex)
        {
            this.AccountIndex = accountIndex;
        }

        [JsonPropertyName("account_index")]
        public ulong AccountIndex { get; set; }
        [JsonPropertyName("address_indices")]
        public ulong[]? AddressIndexes { get; set; }
    }

    public class TransferRpcParameters
    {
        public TransferRpcParameters(
                IEnumerable<TransferDestination> destinations,
                ulong? accountIndex = null,
                ulong[]? subaddressIndexes = null,
                ulong? priority = null,
                ulong? ringSize = null,
                ulong? unlockTime = null,
                ushort? paymentId = null,
                bool? getTransactionKey = null,
                bool? doNotRelay = null,
                bool? getTransactionHex = null,
                bool? getTransactionMetadata = null
            )
        {
            this.Destinations = destinations;
            this.AccountIndex = accountIndex;
            this.SubaddressIndexes = subaddressIndexes;
            this.Priority = priority;
            this.RingSize = ringSize;
            this.UnlockTime = unlockTime;
            this.PaymentId = paymentId?.ToString("x16");
            this.GetTransactionKey = getTransactionKey;
            this.GetTransactionHex = getTransactionHex;
            this.GetTransactionMetadata = getTransactionMetadata;
        }

        [JsonPropertyName("destinations")]
        public IEnumerable<TransferDestination> Destinations { get; set; }
        [JsonPropertyName("account_index")]
        public ulong? AccountIndex { get; set; }
        [JsonPropertyName("priority")]
        public ulong? Priority { get; set; }
        [JsonPropertyName("ring_size")]
        public ulong? RingSize { get; set; }
        [JsonPropertyName("unlock_time")]
        public ulong? UnlockTime { get; set; }
        [JsonPropertyName("payment_id")]
        public string? PaymentId { get; set; }
        [JsonPropertyName("get_tx_key")]
        public bool? GetTransactionKey { get; set; }
        [JsonPropertyName("do_not_relay")]
        public bool? DoNotRelay { get; set; }
        [JsonPropertyName("get_tx_hex")]
        public bool? GetTransactionHex { get; set; }
        [JsonPropertyName("get_tx_metadata")]
        public bool? GetTransactionMetadata { get; set; }
        [JsonPropertyName("subaddr_indices")]
        public ulong[]? SubaddressIndexes { get; set; }
        public class TransferDestination
        {
            public TransferDestination(ulong amount, string address)
            {
                this.Amount = amount;
                this.Address = address;
            }

            [JsonPropertyName("amount")]
            public ulong Amount { get; set; }
            [JsonPropertyName("address")]
            public string Address { get; set; }
        }
    }

    public class SplitTransferRpcParameters : TransferRpcParameters
    {
        public SplitTransferRpcParameters(
                IEnumerable<TransferDestination> destinations,
                ulong? accountIndex = null,
                ulong[]? subaddressIndexes = null,
                ulong? priority = null,
                ulong? ringSize = null,
                ulong? unlockTime = null,
                ushort? paymentId = null,
                bool? getTransactionKey = null,
                bool? doNotRelay = null,
                bool? getTransactionHex = null,
                bool? getTransactionMetadata = null
            ) : base(
                destinations,
                accountIndex,
                subaddressIndexes,
                priority,
                ringSize,
                unlockTime,
                paymentId,
                getTransactionKey,
                doNotRelay,
                getTransactionHex,
                getTransactionMetadata
            )
        { }
    }

    public class DescribeTransferRpcParameters
    {
        public DescribeTransferRpcParameters(string? unsignedTransactionSet, string? multiSignatureTransactionSet)
        {
            this.UnsignedTransactionSet = unsignedTransactionSet;
            this.MultiSignatureTransactionSet = multiSignatureTransactionSet;
        }

        public string? UnsignedTransactionSet { get; set; }
        public string? MultiSignatureTransactionSet { get; set; }
    }

    public class SubmitTransferRpcParameters
    {
        public SubmitTransferRpcParameters(string transactionDataHex)
        {
            this.TransactionDataHex = transactionDataHex;
        }

        [JsonPropertyName("tx_data_hex")]
        public string TransactionDataHex { get; set; }
    }

    public class GetTransferByTransactionIdRpcParameters
    {
        public GetTransferByTransactionIdRpcParameters(string transactionId, ulong? accountIndex = null)
        {
            this.TransactionId = transactionId;
            this.AccountIndex = accountIndex;
        }

        [JsonPropertyName("txid")]
        public string TransactionId {get;set;}
        [JsonPropertyName("account_index")]
        public ulong? AccountIndex {get;set;}
    }
}