using Newtonsoft.Json;

namespace WebSocketClient4SignalR
{
    /// <summary>
    /// Negotiation args
    /// </summary>
    public class ConnectionData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}