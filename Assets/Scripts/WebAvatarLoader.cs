using ReadyPlayerMe;
using UnityEngine;

public class WebAvatarLoader : MonoBehaviour
{
    private string AvatarURL = "https://d1a370nemizbjq.cloudfront.net/5cae00e5-5622-47c7-af0d-02028fad5beb.glb";
    private AvatarLoader avatarLoader;

    private void Start()
    {
        avatarLoader = new AvatarLoader();
    }

    private void OnAvatarImported(GameObject avatar)
    {
        Debug.Log($"Avatar imported. [{Time.timeSinceLevelLoad:F2}]");
    }

    private void OnAvatarLoaded(GameObject avatar, AvatarMetaData metaData)
    {
        Debug.Log($"Avatar loaded. [{Time.timeSinceLevelLoad:F2}]\n\n{metaData}");
    }

    public void LoadAvatar(string avatarUrl)
    {
        AvatarURL = avatarUrl;
        avatarLoader.LoadAvatar(AvatarURL, OnAvatarImported, OnAvatarLoaded);
    }

    public void OnWebViewAvatarGenerated(string avatarUrl)
    {
        LoadAvatar(avatarUrl);
    }
}
