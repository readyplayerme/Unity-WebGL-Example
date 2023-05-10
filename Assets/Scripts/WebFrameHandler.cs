using System;
using Newtonsoft.Json;
using ReadyPlayerMe.WebView;
using UnityEngine;

namespace ReadyPlayerMe.Examples.WebGL
{
    public enum AutoInitialize
    {
        OnStart,
        OnAwake,
        None
    }
    
    public class WebFrameHandler: MonoBehaviour
    {
        public Action<string> OnAvatarExport;
        public Action<string> OnUserSet;
        public Action<string> OnUserAuthorized;
        
        [SerializeField] private AutoInitialize autoInitialize = AutoInitialize.OnStart;
        [SerializeField] private UrlConfig urlConfig;
        
        private void Awake()
        {
            if(autoInitialize == AutoInitialize.OnAwake)
            {
                Setup();
            }
        }
        
        private void Start()
        {
            if (autoInitialize == AutoInitialize.OnStart)
            {
                Setup();
            }
        }

        public void Setup(string loginToken = "")
        {
            WebInterface.SetupRpmFrame(urlConfig.BuildUrl(loginToken), name);
        }
        
        public void LoadWithLoginToken(string loginToken)
        {
            Setup(loginToken);
        }

        /// <summary>
        /// This message is received from the RPM iFrame Javascript in the RPM WebGL Template.
        /// </summary>
        /// <param name="message">The message will contain data pass from the iFrame as JSON</param>
        // ReSharper disable once UnusedMember.Global
        public void FrameMessageReceived(string message)
        {
            var webMessage = JsonConvert.DeserializeObject<WebMessage>(message);
            switch (webMessage.eventName)
            {
                case WebViewEventNames.AVATAR_EXPORT:
                    Debug.Log(webMessage.eventName);
                    OnAvatarExport?.Invoke(webMessage.GetAvatarUrl());
                    WebInterface.SetIFrameVisibility(false);
                    break;
                case WebViewEventNames.USER_SET:
                    Debug.Log(webMessage.eventName);
                    OnUserSet?.Invoke(webMessage.GetUserId());
                    break;
                case WebViewEventNames.USER_AUTHORIZED:
                    Debug.Log(webMessage.eventName);
                    OnUserAuthorized?.Invoke(webMessage.GetUserId());
                    break;
            }
        }
    }
}
