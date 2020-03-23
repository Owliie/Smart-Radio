using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRadio.Data;
using SmartRadio.Data.Models;
using SmartRadio.Hubs;
using SmartRadio.Infrastructure.Extensions;
using SmartRadio.Infrastructure.Filters;
using SmartRadio.Services.Implementations;
using SmartRadio.Services.Interfaces;

namespace SmartRadio
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var connectionString = this.Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<SmartRadioDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDefaultIdentity<User>()
                .AddEntityFrameworkStores<SmartRadioDbContext>();

            services.AddIdentityCore<User>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<SmartRadioDbContext>()
                .AddSignInManager<SignInManager<User>>()
                .AddRoleStore<RoleStore<IdentityRole, SmartRadioDbContext>>()
                .AddUserStore<UserStore<User, IdentityRole, SmartRadioDbContext>>();

            services.AddTransient<IFollowerService, FollowerService>();
            services.AddTransient<IMusicService, MusicService>();
            services.AddTransient<ISearchService, SearchService>();
            services.AddTransient<IOuterMusicRecognitionService, OuterMusicRecognitionService>();
            services.AddTransient<IMusicRecognitionService, MusicRecognitionService>();

            services.AddAutoMapper();
            services.AddMvc(config => { config.Filters.Add<UserIdInCookiesFilter>(); }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.Seed();

            app.UseSignalR(routes =>
            {
                routes.MapHub<FollowingActivityHub>("/FollowingActivity");
                routes.MapHub<MusicHub>("/MusicListing");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas", 
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
