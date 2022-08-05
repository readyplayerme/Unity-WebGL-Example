using System;
using ReadyPlayerMe.Analytics;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class EditorWindowBase : EditorWindow
    {
        private const string SUPPORT_HEADING = "Support";
        private const string DOCS_URL = "https://bit.ly/UnitySDKDocs";
        private const string FAQ_URL =
            "https://docs.readyplayer.me/overview/frequently-asked-questions/game-engine-faq";
        private const string DISCORD_URL = "https://bit.ly/UnitySDKDiscord";

        private const string RPM_SETTINGS = "rpm settings";

        protected GUIStyle HeadingStyle;
        protected GUIStyle DescriptionStyle;

        private GUIStyle webButtonStyle;

        private readonly GUILayoutOption windowWidth = GUILayout.Width(512);
        private Banner banner;

        private string editorWindowName;

        private void LoadAssets()
        {
            if (banner == null)
            {
                banner = new Banner();
            }

            if (HeadingStyle == null)
            {
                HeadingStyle = new GUIStyle()
                {
                    fontSize = 14,
                    richText = true,
                    fontStyle = FontStyle.Bold,
                    margin = new RectOffset(5, 0, 0, 0)
                };
                HeadingStyle.normal.textColor = Color.white;
            }

            if (DescriptionStyle == null)
            {
                DescriptionStyle = new GUIStyle()
                {
                    fontSize = 12,
                    richText = true,
                    wordWrap = true,
                    margin = new RectOffset(5, 0, 0, 0)
                };
                DescriptionStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
            }

            if (webButtonStyle == null)
            {
                webButtonStyle = new GUIStyle(GUI.skin.button);
                webButtonStyle.fontSize = 12;
                webButtonStyle.fixedWidth = 166;
                webButtonStyle.padding = new RectOffset(5, 5, 5, 5);
            }
        }

        protected void SetEditorWindowName(string editorName)
        {
            editorWindowName = editorName;
        }

        protected void DrawContent(Action content)
        {
            LoadAssets();

            Horizontal(() =>
            {
                GUILayout.FlexibleSpace();
                Vertical(() =>
                {
                    banner.DrawBanner(position.size.x);
                    content?.Invoke();
                    DrawExternalLinks();
                }, windowWidth);
                GUILayout.FlexibleSpace();
            });
        }

        private void DrawExternalLinks()
        {
            Vertical(() =>
            {
                GUILayout.Space(10);
                GUILayout.Label(SUPPORT_HEADING, HeadingStyle);
                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Documentation", webButtonStyle))
                {
                    AnalyticsEditorLogger.EventLogger.LogOpenDocumentation(editorWindowName);
                    Application.OpenURL(DOCS_URL);
                }

                if (GUILayout.Button("FAQ", webButtonStyle))
                {
                    AnalyticsEditorLogger.EventLogger.LogOpenFaq(editorWindowName);
                    Application.OpenURL(FAQ_URL);
                }

                if (GUILayout.Button("Discord", webButtonStyle))
                {
                    AnalyticsEditorLogger.EventLogger.LogOpenDiscord(editorWindowName);
                    Application.OpenURL(DISCORD_URL);
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

            }, true);
        }

        #region Horizontal and Vertical Layouts

        protected void Vertical(Action content, bool isBox = false)
        {
            EditorGUILayout.BeginVertical(isBox ? "Box" : GUIStyle.none);
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }

        private void Vertical(Action content, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }

        protected void Horizontal(Action content, bool isBox = false)
        {
            EditorGUILayout.BeginHorizontal(isBox ? "Box" : GUIStyle.none);
            content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        public void Horizontal(Action content, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}
