using System.Runtime.InteropServices;
using ReadyPlayerMe;
using UnityEngine;

public class WebAvatarLoader : MonoBehaviour
{
        [DllImport("__Internal")]
        private static extern void HideReadyPlayerMeFrame();
    
        [SerializeField]
        private string AvatarURL = "https://d1a370nemizbjq.cloudfront.net/5cae00e5-5622-47c7-af0d-02028fad5beb.glb";

        [SerializeField] private bool loadOnStart = true;
        private AvatarLoader avatarLoader;

        private void Start()
        {
            avatarLoader = new AvatarLoader();
            if (loadOnStart)
            {
                LoadAvatar(AvatarURL);
            }
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
            //WebInterface.SetIFrameVisibility(false);
            LoadAvatar(avatarUrl);
        }
}
