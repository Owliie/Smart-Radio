using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRadio.Data;
using SmartRadio.Infrastructure.Mapper;

namespace SmartRadio.Models.FriendViewModels
{
    public class FriendByNameListViewModel : IMapFrom<User>
    {
        public string Name { get; set; }

        public string Id { get; set; }
    }
}
