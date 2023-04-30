using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineVideoStreamingApp.Models;
using System.Diagnostics;
using System.Net.Http;


namespace OnlineVideoStreamingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
    

        

    

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<List<YoutubeVideoModel>> getYoutubeApiData()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDHrJE1pYF2eGs7wJkiiB3bMiyhE238aKU",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = "coding"; // Replace with your search term.
            searchListRequest.MaxResults = 10;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<YoutubeVideoModel> videos = new List<YoutubeVideoModel>();
            
            foreach (var searchResult in searchListResponse.Items)
            {
                Console.WriteLine(searchResult.Id.VideoId);
                YoutubeVideoModel vidModel = new YoutubeVideoModel();
                vidModel.VideoId = searchResult.Id.VideoId;
                vidModel.PlayListId = searchResult.Id.PlaylistId;
                vidModel.ChannelTitle = searchResult.Snippet.ChannelTitle;
                vidModel.Title = searchResult.Snippet.Title;
                vidModel.Description = searchResult.Snippet.Description;
                vidModel.PublishedAt = (DateTime)searchResult.Snippet.PublishedAt;
                videos.Add(vidModel);
                
            }

            Console.WriteLine(videos.Count);
            return videos;
        }
        public async Task<IActionResult> Search()
        {


            List<YoutubeVideoModel> videos = await getYoutubeApiData();

            foreach(var video in videos) {
                Console.WriteLine("Value of video id " + video.VideoId);
            }
            ViewData["Videos"] = videos;
            return View(videos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}