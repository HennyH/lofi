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
            this.PendingTransfers = pendingTransfers;
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

    public class GetBalanceRpcResult
    {
        public GetBalanceRpcResult(
                ulong balance,
                ulong unlockedBalance,
                bool multiSignatureImportNeeded,
                IEnumerable<SubaddressBalance> subaddressBalances
            )
        {
            this.Balance = balance;
            this.UnlockedBalance = unlockedBalance;
            this.MultiSignatureImportNeeded = multiSignatureImportNeeded;
            this.SubaddressBalances = subaddressBalances;
        }

        [JsonPropertyName("balance")]
        public ulong Balance { get; set; }
        [JsonPropertyName("unlocked_balance")]
        public ulong UnlockedBalance { get; set; }
        [JsonPropertyName("multisig_import_needed")]
        public bool MultiSignatureImportNeeded { get; set; }
        [JsonPropertyName("per_subaddress")]
        public IEnumerable<SubaddressBalance> SubaddressBalances { get; set; }

        public class SubaddressBalance
        {
            public SubaddressBalance(
                    ulong addressIndex,
                    string address,
                    ulong balance,
                    ulong unlockedBalance,
                    string label,
                    ulong numberOfUnspentOutputs
                )
            {
                this.AddressIndex = addressIndex;
                this.Address = address;
                this.Balance = balance;
                this.UnlockedBalance = unlockedBalance;
                this.Label = label;
                this.NumberOfUnspentOutputs = numberOfUnspentOutputs;
            }

            [JsonPropertyName("address_index")]
            public ulong AddressIndex { get; set; }
            [JsonPropertyName("address")]
            public string Address { get; set; }
            [JsonPropertyName("balance")]
            public ulong Balance { get; set; }
            [JsonPropertyName("unlocked_balance")]
            public ulong UnlockedBalance { get; set; }
            [JsonPropertyName("label")]
            public string Label { get; set; }
            [JsonPropertyName("num_unspent_outputs")]
            public ulong NumberOfUnspentOutputs { get; set; }
        }
    }

    public class TransferRpcResult
    {
        public TransferRpcResult(
                string transactionHash,
                string transactionKey,
                ulong amount,
                ulong fee,
                ulong weight,
                string transactionBlob,
                string transactionMetadata,
                string multiSignatureTransactionSet,
                string unsignedTransactionSet
            )
        {
            this.TransactionHash = transactionHash;
            this.TransactionKey = transactionKey;
            this.Amount = amount;
            this.Fee = fee;
            this.Weight = weight;
            this.TransactionBlob = transactionBlob;
            this.TransactionMetadata = transactionMetadata;
            this.MultiSignatureTransactionSet = multiSignatureTransactionSet;
            this.UnsignedTransactionSet = unsignedTransactionSet;
        }

        [JsonPropertyName("tx_hash")]
        public string TransactionHash { get; set; }
        [JsonPropertyName("tx_key")]
        public string TransactionKey { get; set; }
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }
        [JsonPropertyName("fee")]
        public ulong Fee { get; set; }
        [JsonPropertyName("weight")]
        public ulong Weight { get; set; }
        [JsonPropertyName("tx_blob")]
        public string TransactionBlob { get; set; }
        [JsonPropertyName("tx_metadata")]
        public string TransactionMetadata { get; set; }
        [JsonPropertyName("multisig_txset")]
        public string MultiSignatureTransactionSet { get; set; }
        [JsonPropertyName("unsigned_txset")]
        public string UnsignedTransactionSet { get; set; }
    }

    public class SplitTransferRpcResult
    {
        public SplitTransferRpcResult(
                IEnumerable<string> transactionHashes,
                IEnumerable<string> transactionKeys,
                IEnumerable<ulong> amounts,
                IEnumerable<ulong> weights,
                IEnumerable<ulong> fees,
                IEnumerable<string> transactionBlobs,
                IEnumerable<string> transactionMetadatas,
                string multiSignatureTransactionSet,
                string unsignedTransactionSet
            )
        {
            this.TransactionHashes = transactionHashes;
            this.TransactionKeys = transactionKeys;
            this.Amounts = amounts;
            this.Weights = weights;
            this.Fees = fees;
            this.TransactionBlobs = transactionBlobs;
            this.TransactionMetadatas = transactionMetadatas;
            this.MultiSignatureTransactionSet = multiSignatureTransactionSet;
            this.UnsignedTransactionSet = unsignedTransactionSet;
        }

        [JsonPropertyName("tx_hash_list")]
        public IEnumerable<string> TransactionHashes { get; set; }
        [JsonPropertyName("tx_key_list")]
        public IEnumerable<string> TransactionKeys { get; set; }
        [JsonPropertyName("amount_list")]
        public IEnumerable<ulong> Amounts { get; set; }
        [JsonPropertyName("fee_list")]
        public IEnumerable<ulong> Fees { get; set; }
        [JsonPropertyName("weight_list")]
        public IEnumerable<ulong> Weights { get; set; }
        [JsonPropertyName("tx_blob_list")]
        public IEnumerable<string> TransactionBlobs { get; set; }
        [JsonPropertyName("tx_metadata_list")]
        public IEnumerable<string> TransactionMetadatas { get; set; }
        [JsonPropertyName("multisig_txset")]
        public string MultiSignatureTransactionSet { get; set; }
        [JsonPropertyName("unsigned_txset")]
        public string UnsignedTransactionSet { get; set; }
    }

    public class DescribeTransferRpcResult
    {
        public DescribeTransferRpcResult(IEnumerable<TransferDescription> transferDescriptions)
        {
            this.TransferDescriptions = transferDescriptions;
        }

        [JsonPropertyName("desc")]
        public IEnumerable<TransferDescription> TransferDescriptions { get; set; }

        public class TransferDescription
        {
            public TransferDescription(
                    ulong amountIn,
                    ulong amountOut,
                    IEnumerable<Recipient> recipients,
                    string changeAddress,
                    ulong changeAmount,
                    ulong fee,
                    string paymentId,
                    ulong unlockTime,
                    ulong dummyOutputs,
                    string extra
                )
            {
                this.AmountIn = amountIn;
                this.AmountOut = amountOut;
                this.Recipients = recipients;
                this.ChangeAddress = changeAddress;
                this.ChangeAmount = changeAmount;
                this.Fee = fee;
                this.PaymentId = paymentId;
                this.UnlockTime = unlockTime;
                this.DummyOutputs = dummyOutputs;
                this.Extra = extra;
            }

            [JsonPropertyName("amount_in")]
            public ulong AmountIn { get; set; }
            [JsonPropertyName("amount_out")]
            public ulong AmountOut { get; set; }
            [JsonPropertyName("recipients")]
            public IEnumerable<Recipient> Recipients { get; set; }
            [JsonPropertyName("change_address")]
            public string ChangeAddress { get; set; }
            [JsonPropertyName("change_amount")]
            public ulong ChangeAmount { get; set; }
            [JsonPropertyName("fee")]
            public ulong Fee { get; set; }
            [JsonPropertyName("payment_id")]
            public string PaymentId { get; set; }
            [JsonPropertyName("unlock_time")]
            public ulong UnlockTime { get; set; }
            [JsonPropertyName("dummy_outputs")]
            public ulong DummyOutputs { get; set; }
            [JsonPropertyName("extra")]
            public string Extra { get; set; }
        }

        public class Recipient
        {
            public Recipient(string address, ulong amount)
            {
                this.Address = address;
                this.Amount = amount;
            }

            [JsonPropertyName("address")]
            public string Address { get; set; }
            [JsonPropertyName("amount")]
            public ulong Amount { get; set; }
        }
    }

    public class SubmitTransferRpcResult
    {
        public SubmitTransferRpcResult(IEnumerable<string> tansactionHashList)
        {
            this.TransactionHashList = tansactionHashList;
        }

        [JsonPropertyName("tx_hash_list")]
        public IEnumerable<string> TransactionHashList { get; set; }
    }
}