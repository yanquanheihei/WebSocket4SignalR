using Newtonsoft.Json;

namespace TestType
{
    public class TestMessage
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("length")]
        public int MessageLength { get { return Message.Length; } }

        [JsonProperty("message")]
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("Code：{0} Length：{1} Content：{2}", Code, MessageLength, Message);
        }
    }
}