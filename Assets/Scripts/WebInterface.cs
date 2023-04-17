#if !UNITY_EDITOR && UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public static class WebInterface
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SetupRpm(string partner, string targetGameObjectName = "");
    
    [DllImport("__Internal")]
    private static extern void ShowReadyPlayerMeFrame();
    
    [DllImport("__Internal")]
    private static extern void HideReadyPlayerMeFrame();
#endif
    public static void SetIFrameVisibility(bool isVisible)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        if (isVisible)
        {
            ShowReadyPlayerMeFrame();
            return;
        }

        HideReadyPlayerMeFrame();
#endif
    }
    public static void SetupRpmFrame(string url, string targetGameObjectName)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        SetupRpm(url,  targetGameObjectName);
#endif
    }
}
