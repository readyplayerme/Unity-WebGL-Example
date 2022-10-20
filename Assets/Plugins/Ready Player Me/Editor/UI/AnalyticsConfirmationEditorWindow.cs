using ReadyPlayerMe;
using ReadyPlayerMe.Analytics;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AnalyticsConfirmationEditorWindow : EditorWindowBase
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

    private static bool neverAskAgain;

    private readonly GUILayoutOption toggleWidth = GUILayout.Width(20);
    private GUIStyle descriptionStyle;
    private GUIStyle buttonStyle;

    private bool variablesLoaded;

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
            AnalyticsEditorLogger.EventLogger.LogOpenProject();
            AnalyticsEditorLogger.EventLogger.IdentifyUser();
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
        DrawContent(DrawContent, false);
    }

    private void LoadCachedVariables()
    {
        neverAskAgain = EditorPrefs.GetBool(METRICS_NEVER_ASK_AGAIN);
        variablesLoaded = true;
    }

    private void LoadStyles()
    {
        if (descriptionStyle == null)
        {
            descriptionStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                richText = true,
                wordWrap = true,
                fixedWidth = 450
            };
            descriptionStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        }

        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fontSize = 12;
            buttonStyle.padding = new RectOffset(5, 5, 5, 5);
            buttonStyle.fixedHeight = ButtonHeight;
            buttonStyle.fixedWidth = 225;
        }
    }

    private void DrawContent()
    {
        Vertical(() =>
        {
            GUILayout.Label(HEADING, HeadingStyle);

            GUILayout.Space(10);
            GUILayout.Label(DESCRIPTION, descriptionStyle);

            GUILayout.Space(10);
            if (GUILayout.Button(ANALYTICS_PRIVACY_TEXT, descriptionStyle))
            {
                Application.OpenURL(ANALYTICS_PRIVACY_URL);
            }

            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);

            GUILayout.Space(10);
            Horizontal(() =>
            {
                GUILayout.Space(4);
                neverAskAgain = EditorGUILayout.Toggle(neverAskAgain, toggleWidth);
                GUILayout.Label("Never Ask Again");
                GUILayout.FlexibleSpace();

                EditorPrefs.SetBool(METRICS_NEVER_ASK_AGAIN, neverAskAgain);
            });

            GUILayout.Space(10);
            Horizontal(() =>
            {
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
            });
        }, true);
    }
}
