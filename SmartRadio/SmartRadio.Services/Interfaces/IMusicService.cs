using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRadio.Data.Models;

namespace SmartRadio.Services.Interfaces
{
    public interface IMusicService
    {
        IQueryable<UserSong> GetSongsByDay(string userId, DateTime date);

        Task<UserSong> AddSongToList(string userId, string name, string artist, string radioStation);
    }
}
