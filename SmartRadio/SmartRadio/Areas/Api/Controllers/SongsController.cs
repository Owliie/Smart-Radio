using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SmartRadio.Areas.Api.Models;
using SmartRadio.Data.Models;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Areas.Api.Controllers
{
    public class SongsController : ApiBaseController
    {
        private readonly IMusicRecognitionService musicRecognitionService;
        private readonly IOuterMusicRecognitionService outerMusicRecognitionService;

        public SongsController(IMusicRecognitionService musicRecognitionService, IOuterMusicRecognitionService outerMusicRecognitionService)
        {
            this.musicRecognitionService = musicRecognitionService;
            this.outerMusicRecognitionService = outerMusicRecognitionService;
        }

        [HttpPost]
        public async Task<IActionResult> ResolveMetadata()
        {
            var result = new SongData();
            var (outerTitle, outerArtist) = ("", "");
            var tempPath = "./temp.mp3";
            byte[] songPart;

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
                    (outerTitle, outerArtist) = this.outerMusicRecognitionService.GetMetaData(tempPath);
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

            if (result != null)
            {
                return this.Json(new SongMetadata()
                {
                    Name = result.Name,
                    Artist = result.Artist
                });
            }
            if (outerTitle != "" && outerArtist != "")
            {
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
