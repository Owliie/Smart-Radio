﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using SmartRadio.Data.Models;
using SmartRadio.Infrastructure.Mapper;

namespace SmartRadio.Models
{
    public class SongListViewModel : ICustomMapping
    {
        public string Name { get; set; }

        public string Artist { get; set; }

        public DateTime Date { get; set; }

        public string RadioStation { get; set; }

        public void ConfigureMapping(Profile profile)
        {
            profile
                .CreateMap<UserSong, SongListViewModel>()
                .ForMember(us => us.Name, opts => opts.MapFrom(us => us.Song.Name))
                .ForMember(us => us.Artist, opts => opts.MapFrom(us => us.Song.Artist));
        }
    }
}
