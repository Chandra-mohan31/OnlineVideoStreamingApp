using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineVideoStreamingApp.Areas.Identity.Data;
using OnlineVideoStreamingApp.Models;

namespace OnlineVideoStreamingApp.Data;

public class OnlineVideoStreamingAppContext : IdentityDbContext<OnlineVideoStreamingAppUser>
{
    public OnlineVideoStreamingAppContext(DbContextOptions<OnlineVideoStreamingAppContext> options)
        : base(options)
    {
    }
    public DbSet<VideosModel> videos { get; set; }

    public DbSet<SubscriberInfoModel> subscriptionInfo { get; set; }
    public DbSet<VideoLikesModel> likesTable { get; set; }
    public DbSet<CommentsInfoModel> commentsTable { get; set; }
    public DbSet<CustomerSupportModel> customerSupportTable { get; set; }
    public DbSet<SavedPostsModelcs> savedVideos { get; set; }





    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
