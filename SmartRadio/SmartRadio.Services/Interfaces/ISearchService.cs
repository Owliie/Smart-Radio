using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartRadio.Data;
using SmartRadio.Data.Models;

namespace SmartRadio.Services.Interfaces
{
    public interface ISearchService
    {
        IQueryable<User> GetUsersByName(string userId, string username, int? limit = null);
    }
}
