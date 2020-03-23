using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartRadio.Services.Interfaces
{
    public interface IOuterMusicRecognitionService
    {
        (string title, string artist) GetMetaData(string path);
    }
}
