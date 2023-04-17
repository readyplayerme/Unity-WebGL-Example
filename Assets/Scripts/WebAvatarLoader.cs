using Newtonsoft.Json;
using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Examples.WebGL;
using ReadyPlayerMe.WebView;
using UnityEngine;
using BodyType = ReadyPlayerMe.AvatarLoader.BodyType;

public class WebAvatarLoader : MonoBehaviour, IFrameEventListener
{
    private const string TAG = nameof(WebAvatarLoader);
    private GameObject avatar;
    private string avatarUrl = "";
    
    private void Start()
    {
        var urlConfig = new UrlConfig();
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
        SDKLogger.Log(TAG,$"Avatar Load failed with error: {args.Message}");
    }

    public void FrameMessageReceived(string message)
    {
        var webMessage = JsonConvert.DeserializeObject<WebMessage>(message);
        switch (webMessage.eventName)
        {
            case WebViewEventNames.AVATAR_EXPORT:
                LoadAvatarFromUrl(webMessage.GetAvatarUrl());
                WebInterface.SetIFrameVisibility(false);
                break;
            case WebViewEventNames.USER_SET:
                Debug.Log(webMessage.eventName);
                break;
            case WebViewEventNames.USER_AUTHORIZED:
                Debug.Log(webMessage.eventName);
                break;
        }
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
