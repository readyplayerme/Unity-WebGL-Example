using System;
using Newtonsoft.Json;
using ReadyPlayerMe.WebView;
using UnityEngine;

namespace ReadyPlayerMe.Examples.WebGL
{
    public class FrameEventHandler
    {
        public Action<string> OnAvatarExport;
        public Action<string> OnUserSet;
        public Action<string> OnUserAuthorized;

        public void MessageReceived(string message)
        {
            var webMessage = JsonConvert.DeserializeObject<WebMessage>(message);
            switch (webMessage.eventName)
            {
                case WebViewEventNames.AVATAR_EXPORT:
                    Debug.Log(webMessage.eventName);
                    OnAvatarExport?.Invoke(webMessage.GetAvatarUrl());
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
