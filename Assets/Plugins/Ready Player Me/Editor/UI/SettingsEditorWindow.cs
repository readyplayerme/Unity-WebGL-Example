using System.IO;
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
        private const string OTHER_SECTION_HEADING = "Other";
        private const string ANALYTICS_LOGGING_DESCRIPTION =
            "We are constantly adding new features and improvements to our SDK. Enable analytics and help us in building even better free tools for more developers. This data is used for internal purposes only and is not shared with third parties.";
        private const string ANALYTICS_PRIVACY_TOOLTIP = "Click to read our Privacy Policy.";
        private const string AVATAR_CONFIG_TOOLTIP = "Assign an avatar configuration to include Avatar API request parameters.";
        private const string ANALYTICS_PRIVACY_URL =
            "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/help-us-improve-the-unity-sdk";
        private const string CACHING_TOOLTIP =
            "Enable caching to improve avatar loading performance at runtime.";
        private const string EDITOR_WINDOW_NAME = "rpm settings";

#if UNITY_EDITOR_LINUX
        private const string SHOW_CACHING_FOLDER_BUTTON_TEXT = "Show in file manager";
#elif UNITY_EDITOR_OSX
        private const string SHOW_CACHING_FOLDER_BUTTON_TEXT = "Reveal in finder";
#else
        private const string SHOW_CACHING_FOLDER_BUTTON_TEXT = "Show in explorer";
