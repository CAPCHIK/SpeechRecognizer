using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class AudioCutter
    {
        public async Task<(string fileName, int rate)> CompressAudio(Stream fileStream, Action<int> onProgress = null)
        {
            var outFormat = new WaveFormat(16000, 1);
            var fileName = $"{Guid.NewGuid()}.wav";
            using (var reader = new Mp3FileReader(fileStream))
            using (var resampler = new MediaFoundationResampler(reader, outFormat) { ResamplerQuality = 60 })
            using (var outStream = File.Create(fileName))
            using (var writer = new WaveFileWriter(outStream, outFormat))
            {
                var buffer = new byte[outFormat.AverageBytesPerSecond * 4];
                var targetSize = outFormat.AverageBytesPerSecond * reader.TotalTime.TotalSeconds;
                var readed = 0;
                while (true)
                {
                    int bytesRead = resampler.Read(buffer, 0, buffer.Length);
                    readed += bytesRead;
                    onProgress?.Invoke((int)(readed / targetSize * 100));
                    if (bytesRead == 0)
                    {
                        // end of source provider
                        break;
                    }
                    // Write will throw exception if WAV file becomes too large
                    await outStream.WriteAsync(buffer, 0, bytesRead);
                }
            }
            return (fileName, 16000);
        }



    }
}