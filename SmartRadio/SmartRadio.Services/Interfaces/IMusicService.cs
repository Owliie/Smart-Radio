﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartRadio.Data.Models;

namespace SmartRadio.Services.Interfaces
{
    public interface IMusicService
    {
        IQueryable<SongData> GetSongsByDay(string userId, DateTime date);
    }
}