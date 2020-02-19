using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SmartRadio.Services.Interfaces;
using SmartRadio.Services.OuterAPI;

namespace SmartRadio.Services.Implementations
{
    public class OuterMusicRecognitionService : IOuterMusicRecognitionService
    {
        public string GetMetadataFromFile(string path)
        {
            var config = new Dictionary<string, object>
            {
                {"host", "identify-eu-west-1.acrcloud.com"},
                { "access_key", "85f5d85e1be0ccf0fa0d312288168f1d"},
                { "access_secret", "c2Jw1Kp0KBdM3z5hRSg21DUgQQVMuDEIyXdPPQoc"},
                { "timeout", 10}
            };

            var recognizer = new ACRCloudRecognizer(config);

           return recognizer.RecognizeByFile(path, 0);
        }
    }
}
