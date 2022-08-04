using System.Collections.Generic;
using System.Text;
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

        private long sessionId;

        public AmplitudeEventLogger()
        {
            appData = ApplicationData.GetData();
            dispatcher = new WebRequestDispatcher();
        }

        public void SetSessionId(long id)
        {
            sessionId = id;
        }

        public void SetUserProperties()
        {
            var userProperties = new Dictionary<string, string>
            {
                { Constants.Properties.ENGINE_VERSION, appData.UnityVersion },
                { Constants.Properties.RENDER_PIPELINE, appData.RenderPipeline },
                { Constants.Properties.SUBDOMAIN, appData.PartnerName },
                { Constants.Properties.APP_NAME, PlayerSettings.productName },
                { Constants.Properties.SDK_TARGET, "Unity" },
                { Constants.Properties.APP_IDENTIFIER, Application.identifier }
            };

            LogEvent(Constants.EventName.SET_USER_PROPERTIES, null, userProperties);
        }


        public void LogEvent(string eventName, Dictionary<string, object> eventProperties = null, Dictionary<string, string> userProperties = null)
        {
            var eventData = new Dictionary<string, object>
            {
                { Constants.AmplitudeKeys.DEVICE_ID, SystemInfo.deviceUniqueIdentifier },
                { Constants.AmplitudeKeys.EVENT_TYPE, eventName },
                { Constants.AmplitudeKeys.PLATFORM, appData.UnityPlatform },
                { Constants.AmplitudeKeys.SESSION_ID, sessionId },
                { Constants.AmplitudeKeys.APP_VERSION, appData.SDKVersion }
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
                events = new[]
                {
                    eventData
                }
            };

            var json = JsonConvert.SerializeObject(payload);
            var bytes = Encoding.UTF8.GetBytes(json);

            dispatcher.Dispatch(ENDPOINT, bytes).Run();
        }
    }
}
