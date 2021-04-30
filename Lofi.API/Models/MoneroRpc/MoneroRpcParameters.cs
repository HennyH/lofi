using System;
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
        public MakeUriRpcParameters(string address, int? amount = null, string? recipientName = null, string? transactionDescription = null)
        {
            this.Address = address;
            this.Amount = amount;
            this.RecipientName = recipientName;
            this.TransactionDescription = transactionDescription;
        }

        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("amount")]
        public int? Amount { get; set; }
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
        public bool? In {get;set;}
        [JsonPropertyName("out")]
        public bool? Out {get;set;}
        [JsonPropertyName("pending")]
        public bool? Pending {get;set;} 
        [JsonPropertyName("failed")]
        public bool? Failed {get;set;}
        [JsonPropertyName("pool")]
        public bool? Pool {get;set;}
        [JsonPropertyName("filter_by_height")]
        public bool? FilterByHeight {get;set;}
        [JsonPropertyName("min_height")]
        public ulong? MinHeight {get;set;}
        [JsonPropertyName("max_height")]
        public ulong? MaxHeight {get;set;}
        [JsonPropertyName("account_index")]
        public ulong? AccountIndex {get;set;}
        [JsonPropertyName("subaddr_indices")]
        public ulong[]? SubaddressIndexes {get;set;}
    }
}