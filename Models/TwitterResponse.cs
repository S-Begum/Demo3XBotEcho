using System.Text.Json.Serialization;

namespace XBotEcho.Models
{
    public class PublicMetrics
    {
        [JsonPropertyName("retweet_count")]
        public int RetweetCount { get; set; }

        [JsonPropertyName("reply_count")]
        public int ReplyCount { get; set; }

        [JsonPropertyName("like_count")]
        public int LikeCount { get; set; }

        [JsonPropertyName("quote_count")]
        public int QuoteCount { get; set; }

        [JsonPropertyName("bookmark_count")]
        public int BookmarkCount { get; set; }

        [JsonPropertyName("impression_count")]
        public int ImpressionCount { get; set; }
    }

    public class TweetData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("author_id")]
        public string AuthorId { get; set; }

        [JsonPropertyName("edit_history_tweet_ids")]
        public List<string> EditHistoryTweetIds { get; set; }

        [JsonPropertyName("public_metrics")]
        public PublicMetrics PublicMetrics { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class UserData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class MetaData
    {
        [JsonPropertyName("newest_id")]
        public string NewestId { get; set; }

        [JsonPropertyName("oldest_id")]
        public string OldestId { get; set; }

        [JsonPropertyName("result_count")]
        public int ResultCount { get; set; }

        [JsonPropertyName("next_token")]
        public string NextToken { get; set; }
    }

    public class Includes
    {
        [JsonPropertyName("users")]
        public List<UserData> Users { get; set; }
    }

    public class TwitterResponse
    {
        [JsonPropertyName("data")]
        public List<TweetData> Data { get; set; }

        [JsonPropertyName("includes")]
        public Includes Includes { get; set; }

        [JsonPropertyName("meta")]
        public MetaData Meta { get; set; }
    }

    public class TweetWithUserInfo
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public List<string> EditHistoryTweetIds { get; set; }
        public PublicMetrics PublicMetrics { get; set; }
        public string Text { get; set; }
        public UserData Author { get; set; }
    }

}
