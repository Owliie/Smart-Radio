﻿using SmartRadio.Data;
using SmartRadio.Data.Models;
using SmartRadio.Infrastructure.Mapper;

namespace SmartRadio.Models.SearchVieModels
{
    public class SearchByNameListViewModel : IMapFrom<User>
    {
        public string UserName { get; set; }

        public string Id { get; set; }
    }
}
