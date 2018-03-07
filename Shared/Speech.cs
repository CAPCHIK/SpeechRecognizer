using Google.Cloud.Speech.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Speech : GoogleService
    {

        public async Task<string> LongRecognize(string googleLink, string language, int rate, Action<int> onProgress = null)
        {
            var speech = SpeechClient.Create();
            var request = await speech.LongRunningRecognizeAsync(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = rate,
                LanguageCode = language,
            }, RecognitionAudio.FromStorageUri(googleLink));

            do
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                request = await request.PollOnceAsync();
                onProgress?.Invoke(request.Metadata.ProgressPercent);
            } while (!request.IsCompleted);
            return request.Result.Results.SelectMany(R => R.Alternatives)
                .Select(A => A.Transcript)
                .ConcatStrings(" ");
        }
    }
}
