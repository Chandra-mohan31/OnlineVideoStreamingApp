using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OnlineVideoStreamingApp.Areas.Identity.Data;

// Add profile data for application users by adding properties to the OnlineVideoStreamingAppUser class
public class OnlineVideoStreamingAppUser : IdentityUser
{
    
    public string? ProfileImageUrl { get; set; }

    
}

