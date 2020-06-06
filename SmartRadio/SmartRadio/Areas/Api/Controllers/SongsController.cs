using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartRadio.Areas.Api.Models;
using SmartRadio.Data;
using SmartRadio.Data.Models;
using SmartRadio.Hubs;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Areas.Api.Controllers
{
    public class SongsController : ApiBaseController
    {
        public IHubContext<MusicHub> HubContext { get; set; }

        private readonly IMusicRecognitionService musicRecognitionService;
        private readonly IOuterMusicRecognitionService outerMusicRecognitionService;
        private readonly IMusicService musicService;
        private readonly UserManager<User> userManager;

        public SongsController(IMusicRecognitionService musicRecognitionService, IOuterMusicRecognitionService outerMusicRecognitionService, IHubContext<MusicHub> hubContext, IMusicService musicService, UserManager<User> userManager)
        {
            this.musicRecognitionService = musicRecognitionService;
            this.outerMusicRecognitionService = outerMusicRecognitionService;
            this.HubContext = hubContext;
            this.musicService = musicService;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> ResolveMetadata()
        {
            var result = new SongData();
            var (outerTitle, outerArtist) = ("", "");
            var tempPath = "./temp.mp3";
            byte[] songPart;
            var radioStation = "";

            if (this.Request.Headers.TryGetValue("X-Station-Name", out var headerValues))
            {
                radioStation = headerValues.FirstOrDefault();
            }

            using (var tempStream = new MemoryStream())
            {
                this.Request.Body.CopyTo(tempStream);
                if (tempStream.Length == 0)
                {
                    return this.BadRequest();
                }

                tempStream.Seek(0, SeekOrigin.Begin);

                using (var stringStream = new StreamReader(tempStream))
                {
                    var songData = await stringStream.ReadToEndAsync();
                    songPart = Convert.FromBase64String(songData);
                }
            }

            await System.IO.File.WriteAllBytesAsync(tempPath, songPart);

            try
            {
                result = await this.musicRecognitionService.GetMetadata(tempPath);
                if (result == null)
                {
                    (outerArtist, outerTitle) = this.outerMusicRecognitionService.GetMetaData(tempPath);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest();
            }
            finally
            {
                System.IO.File.Delete(tempPath);
            }

            var userId = this.userManager.GetUserId(this.User);

            if (result != null)
            {
                var song = await this.musicService.AddSongToList(userId, result.Name, result.Artist, radioStation);
                await this.HubContext.Clients.Group(userId).SendAsync("UpdateMusicList", song);

                return this.Json(new SongMetadata()
                {
                    Name = result.Name,
                    Artist = result.Artist
                });
            }
            if (outerTitle != "" && outerArtist != "")
            {
                var song = await this.musicService.AddSongToList(userId, outerTitle, outerArtist, radioStation);
                await this.HubContext.Clients.Group(userId).SendAsync("UpdateMusicList", song);

                return this.Json(new SongMetadata()
                {
                    Name = outerTitle,
                    Artist = outerArtist
                });
            }
            return this.NotFound();
        }
    }
}
