using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartRadio.Data;
using SmartRadio.Models.SearchVieModels;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ISearchService searchService;
        private readonly UserManager<User> userManager;

        public SearchController(ISearchService searchService, UserManager<User> userManager)
        {
            this.searchService = searchService;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult SearchPreview(string name, int? limit)
        {
            var people = this.searchService
                .GetUsersByName(this.userManager.GetUserId(this.User), name, limit)
                .ProjectTo<SearchByNameListViewModel>()
                .ToList();

            return this.Json(people);
        }

        [HttpPost]
        public IActionResult Search(string name)
        {
            var people = this.searchService
                .GetUsersByName(this.userManager.GetUserId(this.User), name)
                .ProjectTo<SearchByNameListViewModel>()
                .ToList();

            this.ViewData["Search"] = name;

            return this.View(people);
        }
    }
}