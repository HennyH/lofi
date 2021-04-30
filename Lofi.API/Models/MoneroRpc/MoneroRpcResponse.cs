using System;
using System.Text.Json.Serialization;
using Lofi.API.Models.MoneroRpc.Results;

namespace Lofi.API.Models.MoneroRpc
{
    public class MoneroRpcResponse<TResult>
        where TResult : class
    {
        public MoneroRpcResponse(string id, string jsonrpc, TResult result)
        {
            this.Id = id;
            this.JsonRpc = jsonrpc;
            this.Result = result;

            // if (this.Result == null && typeof(TResult) == typeof(EmptyRpcResult))
            // {
            //     this.Result = new EmptyRpcResult() as TResult;
            // }
            
            // if (this.Result == null) throw new ArgumentNullException(nameof(result), "Cannot be null unless the TResult is EmptyRpcResult");
        }

        [JsonPropertyName("error")]
        public MoneroRpcError? Error { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; }
        [JsonPropertyName("result")]
        public TResult Result { get; set; }
    }

    public class MoneroRpcError
    {
        public MoneroRpcError(int code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}