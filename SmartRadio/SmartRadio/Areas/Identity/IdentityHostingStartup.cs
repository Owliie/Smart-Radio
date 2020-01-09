using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRadio.Data;
using SmartRadio.Models;

[assembly: HostingStartup(typeof(SmartRadio.Areas.Identity.IdentityHostingStartup))]
namespace SmartRadio.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<SmartRadioDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("SmartRadioDbContextConnection")));

                services.AddDefaultIdentity<User>()
                    .AddEntityFrameworkStores<SmartRadioDbContext>();
            });
        }
    }
}