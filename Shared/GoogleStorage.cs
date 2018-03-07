using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class GoogleStorage : GoogleService
    {
        private readonly string projectId;
        private StorageClient storageClient;
        public GoogleStorage(string projectId)
        {
            storageClient = StorageClient.Create();
            this.projectId = projectId;
        }

        public async Task<string> UploadFile(Stream fileStream, string name, Action<int> onProgress = null)
        {
            var bucket = storageClient.ListBuckets(projectId).First();
            var progresser = new Progresser(onProgress, L => (int)((double)L / fileStream.Length * 100));
            await storageClient.UploadObjectAsync(bucket.Name, name, "audio/wav", fileStream, progress: progresser);
            return $"gs://{bucket.Name}/{name}";
        }
        
        private class Progresser : IProgress<IUploadProgress>
        {
            private readonly Action<int> sendPercents;
            private readonly Func<long, int> toPercents;

            public Progresser(Action<int> sendPercents, Func<long, int> toPercents)
            {
                this.sendPercents = sendPercents;
                this.toPercents = toPercents;
            }
            public void Report(IUploadProgress value)
            {
                sendPercents?.Invoke(toPercents(value.BytesSent));
            }
        }
    }
}
