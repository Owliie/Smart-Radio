using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartRadio.Data;
using SmartRadio.Data.Models;

namespace SmartRadio.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder Seed(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<SmartRadioDbContext>().Database.Migrate();

                var db = serviceScope.ServiceProvider.GetService<SmartRadioDbContext>();

                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();

                Task
                    .Run(async () =>
                    {
                        if (!await db.Roles.AnyAsync())
                        {
                            var radioRole = new IdentityRole("Radio");

                            await roleManager.CreateAsync(radioRole);
                        }

                        if (!await db.Users.AnyAsync())
                        {
                            var pesho = new User()
                            {
                                UserName = "pesho",
                                Email = "pesho@gmail.com",
                            };

                            var mario = new User()
                            {
                                UserName = "mario",
                                Email = "mario@gmail.com"
                            };

                            var misho = new User()
                            {
                                UserName = "misho",
                                Email = "misho@gmail.com"
                            };

                            var radio = new User()
                            {
                                UserName = "radio",
                                Email = "radio@gmail.com"
                            };

                            pesho.Friends.Add(new UserFriend()
                            {
                                User2 = mario
                            });

                            pesho.Friends.Add(new UserFriend()
                            {
                                User2 = misho
                            });

                            await userManager.CreateAsync(pesho, "test12");
                            await userManager.CreateAsync(mario, "test12");
                            await userManager.CreateAsync(misho, "test12");
                            await userManager.CreateAsync(radio, "test12");
                            await userManager.AddToRoleAsync(radio, "Radio");

                            await db.SaveChangesAsync();
                        }
                    }).Wait();
            }

            return app;
        }
    }
}
