using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OnlineVideoStreamingApp.Areas.Identity.Data;

namespace OnlineVideoStreamingApp.Models
{
    public class VideosModel
    {
        public int Id { get; set; }

        public OnlineVideoStreamingAppUser PostedByUser { get; set; }

        public string ThumbNailImage { get; set; }

        public string VideoUrl { get; set; }   

        public int LikesCount { get; set; }

        public string VideoTitle { get; set; }  

        public string VideoDescription { get; set; }    

        public string VideoCategory { get; set; }
    }
}
