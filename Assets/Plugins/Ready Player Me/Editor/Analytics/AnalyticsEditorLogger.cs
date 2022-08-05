using UnityEditor;

namespace ReadyPlayerMe.Analytics
{
    public static class AnalyticsEditorLogger
    {
        private const string ANALYTICS_LOGGING_ACCEPTED = "AnalyticsLoggingAccepted";

        public static readonly IAnalyticsEventLogger EventLogger;
        public static bool IsEnabled { get; private set; }

        static AnalyticsEditorLogger()
        {
            IsEnabled = EditorPrefs.GetBool(ANALYTICS_LOGGING_ACCEPTED);
            EventLogger = new AnalyticsEventLogger(IsEnabled);
        }

        public static void Enable()
        {
            IsEnabled = true;
            EventLogger.Enable();
            EditorPrefs.SetBool(ANALYTICS_LOGGING_ACCEPTED, true);
        }

        public static void Disable()
        {
            EventLogger.Disable();
            IsEnabled = false;
            EditorPrefs.SetBool(ANALYTICS_LOGGING_ACCEPTED, false);
        }
    }
}
