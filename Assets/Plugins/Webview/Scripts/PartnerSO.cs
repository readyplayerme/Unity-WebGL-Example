using UnityEngine;

public class PartnerSO : ScriptableObject
{
    public string Subdomain = null;

    public string GetUrl(bool keepSessionAlive = true)
    {
        string cacheParam = keepSessionAlive ? "" : "&clearCache";
        return $"https://{GetSubdomain()}.readyplayer.me/avatar?frameApi{ cacheParam }";
    }

    public string GetSubdomain()
    {
        if (string.IsNullOrEmpty(Subdomain)) Subdomain = "demo";
        return Subdomain;
    }
}
