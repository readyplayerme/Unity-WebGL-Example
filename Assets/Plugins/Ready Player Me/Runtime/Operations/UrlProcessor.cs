using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReadyPlayerMe
{
    public class UrlProcessor : IOperation<AvatarContext>
    {
        private const string TAG = nameof(UrlProcessor);

        private const string SHORT_CODE_BASE_URL = "https://api.readyplayer.me/v1/avatars";
        private const string GLB_EXTENSION = ".glb";
        private const string JSON_EXTENSION = ".json";

        public int Timeout { get; set; }
        public Action<float> ProgressChanged { get; set; }

        private bool SaveInProjectFolder { get; set; }

        public async Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            if (string.IsNullOrEmpty(context.Url))
            {
                throw Fail(FailureType.UrlProcessError, "Url string is null");
            }

            SaveInProjectFolder = context.SaveInProjectFolder;
            try
            {
                context.AvatarUri = await Create(context.Url, context.ParametersHash, token);
            }
            catch (Exception e)
            {
                throw Fail(FailureType.UrlProcessError, $"Invalid avatar URL or short code.{e.Message}");
            }

            ProgressChanged?.Invoke(1);
            return context;
        }

        public async Task<AvatarUri> Create(string url, string paramsHash, CancellationToken token = new CancellationToken())
        {
            var fractions = url.Split('?');
            url = fractions[0];
            var avatarApiParameters = fractions.Length > 1 ? $"?{fractions[1]}" : "";
            if (url.ToLower().EndsWith(GLB_EXTENSION))
            {
                return CreateFromUrl(url, paramsHash, avatarApiParameters).Result;
            }

            var urlFromShortCode = await GetUrlFromShortCode(url);
            return CreateFromUrl(urlFromShortCode, paramsHash, avatarApiParameters).Result;
        }

        private Task<AvatarUri> CreateFromUrl(string url, string paramsHash, string avatarApiParameters)
        {
            try
            {
                var avatarUri = new AvatarUri();

                var fractions = url.Split('/', '.');

                avatarUri.Guid = fractions[fractions.Length - 2];
                var fileName = $"{DirectoryUtility.GetAvatarSaveDirectory(avatarUri.Guid, SaveInProjectFolder, paramsHash)}/{avatarUri.Guid}";
                avatarUri.ModelUrl = $"{url}{avatarApiParameters}";
                avatarUri.LocalModelPath = $"{fileName}{GLB_EXTENSION}";

                url = url.Remove(url.Length - GLB_EXTENSION.Length, GLB_EXTENSION.Length);
                avatarUri.MetadataUrl = $"{url}{JSON_EXTENSION}";
                fileName = $"{DirectoryUtility.GetAvatarSaveDirectory(avatarUri.Guid, SaveInProjectFolder)}/{avatarUri.Guid}";
                avatarUri.LocalMetadataPath = $"{fileName}{JSON_EXTENSION}";

                SDKLogger.Log(TAG, "Processing completed.");
                return Task.FromResult(avatarUri);
            }
            catch (Exception e)
            {
                throw Fail(FailureType.UrlProcessError, $"Invalid avatar URL. {e.Message}");
            }
        }

        private Task<string> GetUrlFromShortCode(string shortCode)
        {
            SDKLogger.Log(TAG, "TEST: Getting URL from shortcode");
            var url = $"{SHORT_CODE_BASE_URL}/{shortCode}.glb";
            return Task.FromResult(url);
        }

        private Exception Fail(FailureType failureType, string message)
        {
            SDKLogger.Log(TAG, message);
            throw new CustomException(failureType, message);
        }
    }
}
