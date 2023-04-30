using OnlineVideoStreamingApp.Areas.Identity.Data;

namespace OnlineVideoStreamingApp.Models
{
    public class YoutubeVideoModel
    {
        public string VideoId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ChannelTitle { get; set; }

        public DateTime PublishedAt { get; set; }

        public string PlayListId { get; set; }
    }
}
