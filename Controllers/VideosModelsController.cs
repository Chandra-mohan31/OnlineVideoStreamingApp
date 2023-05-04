using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineVideoStreamingApp.Areas.Identity.Data;
using OnlineVideoStreamingApp.Data;
using OnlineVideoStreamingApp.Models;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;

namespace OnlineVideoStreamingApp.Controllers
{
    public class VideosModelsController : Controller
    {
        private readonly OnlineVideoStreamingAppContext _context;
        private readonly UserManager<OnlineVideoStreamingAppUser> _userManager;
        private readonly IConfiguration _configuration;

        private readonly HttpClient _client;
        //Uri baseAddress = new Uri("https://localhost:7272/api");
        public VideosModelsController(OnlineVideoStreamingAppContext context,UserManager<OnlineVideoStreamingAppUser> userManager,IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _client = new HttpClient();
            //_client.BaseAddress = baseAddress;

        }


        //get advertisements from API
        public List<AdvertisementsModel> getAds()
        {
            List<AdvertisementsModel> ads = new List<AdvertisementsModel>();
            HttpResponseMessage response = _client.GetAsync("https://localhost:7272/api/AdvertisementsModels").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var s = JsonConvert.DeserializeObject<List<AdvertisementsModel>>(data);
                if(s != null)
                {
                    ads = s;
                }

                //Console.WriteLine("=============" + s);
            }

            int count = ads.Count;
            List<AdvertisementsModel> randomAds = new List<AdvertisementsModel>();

            if (count > 0)
            {
                Random rnd = new Random();
                ads = ads.OrderBy(x => rnd.Next()).ToList();

               
                for (int i = 0; i < count && i < 3; i++)
                {
                    randomAds.Add(ads[i]);
                }
            }

            return randomAds;


            
        }

        // GET: VideosModels
        public async Task<IActionResult> Index()
        {
            List<AdvertisementsModel> ads = getAds();
            //List<AdvertisementsModel> ads = new();
            foreach(var ad in ads)
            {
                Console.WriteLine("Ad title : " + ad.AdvertisementTitle);
            }

            ViewData["ads"] = ads;
            var userId = _userManager.GetUserId(this.User);

            var subscribedUsersId = _context.subscriptionInfo
                        .Where(s => s.Subscriber.Id == userId)
                        .Select(s => s.Subscribee.Id).ToList();
            
            foreach(var item in subscribedUsersId)
            {
                Console.WriteLine("SUBSCRIBED TO : " + item);
            }
            ViewData["subscribedUsersId"] = subscribedUsersId;
            ViewData["currUserId"] = userId;
            //var users = _userManager.Users.Where(u => !subscribedUsersId.Contains(u.Id)).ToList();
            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                Console.WriteLine(user.ProfileImageUrl);
            }

            ViewData["users"] = users;

            var videos = await _context.videos.Where(v => subscribedUsersId.Contains(v.PostedByUser.Id)).ToListAsync();


            videos.Sort((v1, v2) => DateTime.Compare((DateTime)v2.VideoPostedOn, (DateTime)v1.VideoPostedOn));



