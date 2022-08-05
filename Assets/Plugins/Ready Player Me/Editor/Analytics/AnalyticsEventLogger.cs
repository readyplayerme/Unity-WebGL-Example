using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Analytics
{
    public class AnalyticsEventLogger : IAnalyticsEventLogger
    {
        private readonly AmplitudeEventLogger amplitudeEventLogger;

        private bool isEnabled;

        public AnalyticsEventLogger(bool isEnabled)
        {
            amplitudeEventLogger = new AmplitudeEventLogger();
            this.isEnabled = isEnabled;
        }

        public void Enable()
        {
            isEnabled = true;
            GenerateSessionId();
            AllowLogging(true);
        }

        public void Disable()
        {
            AllowLogging(false);
            isEnabled = false;
        }

        public void IdentifyUser()
        {
            if (!isEnabled) return;
            amplitudeEventLogger.SetUserProperties();
        }

        public void LogOpenProject()
        {
            if (!isEnabled) return;
            GenerateSessionId();
            amplitudeEventLogger.LogEvent(Constants.EventName.OPEN_PROJECT);
        }

        public void LogCloseProject()
        {
            if (!isEnabled) return;
            amplitudeEventLogger.LogEvent(Constants.EventName.CLOSE_PROJECT);
        }

        public void LogOpenDocumentation(string target)
        {
            if (!isEnabled) return;
            amplitudeEventLogger.LogEvent(Constants.EventName.OPEN_DOCUMENTATION, new Dictionary<string, object>()
            {
                { Constants.Properties.TARGET, target }
            });
        }

        public void LogOpenFaq(string target)
        {
            if (!isEnabled) return;
            amplitudeEventLogger.LogEvent(Constants.EventName.OPEN_FAQ, new Dictionary<string, object>()
            {
                { Constants.Properties.TARGET, target }
            });
        }

        public void LogOpenDiscord(string target)
        {
            if (!isEnabled) return;
            amplitudeEventLogger.LogEvent(Constants.EventName.OPEN_DISCORD, new Dictionary<string, object>()
            {
                { Constants.Properties.TARGET, target }
            });
        }

        public void LogLoadAvatarFromDialog(string avatarUrl, bool eyeAnimation, bool voiceHandler)
        {
            if (!isEnabled) return;
            amplitudeEventLogger.LogEvent(Constants.EventName.LOAD_AVATAR_FROM_DIALOG, new Dictionary<string, object>()
            {
                { Constants.Properties.AVATAR_URL, avatarUrl },
                { Constants.Properties.EYE_ANIMATION, eyeAnimation },
                { Constants.Properties.VOICE_HANDLER, voiceHandler }
            });
        }

        public void LogUpdatePartnerURL(string previousSubdomain, string newSubdomain)
        {
            if (!isEnabled) return;
            amplitudeEventLogger.LogEvent(Constants.EventName.UPDATED_PARTNER_URL, new Dictionary<string, object>()
            {
                { Constants.Properties.PREVIOUS_SUBDOMAIN, previousSubdomain },
                { Constants.Properties.NEW_SUBDOMAIN, newSubdomain }
            }, new Dictionary<string, string>()
            {
                { Constants.Properties.SUBDOMAIN, newSubdomain }
            });
        }


        public void LogOpenDialog(string dialog)
        {
            if (!isEnabled) return;
            amplitudeEventLogger.LogEvent(Constants.EventName.OPEN_DIALOG, new Dictionary<string, object>()
            {
                { Constants.Properties.DIALOG, dialog }
            });
        }

        public void LogBuildApplication(string target, string appName, bool productionBuild)
        {
            if (!isEnabled) return;
            amplitudeEventLogger.LogEvent(Constants.EventName.BUILD_APPLICATION, new Dictionary<string, object>()
            {
                { Constants.Properties.TARGET, target },
                { Constants.Properties.APP_NAME, appName },
                { Constants.Properties.PRODUCTION_BUILD, productionBuild },
                { Constants.Properties.APP_IDENTIFIER, Application.identifier }
            });
        }

        private void GenerateSessionId() => amplitudeEventLogger.SetSessionId(DateTimeOffset.Now.ToUnixTimeMilliseconds());

        private void AllowLogging(bool allow)
        {
            if (!isEnabled) return;
            var appData = ApplicationData.GetData();
            amplitudeEventLogger.LogEvent(Constants.EventName.ALLOW_LOGGING, new Dictionary<string, object>()
            {
                { Constants.Properties.ALLOW, allow }
            }, new Dictionary<string, string>
            {
                { Constants.Properties.ENGINE_VERSION, appData.UnityVersion },
                { Constants.Properties.RENDER_PIPELINE, appData.RenderPipeline },
                { Constants.Properties.SUBDOMAIN, appData.PartnerName },
                { Constants.Properties.APP_NAME, PlayerSettings.productName },
                { Constants.Properties.SDK_TARGET, "Unity" },
                { Constants.Properties.APP_IDENTIFIER, Application.identifier }
            });
        }
    }
}
