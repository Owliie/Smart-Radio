using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using SmartRadio.Services.Helpers.WebRequests;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Services.Implementations
{
    public class OuterMusicRecognitionService : IOuterMusicRecognitionService
    {
        public IConfiguration Configuration { get; set; }

        public OuterMusicRecognitionService(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public (string title, string artist) GetMetaData(string filepath)
        {
            var configuration = this.Configuration.GetValue<string>("ApiToken");
            string URL = "https://api.audd.io/";
            string boundary = "----" + System.Guid.NewGuid();

            string Dateiname = Path.GetFileName(filepath);

            // Read file data
            byte[] data = null;
            using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
            }
            

            // Generate post objects
            var postParameters = new Dictionary<string, object>();
            //postParameters.Add("name", "file");
            postParameters.Add("file", new FileParameter(data, Dateiname, "application/octet-stream"));
            postParameters.Add("api_token", configuration);
            postParameters.Add("return", "timecode,apple_music,deezer,spotify");

            string postURL = URL;
            string userAgent = "Someone";
            HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, userAgent, postParameters);

            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            var fullResponse = responseReader.ReadToEnd();
            webResponse.Close();

            var artistPattern = @"""artist"":""([a-zA-Z0-9\s]+)""";
            var titlePattern = @"""title"":""([a-zA-Z0-9\s]+)""";
            var artist = Regex.Match(fullResponse, artistPattern).Groups[1].ToString();
            var title = Regex.Match(fullResponse, titlePattern).Groups[1].ToString();

            return (artist, title);
        }
    }
}
