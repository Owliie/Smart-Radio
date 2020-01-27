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
    public class FriendService : IFriendService
    {
        private SmartRadioDbContext db;

        public FriendService(SmartRadioDbContext db)
        {
            this.db = db;
        }

        public IQueryable<User> GetFriends(string userId)
        {
            return this.db.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Friends)
                .Select(uf => uf.User2);
        }

        public async Task<User> AddFriend(string userId, string friendId)
        {
            var user = await this.db.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => u.Id == userId);
            var friend = await this.db.Users.FirstOrDefaultAsync(u => u.Id == friendId);
            var userFriend = new UserFriend()
            {
                User1 = user,
                User2 = friend
            };

            user?.Friends.Add(userFriend);

            await this.db.SaveChangesAsync();

            return friend;
        }

        public async Task DeleteFriend(string userId, string friendId)
        {
            var user = await this.db.Users.Include(u => u.Friends).FirstOrDefaultAsync(u => u.Id == userId);
            var friend = user.Friends.FirstOrDefault(uf => uf.Id2 == friendId);
            user.Friends.Remove(friend);

            await this.db.SaveChangesAsync();
        }

        public async Task UpdateRadioStation(string userId, string radioStation)
        {
            var user = await this.db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            user.RadioStation = radioStation;

            await this.db.SaveChangesAsync();
        }
    }
}
