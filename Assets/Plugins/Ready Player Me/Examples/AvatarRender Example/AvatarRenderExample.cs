using System.Collections.Generic;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class AvatarRenderExample : MonoBehaviour
    {
        private const string TAG = nameof(AvatarRenderExample);

        [SerializeField] private string url = "https://d1a370nemizbjq.cloudfront.net/9bcc6840-8b8b-420d-a9d8-bc9c275fce8f.glb";
        [SerializeField] private AvatarRenderScene scene = AvatarRenderScene.FullBodyPostureTransparent;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject loadingPanel;

        private string blendShapeMesh = "Wolf3D_Avatar";
        private Dictionary<string, float> blendShapes = new Dictionary<string, float>
        {
            { "mouthSmile", 0.7f },
            { "viseme_aa", 0.5f },
            { "jawOpen", 0.3f }
        };

        void Start()
        {
            var avatarRenderer = new AvatarRenderLoader();
            avatarRenderer.OnCompleted = UpdateSprite;
            avatarRenderer.OnFailed = Fail;
            avatarRenderer.LoadRender(url, scene, blendShapeMesh, blendShapes);
            loadingPanel.SetActive(true);
        }

        private void UpdateSprite(Texture2D render)
        {
            var sprite = Sprite.Create(render, new Rect(0, 0, render.width, render.height), new Vector2(.5f, .5f));
            spriteRenderer.sprite = sprite;
            loadingPanel.SetActive(false);
            SDKLogger.Log(TAG, "Sprite Updated ");
        }

        private void Fail(FailureType type, string message)
        {
            SDKLogger.Log(TAG, $"Failed with error type: {type} and message: {message}");
        }
    }
}
