using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineVideoStreamingApp.Areas.Identity.Data;
using OnlineVideoStreamingApp.Data;
using OnlineVideoStreamingApp.Models;


namespace OnlineVideoStreamingApp.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly UserManager<OnlineVideoStreamingAppUser>? _userManager;
        private readonly IConfiguration? _configuration;
        private readonly OnlineVideoStreamingAppContext _context;

        private readonly HttpClient? _client;

        public AdvertisementsController(UserManager<OnlineVideoStreamingAppUser> userManager,IConfiguration configuration, OnlineVideoStreamingAppContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
            _client = new HttpClient();
        }

        public List<AdvertisementsModel> getAds()
        {
            List<AdvertisementsModel> ads = new List<AdvertisementsModel>();
            HttpResponseMessage response = _client.GetAsync("https://localhost:7272/api/AdvertisementsModels").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var s = JsonConvert.DeserializeObject<List<AdvertisementsModel>>(data);
                if (s != null)
                {
                    ads = s;
                }

            }

            return ads;
        }
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(this.User);
            var ads = getAds().Where(ad => ad.AdPostedById == userId);
            return View(ads);
        }
        public IActionResult PostAdvertisement() {
            ViewData["razor_pay_access_key"] = _configuration["razor_pay_access_key"];
            HttpResponseMessage response = _client.GetAsync("https://localhost:7272/api/RazorPay").Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Order Id is fetched successfully!");
                string data = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(data);
                ViewData["orderId"] = data.ToString();
                var userId = _userManager.GetUserId(this.User);
                var currUser = _userManager.Users.FirstOrDefault(u => u.Id == userId);
                ViewData["currUser"] = currUser;
                
                
            }
            return View();
        }


        public async Task<bool> PostAd(string adTitle, string adDescription, string productLink,string posterUrl)
        {
            string AD_POST_URL = "https://localhost:7272/api/AdvertisementsModels";
            AdvertisementsModel adModel = new AdvertisementsModel();
            adModel.AdPosterUrl = posterUrl;
            adModel.productLink = productLink;
            adModel.AdvertisementTitle = adTitle;
            adModel.AdvertisementDescription = adDescription;
            var userId = _userManager.GetUserId(this.User);
            var user = await _userManager.FindByIdAsync(userId);
            adModel.AdPostedById = userId;
            var json = JsonConvert.SerializeObject(adModel);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(AD_POST_URL, content);

            Console.WriteLine(response);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("advertisement posted successfully");
                return true;
            }
            else
            {
                Console.WriteLine("failed");
                return false;   
            }



        }

        [HttpPost]
        public async Task<IActionResult> PostAdvertisement(string productLink, string AdvertisementTitle, string AdvertisementDescription,IFormFile adPoster)
        {
            
                Console.WriteLine(adPoster.FileName);
                VideosModelsController vid = new VideosModelsController(_context, _userManager, _configuration);
                string posterUrl = vid.UploadToAwsBucketAndGetUrl(adPoster.FileName, "img");
                Console.WriteLine(posterUrl);
                Console.WriteLine("in the post of advertisements : ");
                Console.WriteLine(AdvertisementDescription);
                Console.WriteLine(AdvertisementTitle);
                Console.WriteLine(productLink);
                
                if(await PostAd(AdvertisementTitle, AdvertisementDescription, productLink, posterUrl))
            {
                return RedirectToAction("Index", "Advertisements");

            }
            return View();


        }

        public IActionResult EditAdvertisement(int id)
        {
            Console.WriteLine(id);
            HttpResponseMessage response = _client.GetAsync($"https://localhost:7272/api/AdvertisementsModels/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var s = JsonConvert.DeserializeObject<AdvertisementsModel>(data);
                if (s != null)
                {
                    return View(s);
                }

            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditAdvertisement(string productLink, string AdvertisementTitle, string AdvertisementDescription, string AdPosterUrl,IFormFile adPoster,string id)
        {
            
            Console.WriteLine($"Product Link: {productLink}");
            Console.WriteLine($"Advertisement Title: {AdvertisementTitle}");
            Console.WriteLine($"Advertisement Description: {AdvertisementDescription}");
            Console.WriteLine($"Ad Poster Url: {AdPosterUrl}");
            Console.WriteLine($"ID: {id}");

       

            var userId = _userManager.GetUserId(this.User);
            AdvertisementsModel editedAdModel = new AdvertisementsModel();
            editedAdModel.productLink = productLink;
            editedAdModel.AdvertisementTitle = AdvertisementTitle;
            editedAdModel.AdvertisementDescription = AdvertisementDescription;
            editedAdModel.AdPostedById = userId;
            editedAdModel.Id = int.Parse(id);

            if (adPoster != null)
            {
                VideosModelsController vid = new VideosModelsController(_context, _userManager, _configuration);
                string updatedUrl = vid.UploadToAwsBucketAndGetUrl(adPoster.FileName, "img");
                Console.WriteLine("UPDATE THE AD POSTER");
                editedAdModel.AdPosterUrl = updatedUrl;

            }
            else
            {
                editedAdModel.AdPosterUrl = AdPosterUrl;

            }

            var json = JsonConvert.SerializeObject(editedAdModel);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PutAsync($"https://localhost:7272/api/AdvertisementsModels/{id}", content).Result;
            Console.WriteLine(response);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("POSTED SUCCESSFULLY");

                string data = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(data);

            }
            else
            {
                Console.WriteLine("failed");
            }

            return RedirectToAction("Index", "AdvertisementS");
        }

        [HttpPost]
        public async Task<IActionResult> DelAd(string adId)
        {
            Console.WriteLine(adId);
            int delId = int.Parse(adId);
            HttpResponseMessage response = _client.DeleteAsync($"https://localhost:7272/api/AdvertisementsModels/{delId}").Result;

            Console.WriteLine(response);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("DELETED SUCCESSFULLY");
                //ViewData["deleteMessage"] = $"deleted ad {delId}";
                string data = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(data);

            }
            return RedirectToAction("Index", "AdvertisementS");

        }
    }
}
