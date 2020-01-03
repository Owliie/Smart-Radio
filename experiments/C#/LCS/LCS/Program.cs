using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LCS
{
    class Program
    {
        static int Similarity(long[] full, long[] part, long epsilon = 2174106058)
        {
            var similarity = 0;

            for (int i = 0; i < full.Length - part.Length; i++)
            {
                var current = 0;
                for (int j = 0; j < part.Length; j++)
                {
                    if (Math.Abs(part[j] - full[i+j]) <= epsilon)
                    {
                        current++;
                    }
                }

                if (current > similarity)
                {
                    similarity = current;
                    if (current >= part.Length / 2)
                    {
                        break;
                    }
                }
            }

            return similarity;
        }


        public static void Main()
        {
            //hashed song:  4441 hashed part:  221
            var hashed_part = @"C:\git\Smart-Radio\experiments\hashed_part.txt";
            var hashed_full = @"C:\git\Smart-Radio\experiments\hashed_song.txt";

            var sw = new Stopwatch();

            sw.Start();
            var arr1 = File.ReadAllLines(hashed_full).Select(long.Parse).ToArray();
            sw.Stop();
            Console.WriteLine($"Read song in {sw.Elapsed}");
            sw.Reset();
            sw.Start();
            var arr2 = File.ReadAllLines(hashed_part).Select(long.Parse).ToArray();
            sw.Stop();
            Console.WriteLine($"Read part in {sw.Elapsed}");
            sw.Reset();


            sw.Start();
            var len = Similarity(arr1, arr2);
            sw.Stop();
            Console.WriteLine($"Equal: {len}, calculated in {sw.Elapsed}");
        }
    }
}
