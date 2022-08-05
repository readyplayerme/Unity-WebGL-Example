using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class Banner
    {
        private const string BANNER_PATH = "Assets/Plugins/Ready Player Me/Editor/RPM_EditorImage_Banner.png";
        private const string HEADING = "Ready Player Me Unity SDK";

        private const int HEADER_TOP = 110;
        private const float HEADER_HEIGHT = 10f;
        private const float HEADER_WIDTH = 320f;
        private const float HEADER_WIDTH_WITHOUT_VERSION = 250;

        private readonly Texture2D banner;
        private readonly GUIStyle headerTextStyle;

        public Banner()
        {
            banner = AssetDatabase.LoadAssetAtPath<Texture2D>(BANNER_PATH);
            headerTextStyle = new GUIStyle();
            headerTextStyle.fontSize = 18;
            headerTextStyle.richText = true;
            headerTextStyle.fontStyle = FontStyle.Bold;
            headerTextStyle.normal.textColor = Color.white;
        }

        public void DrawBanner(float xOffset, bool showVersion = true)
        {
            GUI.DrawTexture(new Rect((xOffset - banner.width) / 2, 0, banner.width, banner.height), banner);
            var width = showVersion ? HEADER_WIDTH : HEADER_WIDTH_WITHOUT_VERSION;
            var headingText = showVersion ? $"{HEADING} {ApplicationData.GetData().SDKVersion}" : HEADING;
            EditorGUI.DropShadowLabel(new Rect((xOffset - width) / 2, HEADER_TOP, HEADER_HEIGHT, banner.height), headingText, headerTextStyle);
            GUILayout.Space(142);
        }
    }
}
