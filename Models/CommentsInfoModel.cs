using OnlineVideoStreamingApp.Areas.Identity.Data;

namespace OnlineVideoStreamingApp.Models
{
    public class CommentsInfoModel
    {

        public int Id { get; set; }

        public VideosModel Video { get; set; }


        public string Comment { get; set; }

        public OnlineVideoStreamingAppUser CommentedUser { get; set; }  

        public DateTime? CommentedOn { get; set; }
    }
}
