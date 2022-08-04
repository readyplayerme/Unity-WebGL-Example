using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReadyPlayerMe
{
    public static class ApplicationData
    {
        private const string SDK_VERSION = "v1.11.0";
        private const string TAG = "ApplicationData";
        private const string DEFAULT_RENDER_PIPELINE = "Built-In Render Pipeline";
        private static readonly AppData Data;

        static ApplicationData()
        {
            Data.SDKVersion = SDK_VERSION;
            Data.PartnerName = GetPartnerSubdomain();
            Data.UnityVersion = Application.unityVersion;
            Data.UnityPlatform = Application.platform.ToString();
            Data.RenderPipeline = GetRenderPipeline();
#if UNITY_EDITOR
            Data.BuildTarget = EditorUserBuildSettings.activeBuildTarget.ToString();
#endif
        }

        private static string GetPartnerSubdomain()
        {
            ScriptableObject partner = Resources.Load<ScriptableObject>("Partner");
            Type type = partner.GetType();
            var method = type.GetMethod("GetSubdomain");
            string partnerSubdomain = method?.Invoke(partner, null) as string;
            return partnerSubdomain;
        }

        private static string GetRenderPipeline()
        {
            string renderPipeline = GraphicsSettings.currentRenderPipeline == null
                ? DEFAULT_RENDER_PIPELINE
                : GraphicsSettings.currentRenderPipeline.name;
            return renderPipeline;
        }

        public static void Log()
        {
            SDKLogger.Log(TAG, $"Partner Subdomain: <color=green>{Data.PartnerName}</color>");
            SDKLogger.Log(TAG, $"SDK Version: <color=green>{Data.SDKVersion}</color>");
            SDKLogger.Log(TAG, $"Unity Version: <color=green>{Data.UnityVersion}</color>");
            SDKLogger.Log(TAG, $"Unity Platform: <color=green>{Data.UnityPlatform}</color>");
            SDKLogger.Log(TAG, $"Unity Render Pipeline: <color=green>{Data.RenderPipeline}</color>");
#if UNITY_EDITOR
            SDKLogger.Log(TAG, $"Unity Build Target: <color=green>{Data.BuildTarget}</color>");
#endif
        }

        public static AppData GetData()
        {
            return Data;
        }
    }
}
