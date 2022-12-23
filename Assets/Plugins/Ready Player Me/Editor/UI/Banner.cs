using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class Banner
    {
        private const string BANNER_PATH = "Assets/Plugins/Ready Player Me/Editor/RPM_EditorImage_Banner.png";

        private readonly Texture2D banner;
        private readonly GUIStyle versionTextStyle;

        private const int BANNER_WIDTH = 460;
        private const int BANNER_HEIGHT = 123;

        private const int FONT_SIZE = 14;

        public Banner()
        {
            banner = AssetDatabase.LoadAssetAtPath<Texture2D>(BANNER_PATH);
            versionTextStyle = new GUIStyle();
            versionTextStyle.fontSize = FONT_SIZE;
            versionTextStyle.richText = true;
            versionTextStyle.fontStyle = FontStyle.Bold;
            versionTextStyle.normal.textColor = Color.white;
            versionTextStyle.alignment = TextAnchor.UpperRight;
        }

        public void DrawBanner(Rect position)
        {
            var rect = new Rect((position.size.x - BANNER_WIDTH) / 2, 0, BANNER_WIDTH, BANNER_HEIGHT);
            GUI.DrawTexture(rect, banner);

            var versionText = new Rect((position.width + BANNER_WIDTH) / 2 - 10, 10, 0, 0);
            EditorGUI.DropShadowLabel(versionText, ApplicationData.GetData().SDKVersion, versionTextStyle);

            GUILayout.Space(128);
        }
    }
}
