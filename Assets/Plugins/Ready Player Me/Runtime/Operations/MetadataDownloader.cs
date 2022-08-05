using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json;

namespace ReadyPlayerMe
{
    public class MetadataDownloader
    {
        private const string TAG = nameof(MetadataDownloader);
        public Action<FailureType, string> OnFailed;
        public Action<AvatarMetadata> OnCompleted;

        public IEnumerator Download(string url)
        {
            SDKLogger.Log(TAG, $"Downloading metadata into memory.");
            var dispatcher = new WebRequestDispatcher();
            dispatcher.OnCompleted = response => ParseResponse(response.Text, response.LastModified);
            dispatcher.OnFailed = error =>
                Fail(FailureType.MetadataDownloadError, $"Failed to download metadata into memory. {error}");
#if UNITY_WEBGL
            // add random tail to the url to prevent JSON from being loaded from the browser cache
            yield return dispatcher.DownloadIntoMemory(url + "?tail=" + Guid.NewGuid());
#else
            yield return dispatcher.DownloadIntoMemory(url);
#endif
        }

        public void SaveToFile(AvatarMetadata metadata, string guid, string path, bool saveInProject)
        {
            DirectoryUtility.ValidateAvatarSaveDirectory(guid, saveInProject);
            var json = JsonConvert.SerializeObject(metadata);
            File.WriteAllText(path, json);
        }

        public AvatarMetadata LoadFromFile(string path)
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<AvatarMetadata>(json);
            }

            return new AvatarMetadata();
        }

        private void ParseResponse(string response, string lastModified)
        {
            var metadata = JsonConvert.DeserializeObject<AvatarMetadata>(response);

            // TODO: when metadata for half-body avatars are fixed, make the check
            // if (metaData.OutfitGender == OutfitGender.None || metaData.BodyType == BodyType.None)
            if (metadata.BodyType == BodyType.None)
            {
                Fail(FailureType.MetadataParseError, "Failed to parse metadata. Unexpected body type.");
            }
            else if (string.IsNullOrEmpty(lastModified))
            {
                Fail(FailureType.MetadataParseError, "Failed to parse metadata. Last-Modified header is missing.");
            }
            else
            {
                metadata.LastModified = DateTime.Parse(lastModified);
                SDKLogger.Log(TAG, $"{metadata.BodyType} metadata loading completed.");
                OnCompleted?.Invoke(metadata);
            }
        }

        private void Fail(FailureType failureType, string message)
        {
            SDKLogger.Log(TAG, message);
            OnFailed?.Invoke(failureType, message);
        }
    }
}
