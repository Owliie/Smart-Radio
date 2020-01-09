using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;

namespace Fingerprinting
{
    class Program
    {
        private static List<int> frequencies;
        
        private static byte[] GetBytesOfSong(string songPath)
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

        private static Complex32[][] Sample(byte[] song, int chunkSize)
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

        private static int GetRangeOfFrequency(int frequency)
        {
            return frequencies.FindIndex(f => f > frequency);
        }

        private static long Fuzz(long peak)
        {
            return peak - (peak % 2);
        }

        private static long Hash(List<long> peaks)
        {
            var i = 1;
            var fingerprint = 0L;

            foreach (var peak in peaks)
            {
                fingerprint += Fuzz(peak) * i;
                i *= 1000;
            }

            return fingerprint;
        }

        private static List<long> Fingerprint(Complex32[][] sampled)
        {
            var size = sampled.GetLength(0);
            var peaks = Enumerable.Repeat(Enumerable.Repeat(0L, frequencies.Count - 1).ToList(), size).ToList();
            var highest = Enumerable.Repeat(Enumerable.Repeat(0F, frequencies.Count).ToList(), size).ToList();
            var hashed = Enumerable.Repeat(0L, size).ToList();

            for (int i = 0; i < sampled.GetLength(0); i++)
            {
                for (int frequency = 40; frequency < 300; frequency++)
                {
                    var magnitude = sampled[i][frequency].Magnitude;
                    var index = GetRangeOfFrequency(frequency);

                    if (magnitude > highest[i][index])
                    {
                        highest[i][index] = magnitude;
                        peaks[i][index - 1] = frequency;
                    }
                }

                hashed[i] = Hash(peaks[i]);
            }

            return hashed;
        }

        private static int MatchMaking(List<long> song, List<long> part, long epsilon = 12000000)
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

        static void Main(string[] args)
        {
            frequencies = new List<int> { 40, 80, 120, 180, 300 };

            var epicPath = @"C:\git\Smart-Radio\experiments\songs\epic.mp3";
            var epicBeginningPath = @"C:\git\Smart-Radio\experiments\songs\epic_part_beginning.mp3";
            var epicPartPath = @"C:\git\Smart-Radio\experiments\songs\epic_part.mp3";
            var pinkoPath = @"C:\git\Smart-Radio\experiments\songs\pinko.mp3";

            var epsilons = new long[] { 2174106058, 12000000, 13000000, 14000000, 15000000, 16000000, 17000000, 18000000, 19000000, 20000000, 21000000 };

            var sampleSize = 1024;

            var epic = Fingerprint(Sample(GetBytesOfSong(epicPath), sampleSize));
            var epicPart = Fingerprint(Sample(GetBytesOfSong(epicPartPath), sampleSize));
//            var epicBeginning = Fingerprint(Sample(GetBytesOfSong(epicBeginningPath), sampleSize));
//            var pinko = Fingerprint(Sample(GetBytesOfSong(pinkoPath), sampleSize));
            Console.WriteLine(epic);
            Console.WriteLine(epicPart);

//            foreach (var epsilon in epsilons)
//            {
//                Console.WriteLine();
//                Console.WriteLine($"epsilon: {epsilon}");
//                Console.WriteLine("epic - epic part: " + MatchMaking(epic, epicPart, epsilon) * 100 / epicPart.Count + "%");
//                Console.WriteLine("epic - epic beginning: " + MatchMaking(epic, epicBeginning, epsilon) * 100 / epicBeginning.Count + "%");
//                Console.WriteLine("pinko - epic part: " + MatchMaking(pinko, epicPart, epsilon) * 100 / epicPart.Count + "%");
//                Console.WriteLine("pinko - epic beginning: " + MatchMaking(pinko, epicBeginning, epsilon) * 100 / epicBeginning.Count + "%");
//            }
        }
    }
}
