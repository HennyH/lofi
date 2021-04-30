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
        public string IntegratedAddress { get; set; }
        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; }
    }

    public class MakeUriRpcResult
    {
        public MakeUriRpcResult(string uri)
        {
            this.Uri = uri;
        }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

    public class GetAccountsRpcResult
    {
        public GetAccountsRpcResult(
                IEnumerable<SubaddressAccount> subaddressAccounts,
                ulong totalBalance,
                ulong totalUnlockedBalance
            )
        {
            this.SubaddressAccounts = subaddressAccounts;
            this.TotalBalance = totalBalance;
            this.TotalUnlockedBalance = totalUnlockedBalance;
        }

        [JsonPropertyName("subaddress_accounts ")]
        public IEnumerable<SubaddressAccount> SubaddressAccounts { get; set; }
        [JsonPropertyName("total_balance")]
        public ulong TotalBalance { get; set; }
        [JsonPropertyName("total_unlocked_balance ")]
        public ulong TotalUnlockedBalance { get; set; }

        public class SubaddressAccount
        {
            public SubaddressAccount(
                    ulong accountIndex,
                    ulong balance,
                    string baseAddress,
                    ulong unlockedBalance,
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
            public ulong AccountIndex { get; set; }
            [JsonPropertyName("balance")]
            public ulong Balance { get; set; }
            [JsonPropertyName("base_address")]
            public string BaseAddress { get; set; }
            [JsonPropertyName("label")]
            public string? Label { get; set; }
            [JsonPropertyName("tag")]
            public string? Tag { get; set; }
            [JsonPropertyName("unlocked_balance")]
            public ulong UnlockedBalance { get; set; }
        }
    }

    public class GetAddressRpcResult
    {
        public GetAddressRpcResult(string address, IEnumerable<Subaddress> addresses)
        {
            this.Address = address;
            this.Addresses = addresses;
        }

        [JsonPropertyName("address")]
        public string Address { get; set; }
        [JsonPropertyName("addresses")]
        public IEnumerable<Subaddress> Addresses { get; set; }
        public class Subaddress
        {
            public Subaddress(string address, string label, ulong addressIndex, bool used)
            {
                this.Address = address;
                this.Label = label;
                this.AddressIndex = addressIndex;
                this.Used = used;
            }

            [JsonPropertyName("address")]
            public string Address { get; set; }
            [JsonPropertyName("label")]
            public string Label { get; set; }
            [JsonPropertyName("address_index")]
            public ulong AddressIndex { get; set; }
            [JsonPropertyName("used")]
            public bool Used { get; set; }

        }
    }

    public class PaymentSubaddressIndexRpcResult
        {
            public PaymentSubaddressIndexRpcResult(ulong major, ulong minor)
            {
                this.Major = major;
                this.Minor = minor;
            }

            [JsonPropertyName("major")]
            public ulong Major { get; set; }
            [JsonPropertyName("minor")]
            public ulong Minor { get; set; }
        }

    public class GetPaymentsRpcResult
    {
        public GetPaymentsRpcResult(IEnumerable<Payment> payments)
        {
            this.Payments = payments;
        }

        [JsonPropertyName("payments")]
        public IEnumerable<Payment> Payments { get; set; }

        public class Payment
        {
            public Payment(
                    string paymentId,
                    string transactionHash,
                    ulong blockHeight,
                    ulong unlockTime,
                    PaymentSubaddressIndexRpcResult subaddressIndex,
                    string address
                )
            {
                this.PaymentId = paymentId;
                this.TransactionHash = transactionHash;
                this.BlockHeight = blockHeight;
                this.UnlockTime = unlockTime;
                this.SubaddressIndex = subaddressIndex;
                this.Address = address;
            }

            [JsonPropertyName("payment_id")]
            public string PaymentId { get; set; }
            [JsonPropertyName("tx_hash")]
            public string TransactionHash { get; set; }
            [JsonPropertyName("block_height")]
            public ulong BlockHeight { get; set; }
            [JsonPropertyName("unlock_time")]
            public ulong UnlockTime { get; set; }
            [JsonPropertyName("subaddr_index")]
            public PaymentSubaddressIndexRpcResult SubaddressIndex { get; set; }
            [JsonPropertyName("address")]
            public string Address { get; set; }
        }
    }

    public class GetTransfersRpcResult
    {
        public GetTransfersRpcResult(
                IEnumerable<Transfer> inTransfers,
                IEnumerable<Transfer> outTransfers,
                IEnumerable<Transfer> pendingTransfers,
                IEnumerable<Transfer> failedTransfers,
                IEnumerable<Transfer> poolTransfers
            )
        {
            this.InTransfers = inTransfers;
            this.OutTransfers = outTransfers;
            this.PendingTransfers =pendingTransfers;
            this.FailedTransfers = failedTransfers;
            this.PoolTransfers = poolTransfers;
        }

        [JsonPropertyName("in")]
        public IEnumerable<Transfer> InTransfers { get; set; }
        [JsonPropertyName("out")]
        public IEnumerable<Transfer> OutTransfers { get; set; }
        [JsonPropertyName("pending")]
        public IEnumerable<Transfer> PendingTransfers { get; set; }
        [JsonPropertyName("failed")]
        public IEnumerable<Transfer> FailedTransfers { get; set; }
        [JsonPropertyName("pool")]
        public IEnumerable<Transfer> PoolTransfers { get; set; }

        public class Transfer
        {
            public Transfer(
                    string address,
                    ulong amount,
                    ulong confirmations,
                    bool doubleSpendSeen,
                    ulong fee,
                    ulong height,
                    string note,
                    string paymentId,
                    PaymentSubaddressIndexRpcResult subaddressIndex,
                    ulong suggestedConfirmationsThreshold,
                    ulong timestamp,
                    string transactionId,
                    string transferType,
                    ulong unlockTime
                )
            {
                this.Address = address;
                this.Amount = amount;
                this.Confirmations = confirmations;
                this.DoubleSpendSeen = doubleSpendSeen;
                this.Fee = fee;
                this.Height = height;
                this.Note = note;
                this.PaymentId = paymentId;
                this.SubaddressIndex = subaddressIndex;
                this.SuggestedConfirmationsThreshold = suggestedConfirmationsThreshold;
                this.Timestamp = timestamp;
                this.TransactionId = transactionId;
                this.TransferType = transferType;
                this.UnlockTime = unlockTime;
            }

            [JsonPropertyName("address")]
            public string Address { get; set; }
            [JsonPropertyName("amount")]
            public ulong Amount { get; set; }
            [JsonPropertyName("confirmations")]
            public ulong Confirmations { get; set; }
            [JsonPropertyName("double_spend_seen")]
            public bool DoubleSpendSeen { get; set; }
            [JsonPropertyName("fee")]
            public ulong Fee { get; set; }
            [JsonPropertyName("height")]
            public ulong Height { get; set; }
            [JsonPropertyName("note")]
            public string Note { get; set; }
            [JsonPropertyName("payment_id")]
            public string PaymentId { get; set; }
            [JsonPropertyName("subaddr_index")]
            public PaymentSubaddressIndexRpcResult SubaddressIndex { get; set; }
            [JsonPropertyName("suggested_confirmations_threshold")]
            public ulong SuggestedConfirmationsThreshold { get; set; }
            [JsonPropertyName("timestamp")]
            public ulong Timestamp { get; set; }
            [JsonPropertyName("txid")]
            public string TransactionId { get; set; }
            [JsonPropertyName("type")]
            public string TransferType { get; set; }
            [JsonPropertyName("unlock_time")]
            public ulong UnlockTime { get; set; }
        }
    }
}