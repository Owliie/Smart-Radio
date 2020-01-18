using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartRadio.Data;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Services.Implementations
{
    public class SearchService : ISearchService
    {
        private readonly SmartRadioDbContext db;

        public SearchService(SmartRadioDbContext db)
        {
            this.db = db;
        }

        public IQueryable<User> GetUsersByName(string username)
        {
            return this.db.Users.Where(u => u.UserName.Contains(username));
        }
    }
}
