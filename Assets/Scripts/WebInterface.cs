using System.Runtime.InteropServices;

public static class WebInterface
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SetupRpm(string partner);
    
    [DllImport("__Internal")]
    private static extern void ShowReadyPlayerMeFrame();
    
    [DllImport("__Internal")]
    private static extern void HideReadyPlayerMeFrame();

    public static void SetIFrameVisibility(bool isVisible)
    {

        if (isVisible)
        {
            
            ShowReadyPlayerMeFrame();
            return;
        }

        HideReadyPlayerMeFrame();

    }
    public static void SetupRpmFrame(string partner)
    {
        SetupRpm(partner);

    }
#endif
}
