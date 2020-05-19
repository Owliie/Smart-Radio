using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartRadio.Areas.Api.Models;
using SmartRadio.Data;
using SmartRadio.Data.Models;
using SmartRadio.Services.Interfaces;

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
                var musicRecognition = serviceScope.ServiceProvider.GetService<IMusicRecognitionService>();

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

                            await userManager.CreateAsync(pesho, "test12");
                            
                            await userManager.CreateAsync(mario, "test12");
                            
                            await userManager.CreateAsync(misho, "test12");
                            
                            await userManager.CreateAsync(radio, "test12");
                            await db.SaveChangesAsync();

                            pesho.Following.Add(new UserFollower()
                            {
                                User2 = mario
                            });

                            pesho.Following.Add(new UserFollower()
                            {
                                User2 = misho
                            });

                            await db.SaveChangesAsync();

                            await userManager.AddToRoleAsync(radio, "Radio");
                            await db.SaveChangesAsync();

                            var songFiles = Directory.EnumerateFiles("./wwwroot/songs");

                            foreach (var songFile in songFiles)
                            {
                                var fileInfo = new FileInfo(songFile);
                                var info = fileInfo.Name.Replace(fileInfo.Extension, "").Split('-');
                                var songData = new SongData()
                                {
                                    Name = info[0],
                                    Artist = info[1]
                                };

                                var fingerprintsList = new List<SongFingerprint>();

                                var fingerprints = musicRecognition.GetSongData(fileInfo.FullName);

                                int index = 0;
                                foreach (var fingerprint in fingerprints)
                                {
                                    var fingerprintModel = new SongFingerprint()
                                    {
                                        Hash = fingerprint,
                                        Offset = index,
                                        Song = songData
                                    };

                                    fingerprintsList.Add(fingerprintModel);
                                    index++;
                                }

                                songData.Fingerprints = fingerprintsList;

                                db.Songs.Add(songData);
                                await db.SaveChangesAsync();
                            }

                            var userSong = new UserSong()
                            {
                                Date = DateTime.Today,
                                Listener = pesho,
                                RadioStation = "101.2",
                                Song = db.Songs.First()
                            };

                            db.UserSongs.Add(userSong);
                            await db.SaveChangesAsync();
                        }
                    }).Wait();
            }

            return app;
        }
    }
}
