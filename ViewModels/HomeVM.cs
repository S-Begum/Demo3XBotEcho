using System.Text.Json;
using Tweetinvi.Core.Models;
using XBotEcho.Models;

namespace XBotEcho.ViewModels
{
    
    public class HomeVM
    {
        private List<TweetWithUserInfo> _searchResult = [];

        public UserProfile? User { get; set; }
        public PostTweet? Tweet { get; set; }
        public TwitterResponse? TweetSearchResult { get; set; }
        public List<TweetWithUserInfo>? SearchResult { get; set; }        
        public string? KeyWord { get; set; }
        public string? Query { get; set; }        
        public string SuccessMsg { get; set; } = string.Empty;
        public string ErrorMsg { get; set; } = string.Empty;
    }
}
