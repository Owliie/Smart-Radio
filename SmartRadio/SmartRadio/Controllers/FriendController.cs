using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartRadio.Data;
using SmartRadio.Models.FriendViewModels;
using SmartRadio.Models.SearchVieModels;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Controllers
{
    [Authorize]
    public class FriendController : Controller
    {
        private readonly IFriendService friendService;
        private readonly UserManager<User> userManager;
        private readonly string userId;

        public FriendController(IFriendService friendService, UserManager<User> userManager)
        {
            this.friendService = friendService;
            this.userManager = userManager;
            this.userId = this.userManager.GetUserId(this.User);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var friends = this.friendService
                .GetFriends(this.userId)
                .ProjectTo<FriendListViewModel>()
                .ToList();

            return View(friends);
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.friendService.DeleteFriend(this.userId, id);

            return this.Ok();
        }

        public async Task<IActionResult> Add(string id) //make it with hub
        {
            await this.friendService.AddFriend(this.userId, id);

            return this.Ok();
        }
    }
}