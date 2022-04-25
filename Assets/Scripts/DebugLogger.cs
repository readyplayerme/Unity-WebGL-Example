using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogger : MonoBehaviour
{
    public void OnShowRpm()
    {
        WebInterface.SetIFrameVisibility(true);
    }

    [SerializeField] private Text debugText;
    public void LogMessage(string message)
    {
        debugText.text = message;
    }
}
