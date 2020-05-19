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

            using (var tempStream = new MemoryStream())
            {
                this.Request.Body.CopyTo(tempStream);

                if (tempStream.Length == 0)
                {
                    return this.BadRequest();
                }

                tempStream.Seek(0, SeekOrigin.Begin);

                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    while (true)
                    {
                        var buffer = new byte[1024];
                        var read = tempStream.Read(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            break;
                        }
                        stream.Write(buffer, 0, read);
                    }
                }
            }

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
