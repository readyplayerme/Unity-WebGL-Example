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

        private string url;
        private bool useVoiceToAnim;
        private bool useEyeAnimations;
        private bool variablesLoaded;

        private readonly GUILayoutOption fieldHeight = GUILayout.Height(20);
        private readonly GUILayoutOption inputFieldWidth = GUILayout.Width(140);
        private GUIStyle avatarButtonStyle;
        private static readonly Vector2 WindowSize = new Vector2Int(512, 374);

        [MenuItem("Ready Player Me/Avatar Loader")]
        private static void ShowWindowMenu()
        {
            var window = (AvatarLoaderEditorWindow) GetWindow(typeof(AvatarLoaderEditorWindow));
            window.titleContent = new GUIContent("Avatar Loader");
            window.minSize = window.maxSize = WindowSize;
            window.ShowUtility();

            AnalyticsEditorLogger.EventLogger.LogOpenDialog(EDITOR_WINDOW_NAME);
        }

        private void OnGUI()
        {
            if (!variablesLoaded) LoadCachedVariables();
            LoadStyles();
            DrawContent(DrawContent);
        }

        private void DrawContent()
        {
            Vertical(() =>
            {
                EditorGUILayout.Space();
                GUILayout.Label(AVATAR_HEADING, HeadingStyle);
                EditorGUILayout.Space();
                DrawInputField();
                DrawOptions();
                DrawLoadAvatarButton();
            }, true);
        }

        private void LoadStyles()
        {
            if (avatarButtonStyle == null)
            {
                avatarButtonStyle = new GUIStyle(GUI.skin.button);
                avatarButtonStyle.fontStyle = FontStyle.Bold;
                avatarButtonStyle.fontSize = 14;
                avatarButtonStyle.padding = new RectOffset(5, 5, 10, 10);
                avatarButtonStyle.fixedHeight = 40;
            }
        }

        private void LoadCachedVariables()
        {
            url = EditorPrefs.GetString(URL_SAVE_KEY);
            useEyeAnimations = EditorPrefs.GetBool(EYE_ANIMATION_SAVE_KEY);
            useVoiceToAnim = EditorPrefs.GetBool(VOICE_TO_ANIM_SAVE_KEY);

            if (EditorPrefs.GetBool(MODEL_CACHING_SAVE_KEY)) EditorPrefs.SetBool(MODEL_CACHING_SAVE_KEY, false);
            SetEditorWindowName(EDITOR_WINDOW_NAME);
            variablesLoaded = true;
        }

        private void DrawInputField()
        {
            GUI.skin.textField.fontSize = 12;

            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField(
                new GUIContent("Avatar Url or Short Code", "Paste the avatar URL received from Ready Player Me here."),
                inputFieldWidth);
            url = EditorGUILayout.TextField(url, fieldHeight);
            EditorPrefs.SetString(URL_SAVE_KEY, url);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawOptions()
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                new GUIContent("Use Eye Animations",
                    "Optional helper component for random eye rotation and blinking, for a less static look."),
                inputFieldWidth);
            useEyeAnimations = EditorGUILayout.Toggle(useEyeAnimations, fieldHeight);
            EditorPrefs.SetBool(EYE_ANIMATION_SAVE_KEY, useEyeAnimations);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                new GUIContent("Use Voice To Animation",
                    "Optional helper component for voice amplitude to jaw bone movement."), inputFieldWidth);
            useVoiceToAnim = EditorGUILayout.Toggle(useVoiceToAnim, fieldHeight);
            EditorPrefs.SetBool(VOICE_TO_ANIM_SAVE_KEY, useVoiceToAnim);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawLoadAvatarButton()
        {
            GUI.enabled = !string.IsNullOrEmpty(url);
            if (GUILayout.Button("Load Avatar", avatarButtonStyle))
            {
                AnalyticsEditorLogger.EventLogger.LogLoadAvatarFromDialog(url, useEyeAnimations, useVoiceToAnim);
                var avatarLoader = new AvatarLoader();
                avatarLoader.SaveInProjectFolder = true;
                avatarLoader.OnFailed += Failed;
                avatarLoader.OnCompleted += Completed;
                avatarLoader.LoadAvatar(url);
            }

            GUI.enabled = true;
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
