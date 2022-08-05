using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe
{

    public class WebRequestDispatcher
    {
        private const int TIMEOUT = 20;
        private const string LAST_MODIFIED = "Last-Modified";

        public Action<float> OnProgressChanged;
        public Action<Response> OnCompleted;
        public Action<string> OnFailed;

        private bool HasInternetConnection => Application.internetReachability != NetworkReachability.NotReachable;

        public IEnumerator Dispatch(string url, byte[] bytes)
        {
            if (HasInternetConnection)
            {
                using (var request = UnityWebRequest.Put(url, bytes))
                {
                    request.method = "POST";
                    request.SetRequestHeader("Content-Type", "application/json");

                    yield return request.SendWebRequest();
                }
            }
            else
            {
                OnFailed?.Invoke("No internet connection.");
            }
        }

        public IEnumerator Dispatch(string url, List<IMultipartFormSection> form)
        {
            if (HasInternetConnection)
            {
                using (var request = UnityWebRequest.Post(url, form))
                {
                    yield return request.SendWebRequest();
                }
            }
            else
            {
                OnFailed?.Invoke("No internet connection.");
            }
        }

        public IEnumerator DownloadIntoMemory(string url)
        {
            if (HasInternetConnection)
            {
                using (var request = new UnityWebRequest(url))
                {
                    request.timeout = TIMEOUT;
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.method = UnityWebRequest.kHttpVerbGET;

                    var op = request.SendWebRequest();

                    while (!op.isDone)
                    {
                        yield return null;
                        OnProgressChanged?.Invoke(request.downloadProgress);
                    }

                    if (request.downloadedBytes == 0 || request.isHttpError || request.isNetworkError)
                    {
                        OnFailed?.Invoke(request.error);
                    }
                    else
                    {
                        OnCompleted?.Invoke(new Response(
                            request.downloadHandler.text,
                            request.downloadHandler.data,
                            request.GetResponseHeader(LAST_MODIFIED)));
                    }
                }
            }
            else
            {
                OnFailed?.Invoke("No internet connection.");
            }
        }

        public IEnumerator DownloadIntoFile(string url, string path)
        {
            if (HasInternetConnection)
            {
                using (var request = new UnityWebRequest(url))
                {
                    request.timeout = TIMEOUT;
                    request.downloadHandler = new DownloadHandlerFile(path);

                    var op = request.SendWebRequest();

                    while (!op.isDone)
                    {
                        yield return null;
                        OnProgressChanged?.Invoke(request.downloadProgress);
                    }

                    if (request.downloadedBytes == 0 || request.isHttpError || request.isNetworkError)
                    {
                        OnFailed?.Invoke(request.error);
                    }
                    else
                    {
                        var byteLength = (long) request.downloadedBytes;
                        var info = new FileInfo(path);

                        while (info.Length != byteLength)
                        {
                            info.Refresh();
                            yield return null;
                        }

                        // Reading file since can't access raw bytes from downloadHandler
                        var bytes = File.ReadAllBytes(path);
                        OnCompleted?.Invoke(new Response(
                            string.Empty,
                            bytes,
                            request.GetResponseHeader(LAST_MODIFIED)));
                    }
                }
            }
            else
            {
                OnFailed?.Invoke("No internet connection.");
            }
        }
    }
}
