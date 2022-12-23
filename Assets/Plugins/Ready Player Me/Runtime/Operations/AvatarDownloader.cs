using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ReadyPlayerMe
{
    public class AvatarDownloader : IOperation<AvatarContext>
    {
        private const string TAG = nameof(AvatarDownloader);
        private readonly bool downloadInMemory;

        public int Timeout { get; set; }

        public Action<float> ProgressChanged { get; set; }

        public AvatarDownloader(bool downloadInMemory = false)
        {
            this.downloadInMemory = downloadInMemory;
        }

        public async Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            if (context.AvatarUri.Equals(default(AvatarUri)))
            {
                throw new InvalidDataException($"Expected cast {typeof(string)} instead got ");
            }

            if (!context.Metadata.IsUpdated && File.Exists(context.AvatarUri.LocalModelPath))
            {
                SDKLogger.Log(TAG, "Loading model from cache.");
                context.Bytes = File.ReadAllBytes(context.AvatarUri.LocalModelPath);
                return context;
            }

            if (context.Metadata.IsUpdated)
            {
                AvatarCache.ClearAvatar(context.AvatarUri.Guid, context.SaveInProjectFolder);
            }

            if (downloadInMemory)
            {
                context.Bytes = await DownloadIntoMemory(context.AvatarUri.ModelUrl, context.AvatarConfig, token);
                return context;
            }

            context.Bytes = await DownloadIntoFile(context.AvatarUri.ModelUrl, context.AvatarUri.Guid, context.AvatarUri.LocalModelPath, context.AvatarConfig, token);
            return context;
        }

        public async Task<byte[]> DownloadIntoMemory(string url, AvatarConfig avatarConfig = null, CancellationToken token = new CancellationToken())
        {
            if (avatarConfig)
            {
                var parameters = AvatarConfigProcessor.ProcessAvatarConfiguration(avatarConfig);
                url += parameters;
                SDKLogger.Log(TAG, $"Download URL with parameters: {url}");
            }

            SDKLogger.Log(TAG, "Downloading avatar into memory.");

            var dispatcher = new WebRequestDispatcher();
            dispatcher.ProgressChanged = ProgressChanged;

            try
            {
                var response = await dispatcher.DownloadIntoMemory(url, token, Timeout);
                return response.Data;
            }
            catch (Exception exception)
            {
                throw Fail($"Failed to download glb model into memory. {exception}");
            }
        }

        public async Task<byte[]> DownloadIntoFile(string url, string guid, string path, AvatarConfig avatarConfig = null, CancellationToken token = new CancellationToken())
        {
            if (avatarConfig)
            {
                var parameters = AvatarConfigProcessor.ProcessAvatarConfiguration(avatarConfig);
                url += parameters;
                SDKLogger.Log(TAG, $"Download URL with parameters: {url}");
            }

            SDKLogger.Log(TAG, $"Downloading avatar into file at {path}");

            var dispatcher = new WebRequestDispatcher();
            dispatcher.ProgressChanged = ProgressChanged;

            try
            {
                var response = await dispatcher.DownloadIntoFile(url, path, token, Timeout);
                return response.Data;
            }
            catch (Exception exception)
            {
                throw Fail($"Failed to download glb model into file. {exception}");
            }
        }

        private Exception Fail(string message)
        {
            SDKLogger.Log(TAG, message);
            throw new CustomException(FailureType.ModelDownloadError, message);
        }
    }
}
