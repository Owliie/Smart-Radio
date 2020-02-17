using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartRadio.Data;
using SmartRadio.Data.Models;

namespace SmartRadio.Infrastructure.Filters
{
    public class UserIdInCookiesFilter : IAsyncActionFilter
    {
        private readonly UserManager<User> userManager;

        public UserIdInCookiesFilter(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await this.userManager.FindByNameAsync(context.HttpContext.User.Identity.Name);
                var id = user.Id;

                context.HttpContext.Response.Cookies.Append("userId", id);
            }

            await next.Invoke();
        }
    }
}
