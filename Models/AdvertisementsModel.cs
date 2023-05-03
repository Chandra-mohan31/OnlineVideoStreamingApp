using OnlineVideoStreamingApp.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineVideoStreamingApp.Models
{
    public class AdvertisementsModel
    {
        public int Id { get; set; }

        public string AdvertisementTitle { get; set; }

        public string? AdvertisementDescription { get; set; }

        public string AdPosterUrl { get; set; }

        public string productLink { get; set; }

        [ForeignKey("AspNetUsers")]
        public string? AdPostedById { get; set; }
        
        
    }
}
