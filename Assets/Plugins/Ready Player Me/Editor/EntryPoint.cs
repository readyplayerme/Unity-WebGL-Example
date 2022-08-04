using System;
using UnityEditor;

namespace ReadyPlayerMe
{
    [InitializeOnLoad]
    public static class EntryPoint
    {
        /// <summary>
        /// Event for when package is imported or when project with package is opened.
        /// </summary>
        public static Action Startup;

        private const string SESSION_STARTED_KEY = "SessionStarted";

        static EntryPoint()
        {
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            if (SessionState.GetBool(SESSION_STARTED_KEY, false)) return;
            SessionState.SetBool(SESSION_STARTED_KEY, true);
            Startup?.Invoke();
        }
    }
}
