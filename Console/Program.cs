using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var converter = new AudioCutter();
            var (fileName, rate) = converter.CompressAudio(File.OpenRead("music.mp3"), P => System.Console.WriteLine("converting " + P)).Result;
            var link = new GoogleStorage("speechreconition-197119").UploadFile(File.OpenRead(fileName), Path.GetFileName(fileName), P => System.Console.WriteLine("uploading " + P)).Result;
            var str = new Speech().LongRecognize(link, "ru", 16000, A => System.Console.WriteLine(A)).Result;
            System.Console.WriteLine(str);
            System.Console.Read();
        }
    }
}
