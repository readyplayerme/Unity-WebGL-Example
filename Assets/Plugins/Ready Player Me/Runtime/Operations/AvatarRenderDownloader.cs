using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe
{
    public class AvatarRenderDownloader
    {
        private const string TAG = nameof(AvatarRenderDownloader);
        private const string RENDER_URL = "https://render.readyplayer.me/render";
        private const string INVALID_RENDER_URL_ERROR = "Not a valid Avatar Render Url. Check render settings";
        private readonly string[] renderExtensions = { ".png", ".jpg" };

        // Called upon failure
        public Action<FailureType, string> OnFailed { get; set; }

        /// Called upon success.
        public Action<Texture2D> OnCompleted { get; set; }

        public IEnumerator RequestAvatarRenderUrl(byte[] payload)
        {
            using (UnityWebRequest request = UnityWebRequest.Put(RENDER_URL, payload))
            {
                request.method = "POST";
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Fail(FailureType.AvatarRenderError, request.error);
                }
                else
                {
                    DownloadAvatarRenderTexture(request.downloadHandler.text).Run();
                }
            }
        }

        private IEnumerator DownloadAvatarRenderTexture(string data)
        {
            string avatarRenderUrl;

            try
            {
                var renderData = JObject.Parse(data);
                avatarRenderUrl = renderData["renders"][0].ToString();

                if (string.IsNullOrEmpty(avatarRenderUrl) || !ValidateRenderUrl(avatarRenderUrl))
                {
                    Fail(FailureType.AvatarRenderError, INVALID_RENDER_URL_ERROR);
                    yield break;
                }
            }
            catch (Exception e)
            {
                Fail(FailureType.AvatarRenderError, e.Message);
                yield break;
            }

            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(avatarRenderUrl))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Fail(FailureType.AvatarRenderError, request.error);
                }
                else
                {
                    SDKLogger.Log(TAG, "Avatar Render Downloaded");
                    if (request.downloadHandler is DownloadHandlerTexture downloadHandlerTexture) OnCompleted?.Invoke(downloadHandlerTexture.texture);
                }
            }
        }

        private bool ValidateRenderUrl(string renderUrl)
        {
            var url = renderUrl.ToLower();
            foreach (var extension in renderExtensions)
            {
                if (url.EndsWith(extension))
                {
                    return true;
                }
            }

            return false;
        }

        private void Fail(FailureType type, string message)
        {
            OnFailed?.Invoke(FailureType.AvatarRenderError, message);
        }
    }
}
