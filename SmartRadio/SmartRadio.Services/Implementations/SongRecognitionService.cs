using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using Microsoft.EntityFrameworkCore;
using NAudio.Wave;
using SmartRadio.Data;
using SmartRadio.Data.Models;
using SmartRadio.Services.Interfaces;

namespace SmartRadio.Services.Implementations
{
    public class SongRecognitionService : ISongRecognitionService
    {
        private readonly SmartRadioDbContext db;
        private readonly List<int> Frequencies = new List<int> { 40, 80, 120, 180, 300 };
        private readonly int sampleSize = 512;

        public SongRecognitionService(SmartRadioDbContext db)
        {
            this.db = db;
        }

        public async Task<SongData> GetMetadata(string fileName)
        {
            var epsilon = 27000000;

            var songFingerprints = this.GetFingerprints(this.Sample(this.GetBytesOfSong(fileName), this.sampleSize));

            foreach (var song in await this.db.Songs.Include(s => s.Fingerprints).ToListAsync())
            {
                var fingerprints = song.Fingerprints.Select(f => f.Hash).ToList();
                if ((this.MatchMaking(fingerprints, songFingerprints, epsilon) * 100 / songFingerprints.Count) >= 45)
                {
                    return song;
                }
            }

            return null;
        }

        public List<long> GetSongData(string fileName)
        {
            return this.GetFingerprints(this.Sample(this.GetBytesOfSong(fileName), this.sampleSize));
        }

        private byte[] GetBytesOfSong(string songPath)
        {
            var songFile = new AudioFileReader(songPath);

            var song = songFile.ToMono();
            var song16 = song.ToWaveProvider16();

            var buffer = new byte[2048];
            var offset = 0;
            while (true)
            {
                var written = song16.Read(buffer, offset, buffer.Length - offset);
                if (written == 0)
                {
                    break;
                }

                offset += written;
                Array.Resize(ref buffer, buffer.Length * 2);
            }

            //trim 0s
            var firstIndex = 0;
            var lastIndex = buffer.Length - 1;
            while (buffer[firstIndex] == 0)
            {
                firstIndex++;
            }

            while (buffer[lastIndex] == 0)
            {
                lastIndex--;
            }

            var songBytes = new byte[lastIndex - firstIndex];
            for (int write = 0, read = firstIndex; write < songBytes.Length; write++, read++)
            {
                songBytes[write] = buffer[read];
            }

            return songBytes;
        }

        private Complex32[][] Sample(byte[] song, int chunkSize)
        {
            var size = song.Length;
            var sampledChunkSize = size / chunkSize;

            var sampled = new Complex32[sampledChunkSize][];

            for (int i = 0; i < sampledChunkSize; i++)
            {
                var time_vars = new Complex32[chunkSize];

                for (int j = 0; j < chunkSize; j++)
                {
                    var num = new Complex32(song[i * chunkSize + j], 0);

                    time_vars[j] = num;
                }

                Fourier.Forward(time_vars);
                sampled[i] = time_vars;
            }

            return sampled;
        }

        private int GetRangeOfFrequency(int frequency)
        {
            return this.Frequencies.FindIndex(f => f > frequency);
        }

        private long Fuzz(long peak)
        {
            return peak - (peak % 2);
        }

        private long Hash(List<long> peaks)
        {
            var i = 1;
            var fingerprint = 0L;

            foreach (var peak in peaks)
            {
                fingerprint += this.Fuzz(peak) * i;
                i *= 1000;
            }

            return fingerprint;
        }

        private List<long> GetFingerprints(Complex32[][] sampled)
        {
            var size = sampled.GetLength(0);
            var peaks = new List<List<long>>();
            for (int i = 0; i < size; i++)
            {
                peaks.Add(new List<long>(new long[this.Frequencies.Count - 1]));
            }
            var highest = new List<List<double>>();
            for (int i = 0; i < size; i++)
            {
                highest.Add(new List<double>(new double[this.Frequencies.Count - 1]));
            }
            var hashed = new List<long>(new long[size]);

            for (int i = 0; i < sampled.GetLength(0); i++)
            {
                for (int frequency = 40; frequency < 300; frequency++)
                {
                    var magnitude = sampled[i][frequency].Magnitude;
                    var index = this.GetRangeOfFrequency(frequency);

                    if (magnitude > highest[i][index - 1])
                    {
                        highest[i][index - 1] = magnitude;
                        peaks[i][index - 1] = frequency;
                    }
                }

                hashed[i] = this.Hash(peaks[i]);
            }

            return hashed;
        }

        private int MatchMaking(List<long> song, List<long> part, long epsilon = 12000000)
        {
            var count = 0;

            for (int i = 0; i < song.Count - part.Count; i++)
            {
                var max = 0;

                for (int j = 0; j < part.Count; j++)
                {
                    if (Math.Abs(part[j] - song[i + j]) <= epsilon)
                    {
                        max++;
                    }
                }

                if (max > count)
                {
                    count = max;
                    if (count >= part.Count / 2)
                    {
                        break;
                    }
                }
            }

            return count;
        }
    }
}
