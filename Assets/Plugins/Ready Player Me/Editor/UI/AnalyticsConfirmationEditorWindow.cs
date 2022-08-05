using ReadyPlayerMe;
using ReadyPlayerMe.Analytics;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AnalyticsConfirmationEditorWindow : EditorWindow
{
    private const string HEADING = "Help us improve Ready Player Me SDK";
    private const string DESCRIPTION =
        "We are constantly adding new features and improvements to our SDK. Enable analytics and help us in building even better free tools for more developers. This data is used for internal purposes only and is not shared with third parties.";
    private const string ANALYTICS_PRIVACY_TEXT = "Read our Privacy Policy and learn how we use the data <b>here</b>.";
    private const string ANALYTICS_PRIVACY_URL =
        "https://docs.readyplayer.me/ready-player-me/integration-guides/unity/help-us-improve-the-unity-sdk";
    private const string NOT_A_FIRST_RUN = "FirstRun";
    private const string METRICS_NEVER_ASK_AGAIN = "NeverAskAgain";

    private const string EDITOR_WINDOW_NAME = "allow analytics popup";

    private static readonly Vector2 WindowSize = new Vector2Int(512, 328);
    private static bool neverAskAgain;

    private readonly GUILayoutOption fieldHeight = GUILayout.Height(20);
    private GUIStyle headingStyle;
    private GUIStyle descriptionStyle;
    private GUIStyle buttonStyle;

    private bool variablesLoaded;

    private Banner banner;

    static AnalyticsConfirmationEditorWindow()
    {
        EntryPoint.Startup += OnStartup;
    }

    private static void OnStartup()
    {
        if (!EditorPrefs.GetBool(METRICS_NEVER_ASK_AGAIN) && !AnalyticsEditorLogger.IsEnabled)
        {
            ShowWindowMenu();
        }

        if (AnalyticsEditorLogger.IsEnabled)
        {
            AnalyticsEditorLogger.EventLogger.IdentifyUser();
            AnalyticsEditorLogger.EventLogger.LogOpenProject();
            EditorApplication.quitting += OnQuit;
        }
    }

    private static void OnQuit()
    {
        AnalyticsEditorLogger.EventLogger.LogCloseProject();
    }

    private static void ShowWindowMenu()
    {
        var window = (AnalyticsConfirmationEditorWindow) GetWindow(typeof(AnalyticsConfirmationEditorWindow));
        window.titleContent = new GUIContent("Analytics Confirmation");
        window.minSize = window.maxSize = WindowSize;
        window.ShowUtility();

        AnalyticsEditorLogger.EventLogger.LogOpenDialog(EDITOR_WINDOW_NAME);
    }

    private void OnDestroy()
    {
        EntryPoint.Startup -= OnStartup;
        if (EditorPrefs.GetBool(NOT_A_FIRST_RUN)) return;
        SettingsEditorWindow.ShowWindowMenu();
        EditorPrefs.SetBool(NOT_A_FIRST_RUN, true);
    }

    private void OnGUI()
    {
        if (!variablesLoaded) LoadCachedVariables();
        LoadStyles();
        DrawContent();
    }

    private void LoadCachedVariables()
    {
        neverAskAgain = EditorPrefs.GetBool(METRICS_NEVER_ASK_AGAIN);
        variablesLoaded = true;
    }

    private void LoadStyles()
    {
        if (headingStyle == null)
        {
            headingStyle = new GUIStyle()
            {
                fontSize = 14,
                richText = true,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(5, 0, 0, 0)
            };
            headingStyle.normal.textColor = Color.white;
        }

        if (descriptionStyle == null)
        {
            descriptionStyle = new GUIStyle()
            {
                fontSize = 12,
                richText = true,
                wordWrap = true,
                margin = new RectOffset(5, 0, 0, 0)
            };
            descriptionStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        }

        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 12;
            buttonStyle.fixedWidth = 251;
            buttonStyle.padding = new RectOffset(6, 6, 8, 8);
        }
    }

    private void DrawContent()
    {
        using (var _ = new CommonEditorLayout())
        {
            if (banner == null)
            {
                banner = new Banner();
            }

            banner.DrawBanner(position.size.x, false);

            GUILayout.Space(10);
            GUILayout.Label(HEADING, headingStyle);

            GUILayout.Space(10);
            GUILayout.Label(DESCRIPTION, descriptionStyle);
            GUILayout.Space(5);
            if (GUILayout.Button(ANALYTICS_PRIVACY_TEXT, descriptionStyle))
            {
                Application.OpenURL(ANALYTICS_PRIVACY_URL);
            }

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(100));

            neverAskAgain = EditorGUILayout.Toggle(neverAskAgain, fieldHeight);
            GUILayout.Label("Never Ask Again");
            EditorPrefs.SetBool(METRICS_NEVER_ASK_AGAIN, neverAskAgain);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Don't Enable Analytics", buttonStyle))
            {
                AnalyticsEditorLogger.Disable();
                Close();
            }
            if (GUILayout.Button("Enable Analytics", buttonStyle))
            {
                AnalyticsEditorLogger.Enable();
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
