using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartRadio.Data;
using SmartRadio.Data.Models;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Services.Implementations
{
    public class FollowerService : IFollowerService
    {
        private SmartRadioDbContext db;

        public FollowerService(SmartRadioDbContext db)
        {
            this.db = db;
        }

        public IQueryable<User> GetFollowing(string userId)
        {
            return this.db.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Following)
                .Select(uf => uf.User2);
        }

        public async Task<User> Follow(string userId, string friendId)
        {
            var user = await this.db.Users.Include(u => u.Following).FirstOrDefaultAsync(u => u.Id == userId);
            var following = await this.db.Users.FirstOrDefaultAsync(u => u.Id == friendId);
            var userFollower = new UserFollower()
            {
                User1 = user,
                User2 = following
            };

            user?.Following.Add(userFollower);

            await this.db.SaveChangesAsync();

            return following;
        }

        public async Task UnFollow(string userId, string friendId)
        {
            var user = await this.db.Users.Include(u => u.Following).FirstOrDefaultAsync(u => u.Id == userId);
            var userFollower = user.Following.FirstOrDefault(uf => uf.Id2 == friendId);
            user.Following.Remove(userFollower);

            await this.db.SaveChangesAsync();
        }

        public async Task UpdateRadioStation(string userId, string radioStation)
        {
            var user = await this.db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            user.RadioStation = radioStation;

            await this.db.SaveChangesAsync();
        }

        public async Task<List<string>> getUserFollowers(string userId)
        {
            return await this.db.Users
                .SelectMany(u => u.Following.Where(f => f.Id2 == userId).Select(f => f.Id1)).ToListAsync();
        }
    }
}
