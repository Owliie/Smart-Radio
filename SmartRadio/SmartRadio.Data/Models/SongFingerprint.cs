using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartRadio.Data.Models
{
    public class SongFingerprint
    {
        [Key]
        public int Id { get; set; }

        public int SongId { get; set; }

        [Required]
        public SongData Song { get; set; }

        public long Hash { get; set; }

        public int Offset { get; set; }
    }
}
