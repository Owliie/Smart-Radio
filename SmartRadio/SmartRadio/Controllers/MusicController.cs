using System;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartRadio.Data;
using SmartRadio.Models;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Controllers
{
    [Authorize]
    public class MusicController : Controller
    {
        private readonly IMusicService musicService;
        private readonly UserManager<User> userManager;

        public MusicController(IMusicService musicService, UserManager<User> userManager)
        {
            this.musicService = musicService;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index(int? day, int? month, int? year)
        {
            var date = DateTime.Today;
            if (day != null && month != null && year != null)
            {
                var requestDate = new DateTime(year.Value, month.Value, day.Value);
                if (requestDate > date)
                {
                    return this.BadRequest();
                }

                date = new DateTime(year.Value, month.Value, day.Value);
            }

            var songs = this.musicService
                .GetSongsByDay(this.userManager.GetUserId(this.User), date)
                .ProjectTo<SongListViewModel>()
                .ToList();

            return this.View(songs);
        }
    }
}