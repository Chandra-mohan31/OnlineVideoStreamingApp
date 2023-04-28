using OnlineVideoStreamingApp.Areas.Identity.Data;

namespace OnlineVideoStreamingApp.Models
{
    public class SubscriberInfoModel
    {

        public int Id { get; set; }

        public OnlineVideoStreamingAppUser Subscriber { get; set; }

        public OnlineVideoStreamingAppUser Subscribee { get; set; }   

    }
}
