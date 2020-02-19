using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartRadio.Services.Interfaces
{
    public interface IOuterMusicRecognitionService
    {
        string GetMetadataFromFile(string path);
    }
}
