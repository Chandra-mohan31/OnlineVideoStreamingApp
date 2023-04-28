using OnlineVideoStreamingApp.Areas.Identity.Data;

namespace OnlineVideoStreamingApp.Models
{
    public class CustomerSupportModel
    {

        public int Id { get; set; } 

        public OnlineVideoStreamingAppUser QueryPostedUser { get; set; }

        public string Query { get; set; }

        public string status { get; set; }
    }
}
