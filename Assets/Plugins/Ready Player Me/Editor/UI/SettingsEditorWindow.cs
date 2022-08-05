using System.Linq;
using ReadyPlayerMe.Analytics;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class SettingsEditorWindow : EditorWindowBase
    {
        private const string WEB_VIEW_PARTNER_SAVE_KEY = "WebViewPartnerSubdomainName";
        private const string SETTINGS_HEADING = "Partner Settings";
        private const string HELP_TEXT =
            "If you are a Ready Player Me partner, please enter your subdomain here to apply your configuration to the WebView.";
        private const string PARTNER_LEARN_MORE_URL =
            "https://docs.readyplayer.me/ready-player-me/for-partners/become-a-partner";
        private const string ANALYTICS_LOGGING_HEADING = "Help us improve Ready Player Me SDK";
        private const string ANALYTICS_LOGGING_DESCRIPTION =
            "We are constantly adding new features and improvements to our SDK. Enable analytics and help us in building even better free tools for more developers. This data is used for internal purposes only and is not shared with third parties.";
        private const string ANALYTICS_PRIVACY_TEXT = "Read our Privacy Policy and learn how we use the data <b>here</b>.";
        private const string ANALYTICS_PRIVACY_URL =
            "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/help-us-improve-the-unity-sdk";
        private const string EDITOR_WINDOW_NAME = "rpm settings";

        private ScriptableObject partner;
        private string partnerSubdomain = "";
        private bool initialized;
        private bool analyticsEnabled;

        private bool saveButtonDirty = true;
        private string SaveButtonText => saveButtonDirty ? "Save" : "Subdomain Saved!";

        private static readonly Vector2Int WindowSize = new Vector2Int(512, 560);

        private GUIStyle textFieldStyle;
        private GUIStyle textLabelStyle;
        private GUIStyle saveButtonStyle;
        private GUIStyle partnerButtonStyle;

        private readonly GUILayoutOption fieldHeight = GUILayout.Height(20);

        [MenuItem("Ready Player Me/Settings")]
        public static void ShowWindowMenu()
        {
            var window = (SettingsEditorWindow) GetWindow(typeof(SettingsEditorWindow));
            window.titleContent = new GUIContent("Ready Player Me Settings");
            window.minSize = window.maxSize = WindowSize;
            window.ShowUtility();

            AnalyticsEditorLogger.EventLogger.LogOpenDialog(EDITOR_WINDOW_NAME);
        }

        private void Initialize()
        {
            SetEditorWindowName(EDITOR_WINDOW_NAME);
            partner = Resources.Load<ScriptableObject>("Partner");
            var type = partner.GetType();
            var method = type.GetMethod("GetSubdomain");
            partnerSubdomain = method?.Invoke(partner, null) as string;
            analyticsEnabled = AnalyticsEditorLogger.IsEnabled;
            initialized = true;
        }

        private void OnGUI()
        {
            if (!initialized) Initialize();
            LoadStyles();
            DrawContent(DrawContent);
        }

        private void LoadStyles()
        {
            if (saveButtonStyle == null)
            {
                saveButtonStyle = new GUIStyle(GUI.skin.button);
                saveButtonStyle.fontSize = 14;
                saveButtonStyle.fontStyle = FontStyle.Bold;
                saveButtonStyle.fixedHeight = 40;
                saveButtonStyle.padding = new RectOffset(5, 5, 10, 10);
            }

            if (textFieldStyle == null)
            {
                textFieldStyle = new GUIStyle(GUI.skin.textField);
                textFieldStyle.alignment = TextAnchor.MiddleCenter;
                textFieldStyle.fontSize = 16;
            }

            if (textLabelStyle == null)
            {
                textLabelStyle = new GUIStyle(GUI.skin.label);
                textLabelStyle.alignment = TextAnchor.MiddleLeft;
                textLabelStyle.fontStyle = FontStyle.Bold;
                textLabelStyle.fontSize = 16;
            }

            if (partnerButtonStyle == null)
            {
                partnerButtonStyle = new GUIStyle(GUI.skin.button);
                partnerButtonStyle.fontSize = 12;
                partnerButtonStyle.padding = new RectOffset(5, 5, 8, 8);
            }
        }

        private void DrawContent()
        {
            Vertical(() =>
            {
                EditorGUILayout.Space();
                GUILayout.Label(SETTINGS_HEADING, HeadingStyle);
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(HELP_TEXT, MessageType.Info);
                EditorGUILayout.Space();

                Horizontal(() =>
                {
                    EditorGUILayout.LabelField("https://", textLabelStyle, GUILayout.Width(65), GUILayout.Height(30));
                    var oldValue = partnerSubdomain;
                    partnerSubdomain = EditorGUILayout.TextField(oldValue, textFieldStyle, GUILayout.Width(300),
                        GUILayout.Height(30));
                    EditorGUILayout.LabelField(".readyplayer.me", textLabelStyle, GUILayout.Width(128),
                        GUILayout.Height(30));

                    if (oldValue != partnerSubdomain)
                    {
                        saveButtonDirty = true;
                    }
                });

                EditorGUILayout.Space();

                if (GUILayout.Button(SaveButtonText, saveButtonStyle) && ValidateSubdomain())
                {
                    saveButtonDirty = false;
                    EditorPrefs.SetString(WEB_VIEW_PARTNER_SAVE_KEY, partnerSubdomain);

                    var type = partner.GetType();
                    var field = type.GetField("Subdomain");

                    var subDomain = field.GetValue(partner).ToString();

                    AnalyticsEditorLogger.EventLogger.LogUpdatePartnerURL(subDomain, partnerSubdomain);

                    field.SetValue(partner, partnerSubdomain);
                    EditorUtility.SetDirty(partner);
                    AssetDatabase.SaveAssets();
                }

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Learn more about the partner program", partnerButtonStyle))
                {
                    Application.OpenURL(PARTNER_LEARN_MORE_URL);
                }
                EditorGUILayout.EndHorizontal();

                Vertical(() =>
                {
                    EditorGUILayout.Space();
                    GUILayout.Label(ANALYTICS_LOGGING_HEADING, HeadingStyle);
                    EditorGUILayout.Space();
                    GUILayout.Label(ANALYTICS_LOGGING_DESCRIPTION, DescriptionStyle);
                    EditorGUILayout.Space(5);
                    if (GUILayout.Button(ANALYTICS_PRIVACY_TEXT, DescriptionStyle))
                    {
                        Application.OpenURL(ANALYTICS_PRIVACY_URL);
                    }
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal(GUILayout.Width(100));
                    analyticsEnabled = EditorGUILayout.ToggleLeft("Yes, enable analytics", analyticsEnabled);
                    if (AnalyticsEditorLogger.IsEnabled != analyticsEnabled)
                    {
                        if (analyticsEnabled)
                        {
                            AnalyticsEditorLogger.Enable();
                        }
                        else
                        {
                            AnalyticsEditorLogger.Disable();
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }, true);

            }, true);
        }

        private bool ValidateSubdomain()
        {
            if (partnerSubdomain.All(char.IsWhiteSpace))
            {
                EditorUtility.DisplayDialog("Subdomain Format Error",
                    $"Partner subdomain cannot contain white space. Value you entered is '{partnerSubdomain}'.", "OK");
                return false;
            }
            return true;
        }
    }
}
