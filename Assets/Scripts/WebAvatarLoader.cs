using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.WebView;
using UnityEngine;
using BodyType = ReadyPlayerMe.AvatarLoader.BodyType;

namespace ReadyPlayerMe.Examples.WebGL
{
    public class WebAvatarLoader : MonoBehaviour
    {
        private const string TAG = nameof(WebAvatarLoader);
        private GameObject avatar;
        private string avatarUrl = "";
        private FrameEventHandler frameEventHandler;

        private void Start()
        {
            frameEventHandler = new FrameEventHandler();
            frameEventHandler.OnAvatarExport += HandleAvatarLoaded;
            frameEventHandler.OnUserSet += HandleUserSet;
            frameEventHandler.OnUserAuthorized += HandleUserAuthorized;

            var urlConfig = new UrlConfig();
            urlConfig.clearCache = true;
            WebInterface.SetupRpmFrame(urlConfig.BuildUrl(), name);
        }

        private void OnAvatarLoadCompleted(object sender, CompletionEventArgs args)
        {
            if (avatar) Destroy(avatar);
            avatar = args.Avatar;
            if (args.Metadata.BodyType == BodyType.HalfBody)
            {
                avatar.transform.position = new Vector3(0, 1, 0);
            }
        }

        private void OnAvatarLoadFailed(object sender, FailureEventArgs args)
        {
            SDKLogger.Log(TAG, $"Avatar Load failed with error: {args.Message}");
        }

        public void FrameMessageReceived(string message)
        {
            frameEventHandler.MessageReceived(message);
        }

        public void HandleAvatarLoaded(string url)
        {
            LoadAvatarFromUrl(url);
        }

        public void HandleUserSet(string userId)
        {
            Debug.Log($"User set: {userId}");
        }

        public void HandleUserAuthorized(string userId)
        {
            Debug.Log($"User authorized: {userId}");
        }

        public void LoadAvatarFromUrl(string generatedUrl)
        {
            var avatarLoader = new AvatarObjectLoader();
            avatarUrl = generatedUrl;
            avatarLoader.OnCompleted += OnAvatarLoadCompleted;
            avatarLoader.OnFailed += OnAvatarLoadFailed;
            avatarLoader.LoadAvatar(avatarUrl);
        }
    }
}
