using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

namespace ReadyPlayerMe
{
    public class UrlProcessor
    {
        private const string TAG = nameof(UrlProcessor);

        private const string SHORT_CODE_REGEX = "^[A-Z0-9]{6}$";
        private const string SHORT_CODE_URL_REGEX = "^(https://readyplayer.me/api/avatar/)[A-Z0-9]{6}$";
        private const string SHORT_CODE_BASE_URL = "https://readyplayer.me/api/avatar";
        private const string GLB_EXTENSION = ".glb";
        private const string JSON_EXTENSION = ".json";

        public Action<FailureType, string> OnFailed;
        public Action<AvatarUri> OnCompleted;

        public bool SaveInProjectFolder { get; set; }

        public void Create(string url)
        {

            if (Regex.Match(url, SHORT_CODE_REGEX).Length > 0)
            {
                GetUrlFromShortCode(url).Run();
            }
            else if (Regex.Match(url, SHORT_CODE_URL_REGEX).Length > 0)
            {
                GetUrlFromShortCode(url.Substring(url.Length - 6)).Run();
            }
            else if (url.ToLower().EndsWith(GLB_EXTENSION))
            {
                CreateFromUrl(url);
            }
            else
            {
                Fail(FailureType.UrlProcessError, "Invalid avatar URL or short code.");
            }
        }

        private void CreateFromUrl(string url)
        {
            try
            {
                var avatarUri = new AvatarUri();

                var fractions = url.Split('/', '.');

                avatarUri.Guid = fractions[fractions.Length - 2];
                var fileName = $"{DirectoryUtility.GetAvatarSaveDirectory(avatarUri.Guid, SaveInProjectFolder)}/{avatarUri.Guid}";
                avatarUri.ModelUrl = url;
                avatarUri.LocalModelPath = $"{fileName}{GLB_EXTENSION}";
                avatarUri.MetadataUrl = url.Replace(GLB_EXTENSION, JSON_EXTENSION);
                avatarUri.LocalMetadataPath = $"{fileName}{JSON_EXTENSION}";

                SDKLogger.Log(TAG, "Processing completed.");
                OnCompleted?.Invoke(avatarUri);
            }
            catch (Exception e)
            {
                Fail(FailureType.UrlProcessError, $"Invalid avatar URL. {e.Message}");
            }
        }

        // TODO Find better approach for getting the correct URL and move to WebRequestDispatcher. 
        private IEnumerator GetUrlFromShortCode(string shortCode)
        {
            SDKLogger.Log(TAG, "Getting URL from short code");
            using (var request = UnityWebRequest.Get($"{SHORT_CODE_BASE_URL}/{shortCode}"))
            {
                yield return request.SendWebRequest();

                if (request.isHttpError || request.isNetworkError)
                {
                    Fail(FailureType.ShortCodeError, $"Invalid avatar short code. {request.error}");
                }
                else
                {
                    CreateFromUrl(request.url);
                }
            }
        }

        private void Fail(FailureType failureType, string message)
        {
            SDKLogger.Log(TAG, message);
            OnFailed?.Invoke(failureType, message);
        }
    }
}
