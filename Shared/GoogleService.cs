using System;
using System.IO;

namespace Shared
{
    public class GoogleService
    {
        public GoogleService()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(Directory.GetCurrentDirectory(), "SpeechReconition-58c4f44ebbbe.json"), EnvironmentVariableTarget.Process);
        }
    }
}