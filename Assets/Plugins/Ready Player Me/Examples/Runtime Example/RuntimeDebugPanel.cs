using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe
{
    public class RuntimeDebugPanel : MonoBehaviour
    {
        private bool showDebugPanel = true;
        private bool pauseLogOutput;
        private Text logTextUI;
        private ScrollRect logScrollRect;
        private string currentLogOutput = "<color=green>Log Output Started...</color>\n";
        private int logCount;
        private const int MAX_LOGS = 100;

        private void Awake()
        {
            InitialiseDebugPanel();
            Application.logMessageReceived += HandleLog;
        }

        private void InitialiseDebugPanel()
        {
            logScrollRect = GetComponentInChildren<ScrollRect>(true);
            logTextUI = GetComponentInChildren<Text>(true);
            UpdateDebugPanel();
        }

        private void UpdateDebugPanel()
        {
            if (logTextUI) logTextUI.text = currentLogOutput;
            logCount++;
            ScrollToBottom();
        }

        public void ToggleShowDebugPanel()
        {
            showDebugPanel = !showDebugPanel;
        }

        public void TogglePauseLogOutput()
        {
            pauseLogOutput = !pauseLogOutput;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (logCount > MAX_LOGS) return;
            currentLogOutput += $"{(logCount < MAX_LOGS ? logString : "<color=yellow>Maximum number of logs reached. Logging suspended.</color>")}\n";
            if (showDebugPanel && !pauseLogOutput) UpdateDebugPanel();
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void ScrollToBottom()
        {
            if (logScrollRect) logScrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
