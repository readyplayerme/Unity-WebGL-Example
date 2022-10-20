using ReadyPlayerMe.Analytics;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class AvatarLoaderEditorWindow : EditorWindowBase
    {
        private const string AVATAR_HEADING = "Download Avatar into Scene";
        private const string URL_SAVE_KEY = "UrlSaveKey";
        private const string VOICE_TO_ANIM_SAVE_KEY = "VoiceToAnimSaveKey";
        private const string EYE_ANIMATION_SAVE_KEY = "EyeAnimationSaveKey";
        private const string MODEL_CACHING_SAVE_KEY = "ModelCachingSaveKey";

        private const string EDITOR_WINDOW_NAME = "avatar loader";
        private const string URL_SHORTCODE_ERROR = "Please enter a valid Avatar URL or Shortcode. Click here to read more about this issue.";

        private string url;
        private bool useVoiceToAnim;
        private bool useEyeAnimations;
        private bool initialized;

        private readonly GUILayoutOption fieldHeight = GUILayout.Height(20);
        private readonly GUILayoutOption inputFieldWidth = GUILayout.Width(145);

        private GUIStyle errorButtonStyle;
        private GUIStyle avatarButtonStyle;
        private GUIStyle parametersSelectButtonStyle;

        private bool isValidUrlShortcode;
        private AvatarLoaderSettings avatarLoaderSettings;

        public static void ShowWindowMenu()
        {
            var window = (AvatarLoaderEditorWindow) GetWindow(typeof(AvatarLoaderEditorWindow));
            window.titleContent = new GUIContent("Avatar Loader");
            window.ShowUtility();

            AnalyticsEditorLogger.EventLogger.LogOpenDialog(EDITOR_WINDOW_NAME);
        }

        private void OnFocus()
        {
            isValidUrlShortcode = EditorUtilities.IsUrlShortcodeValid(url);
        }

        private void OnGUI()
        {
            if (!initialized) Initialize();
            LoadStyles();
            DrawContent(DrawContent);
        }

        private void DrawContent()
        {
            Vertical(() =>
            {
                DrawInputField();
                DrawExtras();
                DrawLoadAvatarButton();
            });
        }

        private void LoadStyles()
        {
            if (avatarButtonStyle == null)
            {
                avatarButtonStyle = new GUIStyle(GUI.skin.button);
                avatarButtonStyle.fontStyle = FontStyle.Bold;
                avatarButtonStyle.fontSize = 14;
                avatarButtonStyle.fixedHeight = ButtonHeight;
            }

            if (parametersSelectButtonStyle == null)
            {
                parametersSelectButtonStyle = new GUIStyle(GUI.skin.button);
                parametersSelectButtonStyle.fontStyle = FontStyle.Bold;
                parametersSelectButtonStyle.fontSize = 10;
                parametersSelectButtonStyle.fixedHeight = 18;
                parametersSelectButtonStyle.fixedWidth = 60;
            }

            if (errorButtonStyle == null)
            {
                errorButtonStyle = new GUIStyle();
                errorButtonStyle.fixedWidth = 20;
                errorButtonStyle.fixedHeight = 20;
                errorButtonStyle.margin = new RectOffset(2, 0, 2, 2);
            }
        }

        private void Initialize()
        {
            url = EditorPrefs.GetString(URL_SAVE_KEY);
            useEyeAnimations = EditorPrefs.GetBool(EYE_ANIMATION_SAVE_KEY);
            useVoiceToAnim = EditorPrefs.GetBool(VOICE_TO_ANIM_SAVE_KEY);

            if (EditorPrefs.GetBool(MODEL_CACHING_SAVE_KEY)) EditorPrefs.SetBool(MODEL_CACHING_SAVE_KEY, false);
            SetEditorWindowName(EDITOR_WINDOW_NAME);
            isValidUrlShortcode = EditorUtilities.IsUrlShortcodeValid(url);
            initialized = true;
        }

        private void DrawInputField()
        {
            Vertical(() =>
            {
                GUILayout.Label(AVATAR_HEADING, HeadingStyle);

                Horizontal(() =>
                {
                    GUILayout.Space(2);

                    EditorGUILayout.LabelField(new GUIContent("Avatar URL or Shortcode", "Paste the avatar URL or shortcode received from Ready Player Me here."), inputFieldWidth);

                    var tempText = EditorUtilities.TextFieldWithPlaceholder(url, " Paste Avatar URL or shortcode here", fieldHeight);

                    if (tempText != url)
                    {
                        url = tempText.Split('?')[0];
                        isValidUrlShortcode = EditorUtilities.IsUrlShortcodeValid(url);
                    }

                    GUIContent button = new GUIContent((Texture) AssetDatabase.LoadAssetAtPath("Assets/Plugins/Ready Player Me/Editor/error.png", typeof(Texture)), URL_SHORTCODE_ERROR);

                    if (!isValidUrlShortcode && GUILayout.Button(button, errorButtonStyle))
                    {
                        Application.OpenURL("https://docs.readyplayer.me/ready-player-me/avatars/avatar-creator#avatar-url-and-data-format");
                    }

                    EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);

                    EditorPrefs.SetString(URL_SAVE_KEY, url);

                    GUILayout.Space(4);
                });
            }, true);
        }

        private void DrawExtras()
        {
            Vertical(() =>
            {
                GUILayout.Label("Extras", HeadingStyle);

                Horizontal(() =>
                {
                    GUILayout.Space(2);

                    Vertical(() =>
                    {
                        useEyeAnimations = EditorGUILayout.ToggleLeft(new GUIContent("Use Eye Animations",
                            "Optional helper component for random eye rotation and blinking, for a less static look."), useEyeAnimations, fieldHeight);
                        EditorPrefs.SetBool(EYE_ANIMATION_SAVE_KEY, useEyeAnimations);

                        useVoiceToAnim = EditorGUILayout.ToggleLeft(new GUIContent("Use Voice To Animation",
                            "Optional helper component for voice amplitude to jaw bone movement."), useVoiceToAnim, fieldHeight);
                        EditorPrefs.SetBool(VOICE_TO_ANIM_SAVE_KEY, useVoiceToAnim);
                    });
                });
            }, true);
        }

        private void DrawLoadAvatarButton()
        {
            Horizontal(() =>
            {
                GUI.enabled = isValidUrlShortcode && !string.IsNullOrEmpty(url);
                if (GUILayout.Button("Load Avatar into the Current Scene", avatarButtonStyle))
                {
                    AnalyticsEditorLogger.EventLogger.LogLoadAvatarFromDialog(url, useEyeAnimations, useVoiceToAnim);
                    avatarLoaderSettings = Resources.Load<AvatarLoaderSettings>(AvatarLoaderSettings.RESOURCE_PATH);
                    var avatarLoader = new AvatarLoader();
                    avatarLoader.SaveInProjectFolder = true;
                    avatarLoader.OnFailed += Failed;
                    avatarLoader.OnCompleted += Completed;
                    avatarLoader.AvatarConfig = avatarLoaderSettings.AvatarConfig;
                    avatarLoader.LoadAvatar(url);
                }

                GUI.enabled = true;

                GUILayout.Space(4);
            }, true);
        }

        private void Failed(object sender, FailureEventArgs args)
        {
            Debug.LogError($"{args.Type} - {args.Message}");
        }

        private void Completed(object sender, CompletionEventArgs args)
        {
            GameObject avatar = args.Avatar;

            if (useEyeAnimations) avatar.AddComponent<EyeAnimationHandler>();
            if (useVoiceToAnim) avatar.AddComponent<VoiceHandler>();

            EditorUtilities.CreatePrefab(avatar, $"{DirectoryUtility.GetRelativeProjectPath(avatar.name)}/{avatar.name}.prefab");

            Selection.activeObject = args.Avatar;
        }
    }
}
