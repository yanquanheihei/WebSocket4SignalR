using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebSocketClient4SignalR
{
    /// <summary>
    /// Message format
    /// </summary>
    public class Message
    {
        [JsonProperty("H")]
        public string HubName { get; set; }

        [JsonProperty("M")]
        public string Method { get; set; }

        [JsonProperty("A")]
        public List<object> Values { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}