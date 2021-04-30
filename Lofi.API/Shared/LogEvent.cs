using Microsoft.Extensions.Logging;

namespace Lofi.API.Shared
{
    public static class LogEvent
    {  
        public static readonly EventId MONERO_RPC_REQUEST = new EventId(1, "MONERO_RPC_REQUEST");
        public static readonly EventId MONERO_RPC_RESPONSE = new EventId(2, "MONERO_RPC_RESPOSNE");
        public static readonly EventId TIP_PAYMENT_CONF = new EventId(3, "TIP_PAYMENT_CONFIRMED");
    }
}