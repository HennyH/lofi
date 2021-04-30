using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lofi.API.Models.MoneroRpc
{
    public class MoneroRpcRequest<TParams>
    {
        public const string DEFAULT_ID = "0";
        public const string DEFAULT_JSON_RPC = "2.0";
        public MoneroRpcRequest(string method, TParams parameters, string id = DEFAULT_ID, string jsonRpc = DEFAULT_JSON_RPC)
        {
            this.JsonRpc = jsonRpc;
            this.Id = id;
            this.Method = method;
            this.Params = parameters;
        }
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc {get;set;} = "2.0";
        [JsonPropertyName("id")]
        public string Id {get;set;} = "0";
        [JsonPropertyName("method")]
        public string Method {get;set;}
        [JsonPropertyName("params")]
        public TParams Params {get;set;}

        public StringContent AsHttpContent()
        {
            return new StringContent(
                JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }),
                Encoding.UTF8,
                MediaTypeNames.Application.Json
            );
        }
    }
}