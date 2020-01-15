using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using SmartRadio.Data.Models;

namespace SmartRadio.Data
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        public string RadioStation { get; set; }

        public ICollection<UserFriend> Friends { get; set; } = new HashSet<UserFriend>();
    }
}
