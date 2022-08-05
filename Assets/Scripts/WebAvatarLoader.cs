using ReadyPlayerMe;
using UnityEngine;

public class WebAvatarLoader : MonoBehaviour
{
    private const string TAG = nameof(WebAvatarLoader);
    private GameObject avatar;
    private string avatarUrl = "";

    private void Start()
    {
        PartnerSO partner = Resources.Load<PartnerSO>("Partner");
        WebInterface.SetupRpmFrame(partner.Subdomain);
    }
    
    public void OnWebViewAvatarGenerated(string generatedUrl)
    {
        var avatarLoader = new AvatarLoader();
        avatarUrl = generatedUrl;
        avatarLoader.OnCompleted += OnAvatarLoadCompleted;
        avatarLoader.OnFailed += OnAvatarLoadFailed;
        avatarLoader.LoadAvatar(avatarUrl);
    }

    private void OnAvatarLoadCompleted(object sender, CompletionEventArgs args)
    {
        if (avatar) Destroy(avatar);
        avatar = args.Avatar;
    }

    private void OnAvatarLoadFailed(object sender, FailureEventArgs args)
    {
        SDKLogger.Log(TAG,$"Avatar Load failed with error: {args.Message}");
    }
}
