using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartRadio.Data;

namespace SmartRadio.Services.Interfaces
{
    public interface ISearchService
    {
        IQueryable<User> GetUsersByName(string username);
    }
}