            return _context.videos != null ? 
                          View(videos) :
                          Problem("Entity set 'OnlineVideoStreamingAppContext.videos'  is null.");
        }
        public async Task<IActionResult> YourVideos()
        {
            var userId = _userManager.GetUserId(this.User);
            var videos = await _context.videos.Where(video => video.PostedByUser.Id == userId).ToListAsync();

            videos.Sort((v1, v2) => DateTime.Compare((DateTime)v2.VideoPostedOn, (DateTime)v1.VideoPostedOn));

            ViewData["videos_count"] = videos.Count;

            ViewData["subscribers"] = _context.subscriptionInfo.Where(sub => sub.Subscribee.Id == userId).ToList().Count;


            ViewData["no_channels_subscribed"] = _context.subscriptionInfo.Where(sub => sub.Subscriber.Id == userId).ToList().Count;


            ViewData["ads"] = getAds();

            return _context.videos != null ?
                        View(videos) :
                        Problem("Entity set 'OnlineVideoStreamingAppContext.videos'  is null.");
        }

        // GET: VideosModels/PlayVideo/5
        public async Task<IActionResult> PlayVideo(int? id)
        {
            if (id == null || _context.videos == null)
            {
                return NotFound();
            }

            var videosModel = await _context.videos.Include(user => user.PostedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (videosModel == null)
            {
                return NotFound();
            }
            var currUserId = _userManager.GetUserId(this.User);
            ViewData["currUserId"] = currUserId;
            var VIEWS_LIST = _context.VideoViewsTable.Where(v => v.Video.Id == id).Select(u => u.ViewedUser.Id).ToList();
            if (!VIEWS_LIST.Contains(currUserId))
            {
                VIEWS_LIST.Add(currUserId);
                VideoViewsModel viewsData = new VideoViewsModel();
                var user = _userManager.Users.FirstOrDefault(u => u.Id == currUserId);
                
                viewsData.ViewedUser = user;
                viewsData.Video = videosModel;

                _context.Add(viewsData);
                await _context.SaveChangesAsync();
            }

            var VIEWS_COUNT = VIEWS_LIST.Count();
            ViewData["VIEWS_COUNT"] = VIEWS_COUNT;


            var LIKES_COUNT = _context.likesTable.Where(l => l.Video.Id == id).Count();

            var COMMENTS = _context.commentsTable.Include(c => c.CommentedUser).Where(l => l.Video.Id == id).ToList();
            foreach(var item in COMMENTS)
            {
                Console.WriteLine("commented by : " + item.CommentedUser.UserName);
            }
            ViewData["COMMENTS"] = COMMENTS;

            ViewData["LIKES_COUNT"] = LIKES_COUNT;


            var subscribedUsersId = _context.subscriptionInfo
                        .Where(s => s.Subscriber.Id == currUserId)
                        .Select(s => s.Subscribee.Id).ToList();

            var suggestedVideos = await _context.videos.Where(v => subscribedUsersId.Contains(v.PostedByUser.Id)).Where(v => (v.VideoCategory == videosModel.VideoCategory || v.PostedByUser == videosModel.PostedByUser) && v.Id != id).Include(v => v.PostedByUser).ToListAsync();
            ViewData["SUGGESTED_VIDEOS"] = suggestedVideos;


            ViewData["ads"] = getAds();

            return View(videosModel);
        }

        // GET: VideosModels/Create
        public IActionResult Create()
        {
            return View();
        }


        //function to upload the files into aws and get back the uploaded url
        public string UploadToAwsBucketAndGetUrl(string filename,string filetype)
        {
            var uploadedUrl = "";

            try
            {
                var accessKey = _configuration["accessKey"];
                var secretAccessKey = _configuration["secretAccessKey"];

                // Create an instance of the Amazon S3 client
                var client = new AmazonS3Client(accessKey, secretAccessKey, Amazon.RegionEndpoint.USEast1);

                // Set the name of your S3 bucket and the key under which to store the file
                var bucketName = "onlinevideostreamingapp";

                var keyName = filename;
                string directory = (filetype == "img") ? "images" : "videos";
                // Set the full path of the file you want to upload
                var filePath = $"C:\\Users\\DELL\\source\\repos\\OnlineVideoStreamingApp\\{directory}\\{filename}";

                var contentType = (filetype == "img") ? "image/jpeg" : "video/mp4";
                // Create a TransferUtility object to upload the file
                var transferUtility = new TransferUtility(client);
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    FilePath = filePath,
                    Key = keyName,
                    ContentType = contentType
                };

                // Upload the file to the S3 bucket
                transferUtility.Upload(fileTransferUtilityRequest);
                // Get the URL of the uploaded file
                var url = client.GetPreSignedURL(new Amazon.S3.Model.GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    Expires = DateTime.Now.AddDays(5) // Set the expiration date of the URL
                });

                // Print out the URL of the uploaded file
                url = url.Substring(0, url.IndexOf("?"));
                uploadedUrl = url;
                Console.WriteLine("Uploaded file URL: " + url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return uploadedUrl;
        }
        // POST: VideosModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ThumbNailImage,VideoUrl,LikesCount,VideoTitle,VideoDescription,VideoCategory")] VideosModel videosModel,IFormFile thumbnailimage,IFormFile videofile)
        {
            if(videofile == null || thumbnailimage == null)
            {
                Console.WriteLine("The files were not uploaded!");
            }else
            {

                //upload the image and video to s3 bucket and get back the url to upload the url to the database 

                Console.WriteLine(videofile.FileName);
                Console.WriteLine(thumbnailimage.FileName);
                Console.WriteLine(UploadToAwsBucketAndGetUrl(thumbnailimage.FileName,"img"));
                Console.WriteLine(UploadToAwsBucketAndGetUrl(videofile.FileName,"video"));


                videosModel.VideoUrl = UploadToAwsBucketAndGetUrl(videofile.FileName, "video");
                videosModel.ThumbNailImage = UploadToAwsBucketAndGetUrl(thumbnailimage.FileName, "img");



                Console.WriteLine(ModelState.IsValid);
                videosModel.LikesCount = 0;

                Console.WriteLine("Uploaded the files to aws s3 bucket");
                var userId = _userManager.GetUserId(this.User);
                var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
                Console.WriteLine(user.UserName);
                videosModel.PostedByUser = user;
                videosModel.VideoPostedOn = DateTime.Now;

                _context.Add(videosModel);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("YourVideos","VideosModels");

            }


            return View(videosModel);
        }

        // GET: VideosModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.videos == null)
            {
                return NotFound();
            }

            var videosModel = await _context.videos.FindAsync(id);
            if (videosModel == null)
            {
                return NotFound();
            }
            return View(videosModel);
        }

        // POST: VideosModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ThumbNailImage,VideoUrl,LikesCount,VideoTitle,VideoDescription,VideoCategory")] VideosModel videosModel)
        {
            if (id != videosModel.Id)
            {
                return NotFound();
            }

           
                try
                {
                var userId = _userManager.GetUserId(this.User);
                var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
                videosModel.PostedByUser = user;
                videosModel.VideoPostedOn = DateTime.Now;
                _context.Update(videosModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideosModelExists(videosModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            return View(videosModel);
        }

        // GET: VideosModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.videos == null)
            {
                return NotFound();
            }

            var videosModel = await _context.videos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (videosModel == null)
            {
                return NotFound();
            }

            return View(videosModel);
        }

        // POST: VideosModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.videos == null)
            {
                return Problem("Entity set 'OnlineVideoStreamingAppContext.videos'  is null.");
            }
            var videosModel = await _context.videos.FindAsync(id);
            if (videosModel != null)
            {
                _context.videos.Remove(videosModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //subscribe to a user 
        [HttpPost("Subscribe")]
        public async Task<IActionResult> SubscribeUser(string subscribeeId)
        {
            SubscriberInfoModel subInfo = new SubscriberInfoModel();
            var userId = _userManager.GetUserId(this.User);
            var user = await _userManager.FindByIdAsync(userId);
            var subcribeeUser = await _userManager.FindByIdAsync(subscribeeId);
            subInfo.Subscriber = user;
            subInfo.Subscribee = subcribeeUser;

            _context.Add(subInfo);


            await _context.SaveChangesAsync();
            return RedirectToAction("Index","VideosModels");
        }

        //unsubscribe a user
        // POST: VideosModels/Delete/5
        [HttpPost("UnSubscribeUser")]
        public async Task<IActionResult> UnSubscribeUser(string subscribeeId)
        {
            Console.WriteLine("Inside Unsubscribe");
            var subscriberId = _userManager.GetUserId(this.User);
            Console.WriteLine("subscribeeId : " + subscribeeId);
            Console.WriteLine("subscriberId : " + subscriberId);

            var info = _context.subscriptionInfo
                       .Where(s => s.Subscriber.Id == subscriberId)
                       .Where(s => s.Subscribee.Id == subscribeeId)
                       .Select(s => s.Id).FirstOrDefault();
            Console.WriteLine("  SubInfo Table Id that is to be deleted" + (int)info);
            var subscriberInfoModel = await _context.subscriptionInfo.FindAsync(info);
            if(subscriberInfoModel != null)
                _context.subscriptionInfo.Remove(subscriberInfoModel);


            await _context.SaveChangesAsync();
            return RedirectToAction("Index","VideosModels");

        }


        [HttpPost("LikePost")]
        public async Task<IActionResult> LikePost(string video_id)
        {

            Console.WriteLine(video_id);
            
            Console.WriteLine("In post of Like");

            var userId = _userManager.GetUserId(this.User);
            var user = await _userManager.FindByIdAsync(userId);
            var videoId = int.Parse(video_id);
            var video = await _context.videos.Where(v => v.Id == videoId).FirstOrDefaultAsync();
            var videoLike = await _context.likesTable
    .FirstOrDefaultAsync(vl => vl.Video.Id == videoId && vl.LikedUser.Id == userId);

            if (videoLike == null)
            {
                Console.WriteLine("lIKE THE POST");
                VideoLikesModel likeModel = new VideoLikesModel();
                likeModel.LikedUser = user;
                likeModel.Video = video;

                _context.Add(likeModel);
                
            }
            else
            {
                Console.WriteLine("already liked the post!");
                var likeID = _context.likesTable
                       .Where(s => s.LikedUser.Id == userId)
                       .Where(s => s.Video.Id == videoId)
                       .Select(s => s.Id).FirstOrDefault();
                Console.WriteLine("the likes table id to be deleted : " + (int)likeID);
                var likeInfoModel = await _context.likesTable.FindAsync(likeID);
                if (likeInfoModel != null)
                    _context.likesTable.Remove(likeInfoModel);

            }
            await _context.SaveChangesAsync();

            return RedirectToAction("PlayVideo", "VideosModels", new { id = video_id });
        }

        [HttpPost("CommentOnPost")]
        public async Task<IActionResult> CommentOnPost(string video_id,string comment)
        {
            Console.WriteLine("In post of Comment");

            Console.WriteLine(video_id);
            Console.WriteLine(comment);
            var userId = _userManager.GetUserId(this.User);
            var user = await _userManager.FindByIdAsync(userId);
            var videoId = int.Parse(video_id);
            var video = await _context.videos.Where(v => v.Id == videoId).FirstOrDefaultAsync();

            CommentsInfoModel commentModel = new CommentsInfoModel();

            commentModel.Comment = comment;
            commentModel.Video = video;
            commentModel.CommentedUser = user;
            commentModel.CommentedOn = DateTime.Now;


            _context.commentsTable.Add(commentModel);

            await _context.SaveChangesAsync();

            return RedirectToAction("PlayVideo", "VideosModels", new { id = video_id });


        }


        //uncomment on post 
        [HttpPost("UnComment")]
        public async Task<IActionResult> UnComment(string commentId,string videoId)
        {
            Console.WriteLine("Inside UnComment");
            Console.WriteLine("The comment Id : " + commentId);
            Console.WriteLine("The video Id : " + videoId);
            var currUserId = _userManager.GetUserId(this.User);
            int cId = int.Parse(commentId);
            var commentsInfoModel = await _context.commentsTable.FindAsync(cId);
            //Console.WriteLine("comment id to be deleted : " + commentsInfoModel.Id);
            if (commentsInfoModel != null)
                _context.commentsTable.Remove(commentsInfoModel);


            await _context.SaveChangesAsync();
            return RedirectToAction("PlayVideo", "VideosModels", new { id = videoId });


        }







        [HttpPost("SearchPosts")]
        public async Task<IActionResult> SearchPosts(string filter,string Val)
        {

            Console.WriteLine($"Search terms : filter : {filter} value : {Val}");
            var userId = _userManager.GetUserId(this.User);

            var subscribedUsersId = _context.subscriptionInfo
                        .Where(s => s.Subscriber.Id == userId)
                        .Select(s => s.Subscribee.Id).ToList();

            foreach (var item in subscribedUsersId)
            {
                Console.WriteLine("SUBSCRIBED TO : " + item);
            }
            ViewData["subscribedUsersId"] = subscribedUsersId;
            ViewData["currUserId"] = userId;
            //var users = _userManager.Users.Where(u => !subscribedUsersId.Contains(u.Id)).ToList();
            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                Console.WriteLine(user.ProfileImageUrl);
            }

            ViewData["users"] = users;

            var videos = await _context.videos.Where(v => subscribedUsersId.Contains(v.PostedByUser.Id)).ToListAsync();


            videos.Sort((v1, v2) => DateTime.Compare((DateTime)v2.VideoPostedOn, (DateTime)v1.VideoPostedOn));

            if(filter == "VideoCategory")
            {
                videos = videos.Where(v => v.VideoCategory == Val).ToList();
            }else if(filter == "VideoTitle")
            {
               videos = videos.Where(v => v.VideoTitle == Val).ToList();
            }else if( filter == "UserName")
            {
                videos = videos.Where(v => v.PostedByUser.UserName == Val).ToList();
            }
            
            return _context.videos != null ?
                          View(videos) :
                          Problem("Entity set 'OnlineVideoStreamingAppContext.videos'  is null.");
        }


        //save videos 

        [HttpPost("SavePost")]
        public async Task<IActionResult> SavePost(string video_id)
        {
            Console.WriteLine("In Save Post");

            Console.WriteLine(video_id);
            var userId = _userManager.GetUserId(this.User);
            var user = await _userManager.FindByIdAsync(userId);
            var videoId = int.Parse(video_id);
            var video = await _context.videos.Where(v => v.Id == videoId).FirstOrDefaultAsync();


            SavedPostsModelcs postToSave = new SavedPostsModelcs();

            postToSave.User = user;
            postToSave.SavedVideo = video;

            _context.savedVideos.Add(postToSave);

            await _context.SaveChangesAsync();

            return RedirectToAction("SavedPosts", "VideosModels");


        }


        public async Task<IActionResult> SavedPosts()
        {

            var userId = _userManager.GetUserId(this.User);
            var user = await _userManager.FindByIdAsync(userId);
            //var videoId = int.Parse(video_id);
            //var video = await _context.videos.Where(v => v.Id == videoId).FirstOrDefaultAsync();

            var SAVED_POSTS = _context.savedVideos.Include(s => s.SavedVideo).Include(s => s.SavedVideo.PostedByUser).Where(l => l.User.Id == userId).ToList();

            //foreach(var item in SAVED_POSTS)
            //{
            //    Console.WriteLine(item.SavedVideo.Id);
            //    Console.WriteLine(item.SavedVideo.VideoTitle);
            //    Console.WriteLine(item.SavedVideo.VideoDescription);



            //}
            ViewData["SAVED_POSTS"] = SAVED_POSTS;






            return View();


        }


        public async Task<IActionResult> UnSavePost(string saved_post_id)
        {

            Console.WriteLine("the id from the table to be deleted to unsave post : " + saved_post_id);
            int saved_post_table_id = int.Parse(saved_post_id);
            var savedPostToBeDeleted = await _context.savedVideos.FindAsync(saved_post_table_id);
            //Console.WriteLine("comment id to be deleted : " + commentsInfoModel.Id);
            if (savedPostToBeDeleted != null)
                _context.savedVideos.Remove(savedPostToBeDeleted);


            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "VideosModels");
        }



        private bool VideosModelExists(int id)
        {
          return (_context.videos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
