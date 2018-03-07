using System;
using System.IO;

namespace Shared
{
    public class GoogleService
    {
        public GoogleService()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(Directory.GetCurrentDirectory(), "SpeechReconition-e15357d1cbb3.json"), EnvironmentVariableTarget.Process);
        }
    }
}