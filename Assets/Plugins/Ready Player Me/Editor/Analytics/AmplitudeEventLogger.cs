using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Analytics
{
    public class AmplitudeEventLogger
    {
        private const string ENDPOINT = "https://analytics-sdk.readyplayer.me/";

        private readonly AppData appData;
        private readonly WebRequestDispatcher dispatcher;
        private readonly AnalyticsTarget target;

        private long sessionId;

        public AmplitudeEventLogger()
        {
            appData = ApplicationData.GetData();
            dispatcher = new WebRequestDispatcher();
            target = Resources.Load<AnalyticsTarget>(RESOURCE_PATH);
        }

        public void SetSessionId(long id)
        {
            sessionId = id;
        }

        public bool IsSessionIdSet()
        {
            return sessionId != 0;
        }

        public void SetUserProperties()
        {
            var userProperties = new Dictionary<string, object>
            {
                { Constants.Properties.ENGINE_VERSION, appData.UnityVersion },
                { Constants.Properties.RENDER_PIPELINE, appData.RenderPipeline },
                { Constants.Properties.SUBDOMAIN, appData.PartnerName },
                { Constants.Properties.APP_NAME, PlayerSettings.productName },
                { Constants.Properties.SDK_TARGET, "Unity" },
                { Constants.Properties.APP_IDENTIFIER, Application.identifier },
                { Constants.Properties.ALLOW_ANALYTICS, true }
            };

            LogEvent(Constants.EventName.SET_USER_PROPERTIES, null, userProperties);
        }


        public async void LogEvent(string eventName, Dictionary<string, object> eventProperties = null, Dictionary<string, object> userProperties = null)
        {
            var eventData = new Dictionary<string, object>
            {
                { Constants.AmplitudeKeys.DEVICE_ID, SystemInfo.deviceUniqueIdentifier },
                { Constants.AmplitudeKeys.EVENT_TYPE, eventName },
                { Constants.AmplitudeKeys.PLATFORM, appData.UnityPlatform },
                { Constants.AmplitudeKeys.SESSION_ID, sessionId },
                { Constants.AmplitudeKeys.APP_VERSION, appData.SDKVersion },
                { Constants.AmplitudeKeys.OPERATING_SYSTEM, SystemInfo.operatingSystem }
            };

            if (userProperties != null)
            {
                eventData.Add(Constants.AmplitudeKeys.USER_PROPERTIES, userProperties);
            }

            if (eventProperties != null)
            {
                eventData.Add(Constants.AmplitudeKeys.EVENT_PROPERTIES, eventProperties);
            }

            var payload = new
            {
                target = GetAnalyticsTarget(),
                events = new[]
                {
                    eventData
                }
            };

            var json = JsonConvert.SerializeObject(payload);
            var bytes = Encoding.UTF8.GetBytes(json);

            try
            {
                await dispatcher.Dispatch(ENDPOINT, bytes, new CancellationToken());
            }
            catch (Exception exception)
            {
                SDKLogger.Log(nameof(AmplitudeEventLogger), exception);
            }
        }

        #region Analytics Target

        private const string PRODUCTION = "unity";
        private const string DEVELOPMENT = "unity-dev";
        private const string RESOURCE_PATH = "Data/Analytics Target";

        private string GetAnalyticsTarget()
        {
            if (target == null)
            {
                return PRODUCTION;
            }

            switch (target.Target)
            {
                case Target.Development:
                    return DEVELOPMENT;
                case Target.Production:
                    return PRODUCTION;
                default:
                    return string.Empty;
            }
        }

        #endregion
    }
}
