using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartRadio.Data.Models
{
    public class UserFollower
    {
        [Required]
        public string Id1 { get; set; }

        [Required]
        public User User1 { get; set; }

        [Required]
        public string Id2 { get; set; }

        [Required]
        public User User2 { get; set; }
    }
}
