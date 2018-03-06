using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var cutter = new AudioCutter();
            cutter.TrimWavFile("music.mp3", "music1.wav", TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(40));
            cutter.TrimWavFile("music.mp3", "music2.wav", TimeSpan.FromSeconds(40), TimeSpan.FromSeconds(80));
            cutter.TrimWavFile("music.mp3", "music3.wav", TimeSpan.FromSeconds(80), TimeSpan.FromSeconds(120));
            new Speech().Convert("music21.wav");
            System.Console.Read();
        }
    }
}
