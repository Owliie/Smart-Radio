using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartRadio.Data;
using SmartRadio.Models.FollowingViewModels;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Controllers
{
    [Authorize]
    public class FollowingController : Controller
    {
        private readonly IFollowerService _followerService;
        private readonly UserManager<User> userManager;
        private readonly string userId;

        public FollowingController(IFollowerService followerService, UserManager<User> userManager)
        {
            this._followerService = followerService;
            this.userManager = userManager;
            this.userId = this.userManager.GetUserId(this.User);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var following = this._followerService
                .GetFollowing(this.userId)
                .ProjectTo<FollowingListViewModel>()
                .ToList();

            return this.Json(following);
        }
    }
}