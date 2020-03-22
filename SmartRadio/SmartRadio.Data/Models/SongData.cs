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
        public string Name { get; set; }

        [Required]
        public string Artist { get; set; }

        public ICollection<SongFingerprint> Fingerprints { get; set; } = new HashSet<SongFingerprint>();
    }
}
