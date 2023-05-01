using OnlineVideoStreamingApp.Areas.Identity.Data;

namespace OnlineVideoStreamingApp.Models
{
    public class SavedPostsModelcs
    {
        public int Id { get; set; } 

        public OnlineVideoStreamingAppUser User { get; set; }

        public VideosModel SavedVideo { get; set; }  
    }
}
