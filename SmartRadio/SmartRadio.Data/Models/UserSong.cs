using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartRadio.Data.Models
{
    public class UserSong
    {
        [Required]
        public string ListenerId { get; set; }

        [Required]
        public User Listener { get; set; }
        
        public int SongId { get; set; }

        [Required]
        public SongData Song { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string RadioStation { get; set; }
    }
}
