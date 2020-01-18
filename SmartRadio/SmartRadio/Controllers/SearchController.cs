using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using SmartRadio.Models.SearchVieModels;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService searchService;

        public SearchController(ISearchService searchService)
        {
            this.searchService = searchService;
        }

        [HttpGet]
        public IActionResult Search(string name)
        {
            var people = this.searchService
                .GetUsersByName(name)
                .ProjectTo<SearchByNameListViewModel>()
                .ToList();

            return this.View(people);
        }
    }
}