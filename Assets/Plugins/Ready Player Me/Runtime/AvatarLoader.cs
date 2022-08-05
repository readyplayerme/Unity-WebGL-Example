using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe
{
    public class AvatarLoader
    {
        private const string TAG = nameof(AvatarLoader);

        /// Called upon avatar loader failure.
        public event EventHandler<FailureEventArgs> OnFailed;

        /// Called upon avatar loader progress change.
        public event EventHandler<ProgressChangeEventArgs> OnProgressChanged;

        /// Called upon avatar loader success.
        public event EventHandler<CompletionEventArgs> OnCompleted;

        /// Avatar Importer instance used for importing the GLB model.
        public IAvatarImporter AvatarImporter { get; set; }

        // If true, saves the avatar in the Asset folder.
        public bool SaveInProjectFolder { get; set; }

        private string avatarUrl;
        private AvatarUri avatarUri;
        private AvatarMetadata avatarMetadata;
        private float startTime;

        private readonly MetadataDownloader metadataDownloader;

        public AvatarLoader()
        {
            metadataDownloader = new MetadataDownloader();
        }

        // TODO: add the messages here
        private void Failed(FailureType type, string message)
        {
            OnFailed?.Invoke(this, new FailureEventArgs()
            {
                Type = type,
                Url = avatarUrl,
                Message = message
            });
            SDKLogger.Log(TAG, $"Failed to load avatar. Error type {type}. URL {avatarUrl}. Message {message}");
        }

        private void ProgressChanged(float progress, ProgressType type)
        {
            OnProgressChanged?.Invoke(this, new ProgressChangeEventArgs()
            {
                Type = type,
                Url = avatarUrl,
                Progress = progress
            });
        }

        public void LoadAvatar(string url)
        {
            startTime = Time.timeSinceLevelLoad;
            SDKLogger.Log(TAG, $"Started loading the avatar from URL {url}");

            avatarUrl = url;

            ProgressChanged(0, ProgressType.LoaderStarted);
            ProcessUrl(url);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        [Obsolete("Use AvatarLoader with OnFailed, OnProgress and OnCompleted event handlers.")]
        public void LoadAvatar(string url, Action<GameObject> onAvatarImported = null,
            Action<GameObject, AvatarMetadata> onAvatarLoaded = null)
        {
            avatarUrl = url;

            ProgressChanged(0, ProgressType.LoaderStarted);
            ProcessUrl(url);

            OnCompleted += (_, args) =>
            {
                onAvatarImported?.Invoke(args.Avatar);
                onAvatarLoaded?.Invoke(args.Avatar, avatarMetadata);
            };
        }

        private void ProcessUrl(string url)
        {
            var urlProcessor = new UrlProcessor();
            urlProcessor.SaveInProjectFolder = SaveInProjectFolder;
            urlProcessor.OnFailed = Failed;
            urlProcessor.OnCompleted = uri =>
            {
                avatarUri = uri;
                ProgressChanged(0.1f, ProgressType.UrlProcessed);
                DownloadMetadata();
            };
            urlProcessor.Create(url);
        }

        private void DownloadMetadata()
        {
            metadataDownloader.OnFailed = Failed;
            metadataDownloader.OnCompleted = MetadataDownloaded;
            metadataDownloader.Download(avatarUri.MetadataUrl).Run();
        }

        private void MetadataDownloaded(AvatarMetadata metadata)
        {
            ProgressChanged(0.2f, ProgressType.MetadataDownloaded);

            avatarMetadata = metadata;
            AvatarMetadata previousMetadata = metadataDownloader.LoadFromFile(avatarUri.LocalMetadataPath);

            if (metadata.LastModified == previousMetadata.LastModified)
            {
                LoadModelFromCache();
            }
            else
            {
                metadataDownloader.SaveToFile(metadata, avatarUri.Guid, avatarUri.LocalMetadataPath, SaveInProjectFolder);
                DownloadModel();
            }
        }

        private void DownloadModel()
        {
            var avatarDownloader = new AvatarDownloader();
            avatarDownloader.OnFailed = Failed;
            avatarDownloader.OnProgressChanged = progress =>
            {
                // model download progress between 0.2 to 0.55
                var scaledProgress = progress * 0.35f + 0.2f;
                ProgressChanged(scaledProgress, ProgressType.ModelDownloaded);
            };
            avatarDownloader.OnCompleted = ImportModel;
            avatarDownloader.DownloadIntoFile(avatarUri.ModelUrl, avatarUri.Guid, avatarUri.LocalModelPath).Run();
        }

        private void LoadModelFromCache()
        {
            SDKLogger.Log(TAG, "Loading model from cache.");

            var bytes = File.ReadAllBytes(avatarUri.LocalModelPath);
            ImportModel(bytes);
        }

        private void ImportModel(byte[] avatarBytes)
        {
            SDKLogger.Log(TAG, "Importing avatar model.");

            var importer = AvatarImporter ?? new GltfUtilityAvatarImporter();
            importer.OnFailed = Failed;
            importer.OnProgressChanged = progress =>
            {
                // model download progress between 0.55 to 0.9
                var scaledProgress = progress * 0.35f + 0.55f;
                ProgressChanged(scaledProgress, ProgressType.ModelImported);
            };
            importer.OnCompleted = PrepareAvatar;
            importer.Import(avatarBytes);
        }

        private void PrepareAvatar(GameObject avatar)
        {
            SDKLogger.Log(TAG, "Preparing avatar");

#if UNITY_EDITOR
            if (SaveInProjectFolder)
            {
                Object.DestroyImmediate(avatar);
                AssetDatabase.Refresh();
                var path = $"{DirectoryUtility.GetRelativeProjectPath(avatarUri.Guid)}/{avatarUri.Guid}.glb";
                var avatarAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                avatar = Object.Instantiate(avatarAsset);
            }
#endif

            var oldInstance = GameObject.Find(avatarUri.Guid);
            if (oldInstance)
            {
                Object.DestroyImmediate(oldInstance);
            }

            avatar.name = avatarUri.Guid;

            var processor = new AvatarProcessor();
            processor.ProcessAvatar(avatar, avatarMetadata);
            processor.OnFailed = Failed;
            OnCompleted?.Invoke(this, new CompletionEventArgs
            {
                Avatar = avatar,
                Url = avatarUrl,
                Metadata = avatarMetadata
            });
            ProgressChanged(1f, ProgressType.AvatarLoaded);

            SDKLogger.Log(TAG, $"Avatar loaded in {Time.timeSinceLevelLoad - startTime:F2} seconds.");
        }
    }
}
