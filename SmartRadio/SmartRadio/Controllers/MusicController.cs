﻿using System;
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
        public IActionResult Index()
        {
            var songs = this.musicService
                .GetSongsByDay(this.userManager.GetUserId(this.User), DateTime.Today)
                .ProjectTo<SongListViewModel>()
                .ToList();

            return View(songs);
        }
    }
}