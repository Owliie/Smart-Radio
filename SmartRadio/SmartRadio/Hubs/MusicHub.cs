using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SmartRadio.Data;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Hubs
{
    public class MusicHub : Hub
    {
        private readonly UserManager<User> userManager;
        private readonly IMusicService musicService;

        public MusicHub(UserManager<User> userManager, IMusicService musicService)
        {
            this.userManager = userManager;
            this.musicService = musicService;
        }

        public async Task JoinRoom(string userId)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, userId);
        }

        public async Task UpdateMusicList(string userId, string name, string artist, string radioStation)
        {
            var song = this.musicService.AddSongToList(userId, name, artist, radioStation);
            await this.Clients.Group(userId).SendAsync("UpdateMusicList", song);
        }
    }
}
