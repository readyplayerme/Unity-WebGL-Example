using System;
using System.Collections;

namespace ReadyPlayerMe
{
    public class AvatarDownloader
    {
        private const string TAG = nameof(AvatarDownloader);
        public Action<FailureType, string> OnFailed;
        public Action<float> OnProgressChanged;
        public Action<byte[]> OnCompleted;

        public IEnumerator DownloadIntoMemory(string url)
        {
            SDKLogger.Log(TAG, "Downloading avatar into memory.");

            var dispatcher = new WebRequestDispatcher();
            dispatcher.OnProgressChanged = OnProgressChanged;
            dispatcher.OnCompleted = response => OnCompleted?.Invoke(response.Data);
            dispatcher.OnFailed = error => Fail(FailureType.ModelDownloadError,
                $"Failed to download glb model into memory. {error}");
            yield return dispatcher.DownloadIntoMemory(url);
        }

        public IEnumerator DownloadIntoFile(string url, string guid, string path)
        {
            SDKLogger.Log(TAG, $"Downloading avatar into file at {path}");

            var dispatcher = new WebRequestDispatcher();
            dispatcher.OnProgressChanged = OnProgressChanged;
            dispatcher.OnCompleted = response => OnCompleted?.Invoke(response.Data);
            dispatcher.OnFailed = error => Fail(FailureType.ModelDownloadError,
                $"Failed to download glb model into file. {error}");
            yield return dispatcher.DownloadIntoFile(url, path);
        }

        private void Fail(FailureType failureType, string message)
        {
            SDKLogger.Log(TAG, message);
            OnFailed?.Invoke(failureType, message);
        }
    }
}
