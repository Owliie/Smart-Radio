using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SmartRadio.Data;
using SmartRadio.Models.FollowingViewModels;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Hubs
{
    [Authorize]
    public class FollowingActivityHub : Hub
    {
        private readonly IFollowerService _followerService;
        private readonly UserManager<User> userManager;

        public async Task JoinRoom(string userId)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, userId);

            foreach (var following in this._followerService.GetFollowing(userId))
            {
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, following.Id);
            }

            var followingUsers = this._followerService
                .GetFollowing(userId)
                .ProjectTo<FollowingListViewModel>()
                .ToList();

            await this.Clients.Group(userId).SendAsync("DisplayFollowing", followingUsers);
        }

        public FollowingActivityHub(IFollowerService followerService, UserManager<User> userManager)
        {
            this._followerService = followerService;
            this.userManager = userManager;
        }

        public async Task UpdateRadioStation(string userId, string followingId, string radioStation)
        {
            await this._followerService.UpdateRadioStation(followingId, radioStation);
            await this.Clients.Group(userId).SendAsync("UpdateRadioStation", followingId, radioStation);
        }

        public async Task Follow(string userId, string followingId)
        {
            var following = await this._followerService.Follow(userId, followingId);
            await this.Clients.Group(userId).SendAsync("Follow", following);
        }

        public async Task UnFollow(string userId, string followingId)
        {
            await this._followerService.UnFollow(userId, followingId);
            await this.Clients.Group(userId).SendAsync("UnFollow", followingId);
        }
    }
}
