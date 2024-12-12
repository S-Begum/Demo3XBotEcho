using Newtonsoft.Json;

namespace XBotEcho.Models
{
    public class PostTweet
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
    }
}
