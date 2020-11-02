using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebSocketClient4SignalR
{
    /// <summary>
    /// SignalR return message format
    /// </summary>
    public class ReceivedMessage
    {
        [JsonProperty("M")]
        public List<Message> Messages { get; set; }
    }
}