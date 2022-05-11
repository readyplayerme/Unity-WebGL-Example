using UnityEngine;
using UnityEngine.UI;

public class DebugLogger : MonoBehaviour
{
    [SerializeField] private Text debugText;
    public void LogMessage(string message)
    {
        debugText.text = message;
    }
}
