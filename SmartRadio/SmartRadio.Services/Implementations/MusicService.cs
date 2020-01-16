using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartRadio.Data;
using SmartRadio.Data.Models;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Services.Implementations
{
    class MusicService : IMusicService
    {
        private SmartRadioDbContext db;

        public MusicService(SmartRadioDbContext db)
        {
            this.db = db;
        }

        public IQueryable<SongData> GetSongs(string userId, DateTime date)
        {
            return this.db.Songs
                .Where(s => s.ListenerId == userId && s.Date.Equals(date)).OrderByDescending(s => s.Date);
        }
    }
}
