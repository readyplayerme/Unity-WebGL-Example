using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class AvatarRenderDownloader : IOperation<AvatarContext>
    {
        private const string TAG = nameof(AvatarRenderDownloader);
        private const string RENDER_URL = "https://render.readyplayer.me/render";
        private const string INVALID_RENDER_URL_ERROR = "Not a valid Avatar Render Url. Check render settings";
        private const string RENDERS = "renders";
        private readonly string[] renderExtensions = { ".png", ".jpg" };

        public int Timeout { get; set; }
        public Action<float> ProgressChanged { get; set; }

        public async Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            try
            {
                context.Data = await RequestAvatarRenderUrl(context.Bytes, token);
                SDKLogger.Log(TAG, "Avatar Render Downloaded");
                return context;
            }
            catch (CustomException exception)
            {
                throw new CustomException(FailureType.AvatarRenderError, exception.Message);
            }
        }

        public async Task<Texture2D> RequestAvatarRenderUrl(byte[] payload, CancellationToken token = new CancellationToken())
        {
            string response;
            var dispatcher = new WebRequestDispatcher();
            dispatcher.ProgressChanged += ProgressChanged;

            try
            {
                response = await dispatcher.Dispatch(RENDER_URL, payload, token);

            }
            catch (CustomException exception)
            {
                throw new CustomException(exception.FailureType, exception.Message);
            }

            return await Parse(response, token);
        }

        private async Task<Texture2D> Parse(string json, CancellationToken token)
        {
            try
            {
                var renderData = JObject.Parse(json);
                var avatarRenderUrl = renderData[RENDERS][0].ToString();

                if (string.IsNullOrEmpty(avatarRenderUrl) || !ValidateRenderUrl(avatarRenderUrl))
                {
                    throw new CustomException(FailureType.AvatarRenderError, INVALID_RENDER_URL_ERROR);
                }

                var webRequestDispatcher = new WebRequestDispatcher();
                return await webRequestDispatcher.DownloadTexture(avatarRenderUrl, token);
            }
            catch (Exception exception)
            {
                throw new CustomException(FailureType.AvatarRenderError, exception.Message);
            }
        }

        private bool ValidateRenderUrl(string renderUrl)
        {
            var url = renderUrl.ToLower();
            return renderExtensions.Any(extension => url.EndsWith(extension));
        }
    }
}
