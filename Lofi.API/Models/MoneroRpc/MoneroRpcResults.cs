using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lofi.API.Models.MoneroRpc.Results
{
    public class EmptyRpcResult
    { }

    public class MakeIntegratedAddressRpcResult
    {
        public MakeIntegratedAddressRpcResult(string integratedAddress, string paymentId)
        {
            this.IntegratedAddress = integratedAddress;
            this.PaymentId = paymentId;
        }

        [JsonPropertyName("integrated_address")]
        public string IntegratedAddress {get;set;}
        [JsonPropertyName("payment_id")]
        public string PaymentId {get;set;}
    }

    public class MakeUriRpcResult
    {
        public MakeUriRpcResult(string uri)
        {
            this.Uri = uri;
        }

        [JsonPropertyName("uri")]
        public string Uri {get;set;}
    }
    
    public class GetAccountsRpcResult
    {
        public GetAccountsRpcResult(
                IEnumerable<SubaddressAccount> subaddressAccounts,
                uint totalBalance,
                uint totalUnlockedBalance
            )
        {
            this.SubaddressAccounts = subaddressAccounts;
            this.TotalBalance = totalBalance;
            this.TotalUnlockedBalance = totalUnlockedBalance; 
        }

        [JsonPropertyName("subaddress_accounts ")]
        public IEnumerable<SubaddressAccount> SubaddressAccounts {get;set;}
        [JsonPropertyName("total_balance")]
        public uint TotalBalance {get;set;}
        [JsonPropertyName("total_unlocked_balance ")]
        public uint TotalUnlockedBalance {get;set;}

        public class SubaddressAccount
        {
            public SubaddressAccount(
                    uint accountIndex,
                    uint balance,
                    string baseAddress,
                    uint unlockedBalance,
                    string? label = null,
                    string? tag = null
                )
            {
                this.AccountIndex = accountIndex;
                this.Balance = balance;
                this.BaseAddress = baseAddress;
                this.UnlockedBalance = unlockedBalance;
                this.Label = label;
                this.Tag = tag;
            }

            [JsonPropertyName("account_index")]
            public uint AccountIndex {get;set;}
            [JsonPropertyName("balance")]
            public uint Balance {get;set;}
            [JsonPropertyName("base_address")]
            public string BaseAddress {get;set;}
            [JsonPropertyName("label")]
            public string? Label {get;set;}
            [JsonPropertyName("tag")]
            public string? Tag {get;set;}
            [JsonPropertyName("unlocked_balance")]
            public uint UnlockedBalance {get;set;}
        }
    }

    public class GetAddressRpcResult
    {
        public GetAddressRpcResult(string address, IEnumerable<Subaddress> addresses)
        {
            this.Address = address;
            this.Addresses =addresses;
        }

        [JsonPropertyName("address")]
        public string Address {get;set;}
        [JsonPropertyName("addresses")]
        public IEnumerable<Subaddress> Addresses {get;set;}
        public class Subaddress
        {
            public Subaddress(string address, string label, uint addressIndex, bool used)
            {
                this.Address = address;
                this.Label = label;
                this.AddressIndex = addressIndex;
                this.Used = used;
            }

            [JsonPropertyName("address")]
            public string Address {get;set;}
            [JsonPropertyName("label")]
            public string Label {get;set;}
            [JsonPropertyName("address_index")]
            public uint AddressIndex {get;set;}
            [JsonPropertyName("used")]
            public bool Used {get;set;}

        }
    }
}