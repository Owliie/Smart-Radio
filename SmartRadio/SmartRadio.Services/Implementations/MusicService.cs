using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRadio.Data;
using SmartRadio.Data.Models;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Services.Implementations
{
    public class MusicService : IMusicService
    {
        private SmartRadioDbContext db;

        public MusicService(SmartRadioDbContext db)
        {
            this.db = db;
        }

        public IQueryable<SongData> GetSongsByDay(string userId, DateTime date)
        {
            return this.db.Songs
                .Where(s => s.ListenerId == userId && s.Date.Day.Equals(date.Day)).OrderByDescending(s => s.Date);
        }

        public async Task<SongData> AddSongToList(string userId, string name, string artist, string radioStation)
        {
            var song = new SongData()
            {
                Artist = artist,
                Date = DateTime.Now,
                ListenerId = userId,
                Name = name,
                RadioStation = radioStation
            };

            await this.db.Songs.AddAsync(song);
            await this.db.SaveChangesAsync();

            return song;
        }
    }
}
