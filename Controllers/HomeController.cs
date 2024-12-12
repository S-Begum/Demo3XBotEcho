using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Text.Json;
using Tweetinvi.Core.Web;
using Tweetinvi.Exceptions;
using XBotEcho.Models;
using XBotEcho.Service;
using XBotEcho.ViewModels;

namespace XBotEcho.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TwitterService _twitterService;
        private readonly IMemoryCache _cache;

        public HomeController(ILogger<HomeController> logger, TwitterService twitterService, IMemoryCache cache)
        {
            _logger = logger;
            _twitterService = twitterService;
            _cache = cache;
        }

        public async Task<IActionResult> Index(HomeVM vm)
        {            
            try
            {
                if (vm.KeyWord == "profile")
                {
                    vm.User = await _twitterService.RetrieveProfile();
                    _logger.LogInformation($"User profile for '{vm.User.FullName}' retrieved successfully.");
                }
                if (_cache.TryGetValue("SearchResult", out string? response))
                {
                    vm.TweetSearchResult = JsonSerializer.Deserialize<TwitterResponse>(response);
                    if (vm.TweetSearchResult != null)
                    {
                        vm.SearchResult = vm.TweetSearchResult.Data
                                .Join(
                                        vm.TweetSearchResult.Includes.Users,
                                        tweet => tweet.AuthorId,
                                        user => user.Id,
                                        (tweet, user) => new TweetWithUserInfo
                                        {
                                            Id = tweet.Id,
                                            AuthorId = tweet.AuthorId,
                                            EditHistoryTweetIds = tweet.EditHistoryTweetIds,
                                            PublicMetrics = tweet.PublicMetrics,
                                            Text = tweet.Text,
                                            Author = user
                                        }).ToList();
                    }
                    _logger.LogInformation($"{vm.TweetSearchResult?.Meta.ResultCount} " +
                    $"tweets retrieved successfully. Time: {DateTime.Now:g}. Query: {vm.Query}. Content: {response}");
                    vm.SuccessMsg = $"{vm.TweetSearchResult?.Meta.ResultCount} tweets retrieved successfully";
                }
            }
            catch (TwitterException tex)
            {
                _logger.LogError($"Twitter API error: {tex.TwitterDescription}");
                vm.ErrorMsg = $"Twitter API error: {tex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error: {ex.Message}");
                vm.ErrorMsg = $"Error: {ex.Message}";
            }
            return View(vm);
        }

        // for free twitter api, wait 15-20 minutes before a new request (post/get) 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetNewTweets(string query)
        {
            HomeVM model = new();
            try
            {
                await _twitterService.GetRecentTweets(query);
            }
            catch (TwitterException tex)
            {
                _logger.LogError($"Twitter API error: {tex.TwitterDescription}.  Error message: {tex.Message}");
                model.ErrorMsg = $"Twitter API error: {tex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError("General error: " + ex.Message);
                model.ErrorMsg = $"Error: {ex.Message}";
            }
            return RedirectToAction(nameof(Index), new { Query = query, model.ErrorMsg });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendNewTweet(PostTweet tweet)
        {
            HomeVM vm = new();
            try
            {                
                ITwitterResult result = await _twitterService.PostNewTweet(tweet);
                _logger.LogInformation($"{result.Response.StatusCode}: Tweet posted successfully");
                vm.SuccessMsg = $"{result.Response.StatusCode}: Tweet posted successfully";
            }
            catch (TwitterException tex)
            {
                _logger.LogError($"Twitter API error: {tex.TwitterDescription}. Error message: {tex.Message}");
                vm.ErrorMsg = $"Twitter API error: {tex.Message}.";
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error: {ex.Message}");
                vm.ErrorMsg = $"Error: {ex.Message}";
            }
            return View(nameof(Index), new {vm.SuccessMsg, vm.ErrorMsg });
        }

        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
