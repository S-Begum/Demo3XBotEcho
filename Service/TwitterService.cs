using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using Tweetinvi;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;
using XBotEcho.Configurations;
using XBotEcho.Models;

namespace XBotEcho.Service
{
    public class TwitterService
    {
        private readonly IMemoryCache _cache;
        private readonly IOptions<TwitterApiSettings> _twitterApiSettings;
        private readonly TwitterClient client;
        private readonly string _bearerToken;

        public TwitterService(IMemoryCache cache, IOptions<TwitterApiSettings> twitterApiSettings)
        {
            _cache = cache;
            _twitterApiSettings = twitterApiSettings;
            client = new TwitterClient(
                _twitterApiSettings.Value.ClientId,
                _twitterApiSettings.Value.ClientSecret,
                _twitterApiSettings.Value.AccessToken,
                _twitterApiSettings.Value.AccessTokenSecret);
            _bearerToken = client.Auth.CreateBearerTokenAsync().Result;
        }

        public async Task<UserProfile> RetrieveProfile()
        {
            IAuthenticatedUser user = await client.Users.GetAuthenticatedUserAsync();
            UserProfile profile = new()
            {
                UserName = user.ToString(),
                FullName = user.Name,
                Location = user.Location,
                ImageUrl = user.ProfileImageUrl,
                Email = user.Email,
                DateCreated = user.CreatedAt,
                TimeZone = user.TimeZone
            };
            return profile;
        }

        public async Task<ITwitterResult>? PostNewTweet(PostTweet tweet)
        {
            var url = _twitterApiSettings.Value.PostUrl;
            var result = await client.Execute.AdvanceRequestAsync(BuildNewTweet(tweet, client, url));
            return result;
        }

        private static Action<ITwitterRequest> BuildNewTweet(PostTweet tweet, TwitterClient client, string url)
        {
            
            return (ITwitterRequest request) =>
            {
                var jsonBody = client.Json.Serialize(tweet);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                request.Query.Url = url;
                request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                request.Query.HttpContent = content;
            };
        }


        public async Task<string?> GetRecentTweets(string query)
        {
            using (var httpClient = new HttpClient())
            {
                var searchString = Uri.EscapeDataString(query);
                var xApiSettings = _twitterApiSettings.Value;
                var url = $"{xApiSettings.SearchUrl}{searchString}{xApiSettings.SearchFields}";
                var requestConfigurator = new Action<ITwitterRequest>(request =>
                {
                    request.Query.Url = url;
                    request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.GET;
                });

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
                _cache.Remove("SearchResult");

                ITwitterResult searchResult = await client.Execute.AdvanceRequestAsync(requestConfigurator);
                _cache.Set("SearchResult", searchResult.Content, DateTimeOffset.Now.AddSeconds(60));

                return searchResult.Content;
            }
        }
    }
}
