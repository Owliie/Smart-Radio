using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SmartRadio.Data;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Hubs
{
    [Authorize]
    public class FriendsActivityHub : Hub
    {
        private readonly IFriendService friendService;
        private readonly UserManager<User> userManager;

        public async Task JoinRoom(string userId)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, userId);
        }

        public FriendsActivityHub(IFriendService friendService, UserManager<User> userManager)
        {
            this.friendService = friendService;
            this.userManager = userManager;
        }

        public async Task UpdateRadioStation(string userId, string friendId, string radioStation)
        {
            await this.friendService.UpdateRadioStation(friendId, radioStation);
            await this.Clients.Group(userId).SendAsync("UpdateRadioStation", friendId, radioStation);
        }

        public async Task AddFriend(string userId, string friendId)
        {
            await this.friendService.AddFriend(userId, friendId);
            await this.Clients.Group(userId).SendAsync("AddFriend", userId, friendId);
        }

        public async Task DeleteFriend(string userId, string friendId)
        {
            await this.friendService.DeleteFriend(userId, friendId);
            await this.Clients.Group(userId).SendAsync("DeleteFriend", userId, friendId);
        }
    }
}
