using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRadio.Data;
using SmartRadio.Data.Models;

namespace SmartRadio.Services.Interfaces
{
    public interface IFollowerService
    {
        IQueryable<User> GetFollowing(string userId);
        Task<User> Follow(string userId, string friendId);
        Task UnFollow(string userId, string friendId);
        Task UpdateRadioStation(string userId, string radioStation);
    }
}
