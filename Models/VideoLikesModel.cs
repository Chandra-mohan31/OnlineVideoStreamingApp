using OnlineVideoStreamingApp.Areas.Identity.Data;

namespace OnlineVideoStreamingApp.Models
{
    public class VideoLikesModel
    {

        public int Id { get; set; }

        public VideosModel Video { get; set; }

        public OnlineVideoStreamingAppUser LikedUser { get; set; }
    }
}