#endif

        private const string DOMAIN_VALIDATION_ERROR = "Please enter a valid partner subdomain (e.g. demo). Click here to read more about this issue.";

        private string partnerSubdomain = string.Empty;
        private bool initialized;
        private bool analyticsEnabled;
        private bool avatarCachingEnabled;

        private bool isCacheEmpty;
        private AvatarLoaderSettings avatarLoaderSettings;

        private readonly GUILayoutOption inputFieldWidth = GUILayout.Width(128);
        private readonly GUILayoutOption objectFieldWidth = GUILayout.Width(318);

        private GUIStyle textFieldStyle;
        private GUIStyle textLabelStyle;
        private GUIStyle saveButtonStyle;
        private GUIStyle partnerButtonStyle;
        private GUIStyle avatarCachingButtonStyle;
        private GUIStyle privacyPolicyStyle;
        private GUIStyle errorButtonStyle;

        private AvatarConfig avatarConfig;

        private bool subdomainFocused;
        private string subdomainAfterFocus = string.Empty;
        private const string SUBDOMAIN_FIELD_CONTROL_NAME = "subdomain";

        public static void ShowWindowMenu()
        {
            var window = (SettingsEditorWindow) GetWindow(typeof(SettingsEditorWindow));
            window.titleContent = new GUIContent("Ready Player Me Settings");
            window.ShowUtility();

            AnalyticsEditorLogger.EventLogger.LogOpenDialog(EDITOR_WINDOW_NAME);
        }

        private void Initialize()
        {
            SetEditorWindowName(EDITOR_WINDOW_NAME);

            partnerSubdomain = SubdomainHelper.PartnerDomain;
            SaveSubdomain();

            analyticsEnabled = AnalyticsEditorLogger.IsEnabled;
            avatarLoaderSettings = Resources.Load<AvatarLoaderSettings>(AvatarLoaderSettings.RESOURCE_PATH);
            avatarCachingEnabled = avatarLoaderSettings != null && avatarLoaderSettings.AvatarCachingEnabled;
            isCacheEmpty = AvatarCache.IsCacheEmpty();
            avatarConfig = avatarLoaderSettings != null ? avatarLoaderSettings.AvatarConfig : null;

            initialized = true;
        }

        private void OnFocus()
        {
            isCacheEmpty = AvatarCache.IsCacheEmpty();
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
                saveButtonStyle.fixedWidth = 449;
                saveButtonStyle.fixedHeight = ButtonHeight;
                saveButtonStyle.padding = new RectOffset(5, 5, 5, 5);
            }

            if (textFieldStyle == null)
            {
                textFieldStyle = new GUIStyle(GUI.skin.textField);
                textFieldStyle.fontSize = 12;
            }

            if (textLabelStyle == null)
            {
                textLabelStyle = new GUIStyle(GUI.skin.label);
                textLabelStyle.fontStyle = FontStyle.Bold;
                textLabelStyle.fontSize = 12;
            }

            if (partnerButtonStyle == null)
            {
                partnerButtonStyle = new GUIStyle(GUI.skin.button);
                partnerButtonStyle.fontSize = 12;
                partnerButtonStyle.padding = new RectOffset(5, 5, 5, 5);
            }

            if (avatarCachingButtonStyle == null)
            {
                avatarCachingButtonStyle = new GUIStyle(GUI.skin.button);
                avatarCachingButtonStyle.fontStyle = FontStyle.Bold;
                avatarCachingButtonStyle.fontSize = 12;
                avatarCachingButtonStyle.padding = new RectOffset(5, 5, 5, 5);
                avatarCachingButtonStyle.fixedHeight = ButtonHeight;
                avatarCachingButtonStyle.fixedWidth = 225;
            }

            if (privacyPolicyStyle == null)
            {
                privacyPolicyStyle = new GUIStyle(GUI.skin.label);
                privacyPolicyStyle.fontStyle = FontStyle.Bold;
                privacyPolicyStyle.fontSize = 12;
                privacyPolicyStyle.fixedWidth = 100;
            }

            if (errorButtonStyle == null)
            {
                errorButtonStyle = new GUIStyle();
                errorButtonStyle.fixedWidth = 20;
                errorButtonStyle.fixedHeight = 20;
                errorButtonStyle.margin = new RectOffset(2, 0, 2, 2);
            }
        }

        private void DrawContent()
        {
            Vertical(() =>
            {
                DrawPartnerSettings();
                DrawAvatarSettings();
                DrawAvatarCaching();
                DrawOtherSection();
            });
        }

        private void DrawPartnerSettings()
        {
            Vertical(() =>
            {
                GUILayout.Label(new GUIContent(SETTINGS_HEADING, HELP_TEXT), HeadingStyle);

                Horizontal(() =>
                {
                    GUILayout.Space(2);

                    EditorGUILayout.LabelField("Your subdomain:          https:// ", textLabelStyle, GUILayout.Width(176));
                    var oldValue = partnerSubdomain;
                    GUI.SetNextControlName(SUBDOMAIN_FIELD_CONTROL_NAME);
                    partnerSubdomain = EditorGUILayout.TextField(oldValue, textFieldStyle, GUILayout.Width(128), GUILayout.Height(20));

                    EditorGUILayout.LabelField(".readyplayer.me", textLabelStyle, GUILayout.Width(116), GUILayout.Height(20));
                    GUIContent button = new GUIContent((Texture) AssetDatabase.LoadAssetAtPath("Assets/Plugins/Ready Player Me/Editor/error.png", typeof(Texture)), DOMAIN_VALIDATION_ERROR);

                    var isSubdomainValid = ValidateSubdomain();

                    if (!isSubdomainValid)
                    {
                        if (GUILayout.Button(button, errorButtonStyle))
                        {
                            Application.OpenURL("https://docs.readyplayer.me/ready-player-me/for-partners/partner-subdomains");
                        }

                        EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
                    }

                    if (IsSubdomainFocusLost())
                    {
                        SaveSubdomain();
                    }
                });
            }, true);
        }

        private void DrawAvatarSettings()
        {
            Vertical(() =>
            {
                GUILayout.Label(new GUIContent("Avatar Settings"), HeadingStyle);

                Horizontal(() =>
                {
                    GUILayout.Space(2);
                    EditorGUILayout.LabelField(new GUIContent("Avatar Config", AVATAR_CONFIG_TOOLTIP), inputFieldWidth);
                    avatarConfig = EditorGUILayout.ObjectField(avatarConfig, typeof(AvatarConfig), false, objectFieldWidth) as AvatarConfig;
                    if (avatarLoaderSettings != null && avatarLoaderSettings.AvatarConfig != avatarConfig)
                    {
                        avatarLoaderSettings.AvatarConfig = avatarConfig;
                        SaveAvatarLoaderSettings();
                    }
                });
            }, true);
        }

        private void DrawAvatarCaching()
        {
            Vertical(() =>
            {
                GUILayout.Label("Avatar Caching", HeadingStyle);

                Horizontal(() =>
                {
                    GUILayout.Space(2);
                    var cachingEnabled = avatarCachingEnabled;
                    avatarCachingEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Avatar caching enabled", CACHING_TOOLTIP), avatarCachingEnabled);

                    if (cachingEnabled != avatarCachingEnabled && avatarLoaderSettings != null)
                    {
                        avatarLoaderSettings.AvatarCachingEnabled = avatarCachingEnabled;
                        SaveAvatarLoaderSettings();
                    }
                });

                GUILayout.Space(4);

                Horizontal(() =>
                {
                    GUI.enabled = !isCacheEmpty;
                    if (GUILayout.Button("Clear local avatar cache", avatarCachingButtonStyle))
                    {
                        TryClearCache();
                        isCacheEmpty = AvatarCache.IsCacheEmpty();
                    }
                    GUI.enabled = true;

                    if (GUILayout.Button(SHOW_CACHING_FOLDER_BUTTON_TEXT, avatarCachingButtonStyle))
                    {
                        var path = DirectoryUtility.GetAvatarsDirectoryPath();
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        EditorUtility.RevealInFinder(path);
                    }
                });
            }, true);
        }

        private void DrawOtherSection()
        {
            Vertical(() =>
            {
                GUILayout.Label(OTHER_SECTION_HEADING, HeadingStyle);

                Horizontal(() =>
                {
                    GUILayout.Space(2);
                    analyticsEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Analytics enabled", ANALYTICS_LOGGING_DESCRIPTION), analyticsEnabled, GUILayout.Width(125));

                    if (GUILayout.Button(new GUIContent("(Privacy Policy)", ANALYTICS_PRIVACY_TOOLTIP), privacyPolicyStyle))
                    {
                        Application.OpenURL(ANALYTICS_PRIVACY_URL);
                    }

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
                });
            }, true);
        }

        private void SaveSubdomain()
        {
            EditorPrefs.SetString(WEB_VIEW_PARTNER_SAVE_KEY, partnerSubdomain);

            var subDomain = SubdomainHelper.PartnerDomain;
            if (subDomain != partnerSubdomain)
            {
                AnalyticsEditorLogger.EventLogger.LogUpdatePartnerURL(subDomain, partnerSubdomain);
            }
            SubdomainHelper.SaveToScriptableObject(partnerSubdomain);
        }

        private bool IsSubdomainFocusLost()
        {
            // focus changed from subdomain to another item
            if (GUI.GetNameOfFocusedControl() == string.Empty && subdomainFocused)
            {
                subdomainFocused = false;

                if (subdomainAfterFocus != partnerSubdomain)
                {
                    return true;
                }
            }
            if (GUI.GetNameOfFocusedControl() == SUBDOMAIN_FIELD_CONTROL_NAME && !subdomainFocused)
            {
                subdomainFocused = true;
                subdomainAfterFocus = partnerSubdomain;
            }

            return false;
        }

        private bool ValidateSubdomain()
        {
            return !partnerSubdomain.All(char.IsWhiteSpace) && !partnerSubdomain.Contains('/') && !EditorUtilities.IsUrlShortcodeValid(partnerSubdomain);
        }

        private static void TryClearCache()
        {
            if (AvatarCache.IsCacheEmpty())
            {
                EditorUtility.DisplayDialog("Clear Cache", $"Cache is already empty", "Ok");
                return;
            }
            var size = (AvatarCache.GetCacheSize() / (1024f * 1024)).ToString("F2");
            var avatarCount = AvatarCache.GetAvatarCount();
            if (EditorUtility.DisplayDialog("Clear Cache", $"Do you want to clear all the Avatars cache from persistent data path, {size} MB and {avatarCount} avatars?", "Ok", "Cancel"))
            {
                AvatarCache.Clear();
            }
        }

        private void SaveAvatarLoaderSettings()
        {
            EditorUtility.SetDirty(avatarLoaderSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
