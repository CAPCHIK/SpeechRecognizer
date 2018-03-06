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

        public void TrimWavFile(string input, string output, TimeSpan start, TimeSpan end)
        {
            using (var reader = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(input)))
            {
                using (WaveFileWriter writer = new WaveFileWriter(output, reader.WaveFormat))
                {
                    int segement = reader.WaveFormat.AverageBytesPerSecond / 1000;

                    int startPosition = (int)start.TotalMilliseconds * segement;
                    startPosition = startPosition - startPosition % reader.WaveFormat.BlockAlign;

                    int endBytes = (int)end.TotalMilliseconds * segement;
                    endBytes = endBytes - endBytes % reader.WaveFormat.BlockAlign;
                    int endPosition = endBytes;
                    Console.WriteLine($"{startPosition} {endPosition}");
                    TrimWavFile(reader, writer, startPosition, endPosition);
                }
            }
            using (var inputReader = new AudioFileReader(output))
            {
                // convert our stereo ISampleProvider to mono
                var mono = new StereoToMonoSampleProvider(inputReader)
                {
                    LeftVolume = 0.0f, // discard the left channel
                    RightVolume = 1.0f // keep the right channel
                };

                // ... OR ... could write the mono audio out to a WAV file
                WaveFileWriter.CreateWaveFile16(output.Replace(".", "1."), mono);
            }
        }

        private void TrimWavFile(WaveStream reader, WaveFileWriter writer, int startPosition, int endPosition)
        {
            reader.Position = startPosition;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPosition)
            {
                int segment = (int)(endPosition - reader.Position);
                if (segment > 0)
                {
                    int bytesToRead = Math.Min(segment, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }
}