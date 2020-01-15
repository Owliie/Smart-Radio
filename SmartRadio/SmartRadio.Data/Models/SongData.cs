using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartRadio.Data.Models
{
    public class SongData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ListenerId { get; set; }

        [Required]
        public User Listener { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Artist { get; set; }

        [Required]
        public string FMStation { get; set; }
    }
}
