using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Microsoft.AspNetCore.Http;
using SmartRadio.Areas.Api.Models;
using SmartRadio.Data.Models;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Areas.Api.Controllers
{
    public class SongsController : ApiBaseController
    {
        private readonly ISongRecognitionService songRecognitionService;

        public SongsController(ISongRecognitionService songRecognitionService)
        {
            this.songRecognitionService = songRecognitionService;
        }

        [HttpPost]
        public async Task<IActionResult> ResolveMetadata(IFormFile songPart)
        {
            if (songPart != null)
            {
                var tempPath = "./temp.mp3";
                var result = new SongData();
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await songPart.CopyToAsync(stream);
                }

                try
                {
                    result = await this.songRecognitionService.GetMetadata(tempPath);
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
            }

            return this.BadRequest();
        }
    }
}
