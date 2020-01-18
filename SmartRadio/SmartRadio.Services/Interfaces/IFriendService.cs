using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRadio.Data;

namespace SmartRadio.Services.Interfaces
{
    public interface IFriendService
    {
        IQueryable<User> GetFriends(string userId);
        Task<User> AddFriend(string userId, string friendId);
        Task DeleteFriend(string userId, string friendId);
        Task UpdateRadioStation(string userId, string radioStation);
    }
}
