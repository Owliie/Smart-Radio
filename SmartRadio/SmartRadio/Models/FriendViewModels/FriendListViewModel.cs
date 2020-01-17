using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SmartRadio.Data;
using SmartRadio.Infrastructure.Mapper;

namespace SmartRadio.Models.FriendViewModels
{
    public class FriendListViewModel : IMapFrom<User>
    {
        public string RadioStation { get; set; }

        public string Name { get; set; }

        public string Id { get; set; }
    }
}
