using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartRadio.Data.Models;

namespace SmartRadio.Services.Interfaces
{
    public interface IMusicRecognitionService
    {
        Task<SongData> GetMetadata(string fileName);

        List<long> GetSongData(string fileName);
    }
}
