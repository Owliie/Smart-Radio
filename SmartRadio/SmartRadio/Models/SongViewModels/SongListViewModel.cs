using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using SmartRadio.Data.Models;
using SmartRadio.Infrastructure.Mapper;

namespace SmartRadio.Models
{
    public class SongListViewModel : IMapFrom<SongData>
    {
        public string Name { get; set; }

        public string Artist { get; set; }

        public DateTime Date { get; set; }

        public string RadioStation { get; set; }
    }
}
