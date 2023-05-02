using OnlineVideoStreamingApp.Areas.Identity.Data;

namespace OnlineVideoStreamingApp.Models
{
    public class VideoViewsModel
    {
     


        public int Id { get; set; }

        public VideosModel? Video { get; set; }

        public OnlineVideoStreamingAppUser? ViewedUser { get; set; }
    }
}
