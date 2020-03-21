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
        private readonly ISongRecognitionService songRecognitionService;
        private readonly IOuterMusicRecognitionService outerMusicRecognitionService;

        public SongsController(ISongRecognitionService songRecognitionService, IOuterMusicRecognitionService outerMusicRecognitionService)
        {
            this.songRecognitionService = songRecognitionService;
            this.outerMusicRecognitionService = outerMusicRecognitionService;
        }

        [HttpPost]
        public async Task<IActionResult> ResolveMetadata()
        {
            if (this.Request.Body.Length == 0)
            {
                return this.BadRequest();
            }

            var tempPath = "./temp.mp3";
            var result = new SongData();
            var outerResult = "";
            byte[] songPart = null;

            using (var body = new StreamReader(this.Request.Body))
            {
                body.BaseStream.Seek(0, SeekOrigin.Begin);
                var songData = await body.ReadToEndAsync();
                songPart = Convert.FromBase64String(songData);
            }

            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                stream.Seek(0, SeekOrigin.End);
                await stream.WriteAsync(songPart, 0, songPart.Length);
            }

            try
            {
                result = await this.songRecognitionService.GetMetadata(tempPath);
                outerResult = this.outerMusicRecognitionService.GetMetadataFromFile(tempPath);
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
            else
            {
                // result from song recognition API
                Console.WriteLine(outerResult);
                return this.Json(new SongMetadata()
                {
                    Name = "Song",
                    Artist = "Artist"
                });
            }
        }
    }
}
