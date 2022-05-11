using System.Runtime.InteropServices;

public static class WebInterface
{

    [DllImport("__Internal")]
    private static extern void SetupRpm(string partner);
    
    [DllImport("__Internal")]
    private static extern void ShowReadyPlayerMeFrame();
    
    [DllImport("__Internal")]
    private static extern void HideReadyPlayerMeFrame();

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

    public static void SetupRpmFrame(string partner)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        SetupRpm(partner);
#endif
    }

}
