using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    public static class EditorUtilities
    {
        private const string TAG = nameof(EditorUtilities);
        private const string SHORT_CODE_REGEX = "^[A-Z0-9]{6}$";

        public static void CreatePrefab(GameObject source, string path)
        {
            PrefabUtility.SaveAsPrefabAssetAndConnect(source, path, InteractionMode.AutomatedAction, out var success);
            PrefabUtility.ApplyObjectOverride(source, path, InteractionMode.AutomatedAction);

            SDKLogger.Log(TAG, success ? $"Prefab created successfully at path: {path}" : "Prefab creation failed");

            AssetDatabase.Refresh();
            EditorUtility.SetDirty(source);
        }

        public static string TextFieldWithPlaceholder(string text, string placeholder, GUILayoutOption layoutOptions)
        {
            var newText = EditorGUILayout.TextField(text, layoutOptions);
            if (string.IsNullOrEmpty(text))
            {
                var previousColor = GUI.color;
                GUI.color = Color.grey;
                EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), placeholder);
                GUI.color = previousColor;
            }
            return newText;
        }

        public static bool IsUrlShortcodeValid(string shortcodeUrl)
        {
            return string.IsNullOrEmpty(shortcodeUrl) || Regex.Match(shortcodeUrl, SHORT_CODE_REGEX).Length > 0 || shortcodeUrl.EndsWith(".glb");
        }

        public static class BackgroundStyle
        {
            private static GUIStyle style = new GUIStyle();
            private static Texture2D texture = new Texture2D(1, 1);

            public static GUIStyle Get(Color color)
            {
                texture.SetPixel(0, 0, color);
                texture.Apply();
                style.normal.background = texture;
                return style;
            }
        }
    }
}
