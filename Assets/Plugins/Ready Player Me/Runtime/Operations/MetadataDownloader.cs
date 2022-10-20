using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReadyPlayerMe
{
    public class MetadataDownloader : IOperation<AvatarContext>
    {
        private const string TAG = nameof(MetadataDownloader);

        public int Timeout { get; set; }
        public Action<float> ProgressChanged { get; set; }

        public async Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            if (context.AvatarUri.Equals(default(AvatarUri)))
            {
                throw new InvalidDataException($"Expected cast {typeof(string)}");
            }
            context.Metadata = await Download(context.AvatarUri.MetadataUrl, token);
            context.Metadata.IsUpdated = context.SaveInProjectFolder || IsUpdated(context.Metadata, context.AvatarUri, context.AvatarCachingEnabled);
            if (context.Metadata.IsUpdated)
            {
                SaveToFile(context.Metadata, context.AvatarUri.Guid, context.AvatarUri.LocalMetadataPath, context.SaveInProjectFolder);
            }
            return context;
        }

        public async Task<AvatarMetadata> Download(string url, CancellationToken token = new CancellationToken())
        {
            SDKLogger.Log(TAG, $"Downloading metadata into memory.");
            var dispatcher = new WebRequestDispatcher();
            dispatcher.ProgressChanged += ProgressChanged;

            try
            {
#if UNITY_WEBGL
                // add random tail to the url to prevent JSON from being loaded from the browser cache
                var response = await dispatcher.DownloadIntoMemory(url + "?tail=" + Guid.NewGuid(), token, Timeout);
#else
                var response = await dispatcher.DownloadIntoMemory(url, token, Timeout);
#endif
                return ParseResponse(response.Text, response.LastModified);

            }
            catch (CustomException error)
            {
                string message;
                FailureType failureType;
                if (error.FailureType == FailureType.MetadataParseError)
                {
                    failureType = error.FailureType;
                    message = error.Message;
                }
                else
                {
                    failureType = FailureType.MetadataDownloadError;
                    message = $"Failed to download metadata into memory. {error}";

                }

                SDKLogger.Log(TAG, message);
                throw new CustomException(failureType, message);
            }
        }

        public void SaveToFile(AvatarMetadata metadata, string guid, string path, bool saveInProject)
        {
            DirectoryUtility.ValidateAvatarSaveDirectory(guid, saveInProject);
            var json = JsonConvert.SerializeObject(metadata);
            File.WriteAllText(path, json);
        }

        public AvatarMetadata LoadFromFile(string path, bool avatarCachingEnabled)
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<AvatarMetadata>(json);
            }

            return new AvatarMetadata();
        }

        private bool IsUpdated(AvatarMetadata metadata, AvatarUri uri, bool avatarCachingEnabled)
        {
            var previousMetadata = LoadFromFile(uri.LocalMetadataPath, avatarCachingEnabled);
            if (avatarCachingEnabled && metadata.LastModified == previousMetadata.LastModified) return false;
            return true;
        }

        private AvatarMetadata ParseResponse(string response, string lastModified)
        {
            var metadata = JsonConvert.DeserializeObject<AvatarMetadata>(response);

            if (!string.IsNullOrEmpty(lastModified))
            {
                metadata.LastModified = DateTime.Parse(lastModified);
            }

            // TODO: when metadata for half-body avatars are fixed, make the check
            // if (metaData.OutfitGender == OutfitGender.None || metaData.BodyType == BodyType.None)
            if (metadata.BodyType == BodyType.None)
            {
                throw new CustomException(FailureType.MetadataParseError, "Failed to parse metadata. Unexpected body type.");
            }

            if (string.IsNullOrEmpty(lastModified))
            {
                throw new CustomException(FailureType.MetadataParseError, "Failed to parse metadata. Last-Modified header is missing.");
            }

            metadata.LastModified = DateTime.Parse(lastModified);
            SDKLogger.Log(TAG, $"{metadata.BodyType} metadata loading completed.");
            return metadata;
        }
    }


}
