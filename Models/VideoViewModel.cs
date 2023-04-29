using OnlineVideoStreamingApp.Areas.Identity.Data;

namespace OnlineVideoStreamingApp.Models
{
    public class VideoViewModel
    {
        public int Id { get; set; }

        public string PostedByUserUsername { get; set; }

        public string ThumbNailImage { get; set; }

        public string VideoUrl { get; set; }

        public int LikesCount { get; set; }

        public string VideoTitle { get; set; }

        public string VideoDescription { get; set; }

        public string VideoCategory { get; set; }

        public string userProfileImageUrl { get; set; }
    }
}
