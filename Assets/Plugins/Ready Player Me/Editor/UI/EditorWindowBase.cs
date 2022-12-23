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

        protected GUIStyle HeadingStyle;
        protected GUIStyle DescriptionStyle;

        private GUIStyle webButtonStyle;

        private readonly GUILayoutOption windowWidth = GUILayout.Width(460);
        protected readonly float ButtonHeight = 30f;
        private Banner banner;

        private string editorWindowName;
        private bool windowResized;

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
                    margin = new RectOffset(5, 0, 0, 8),
                    normal =
                    {
                        textColor = Color.white
                    }
                };
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
                webButtonStyle.fixedWidth = 149;
                webButtonStyle.fixedHeight = ButtonHeight;
                webButtonStyle.padding = new RectOffset(5, 5, 5, 5);
            }
        }

        protected void SetEditorWindowName(string editorName)
        {
            editorWindowName = editorName;
        }

        protected void DrawContent(Action content, bool useBanner = true)
        {
            LoadAssets();

            Horizontal(() =>
            {
                GUILayout.FlexibleSpace();
                Vertical(() =>
                {
                    banner.DrawBanner(position);
                    content?.Invoke();
                    if (useBanner) DrawExternalLinks();
                }, windowWidth);
                GUILayout.FlexibleSpace();
            });

            SetWindowSize();
        }

        private void DrawExternalLinks()
        {
            Vertical(() =>
            {
                GUILayout.Label(SUPPORT_HEADING, HeadingStyle);

                EditorGUILayout.BeginHorizontal();
                GUI.color = Color.white;
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

                EditorGUILayout.EndHorizontal();
            }, true);
        }

        private void SetWindowSize()
        {
            float height = GUILayoutUtility.GetLastRect().height;
            if (!windowResized && height > 1)
            {
                minSize = maxSize = new Vector2(460, height);
                windowResized = true;
            }
        }

        #region Horizontal and Vertical Layouts

        protected void Vertical(Action content, bool isBox = false)
        {
            EditorGUILayout.BeginVertical(isBox ? "Box" : GUIStyle.none);
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }

        protected void Vertical(Action content, params GUILayoutOption[] options)
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

        protected void Horizontal(Action content, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}
