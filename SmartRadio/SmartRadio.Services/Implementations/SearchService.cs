using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartRadio.Data;
using SmartRadio.Data.Models;
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

        public IQueryable<User> GetUsersByName(string userId, string username, int? limit = null)
        {
            var followingUsers = this.db.Users.Where(u => u.Id == userId).SelectMany(u => u.Following).Select(uf => uf.Id2);
            var query = this.db.Users
                .Where(u => u.UserName.Contains(username) && !followingUsers.Contains(u.Id) && u.Id != userId)
                .OrderBy(u => u.UserName);

            return limit.HasValue ? query.Take(limit.Value) : query;
        }
    }
}
