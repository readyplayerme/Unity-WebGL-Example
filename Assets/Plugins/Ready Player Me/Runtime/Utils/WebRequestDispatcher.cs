using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ReadyPlayerMe
{
    public class WebRequestDispatcher
    {
        private const int TIMEOUT = 20;
        private const string LAST_MODIFIED = "Last-Modified";
        private const string NO_INTERNET_CONNECTION = "No internet connection.";
        private const string CLOUDFRONT_IDENTIFIER = "cloudfront";

        public Action<float> ProgressChanged;

        private bool HasInternetConnection => Application.internetReachability != NetworkReachability.NotReachable;

        public async Task<string> Dispatch(string url, byte[] bytes, CancellationToken token)
        {
            if (HasInternetConnection)
            {
                using (var request = UnityWebRequest.Put(url, bytes))
                {
                    request.method = "POST";
                    request.SetRequestHeader("Content-Type", "application/json");

                    var asyncOperation = request.SendWebRequest();
                    while (!asyncOperation.isDone && !token.IsCancellationRequested)
                    {
                        await Task.Yield();
                        ProgressChanged?.Invoke(request.downloadProgress);
                    }

                    token.ThrowCustomExceptionIfCancellationRequested();

                    if (request.isHttpError || request.isNetworkError)
                    {
                        throw new CustomException(FailureType.DownloadError, request.error);
                    }

                    return request.downloadHandler.text;
                }
            }

            throw new CustomException(FailureType.NoInternetConnection, NO_INTERNET_CONNECTION);
        }

        public async Task<Response> DownloadIntoMemory(string url, CancellationToken token, int timeout = TIMEOUT)
        {
            if (HasInternetConnection)
            {
                using (var request = new UnityWebRequest(url))
                {
                    request.timeout = timeout;
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.method = UnityWebRequest.kHttpVerbGET;

                    if (!url.Contains(CLOUDFRONT_IDENTIFIER)) // Required to prevent CORS errors in WebGL
                    {
                        foreach (var header in CommonHeaders.GetRequestHeaders())
                        {
                            request.SetRequestHeader(header.Key, header.Value);
                        }
                    }

                    var asyncOperation = request.SendWebRequest();
                    while (!asyncOperation.isDone && !token.IsCancellationRequested)
                    {
                        await Task.Yield();
                        ProgressChanged?.Invoke(request.downloadProgress);
                    }

                    token.ThrowCustomExceptionIfCancellationRequested();

                    if (request.downloadedBytes == 0 || request.isHttpError || request.isNetworkError)
                    {
                        throw new CustomException(FailureType.DownloadError, request.error);
                    }

                    return new Response(
                        request.downloadHandler.text,
                        request.downloadHandler.data,
                        request.GetResponseHeader(LAST_MODIFIED));
                }
            }

            throw new CustomException(FailureType.NoInternetConnection, NO_INTERNET_CONNECTION);
        }

        public async Task<Response> DownloadIntoFile(string url, string path, CancellationToken token, int timeout = TIMEOUT)
        {
            if (HasInternetConnection)
            {
                using (var request = new UnityWebRequest(url))
                {
                    request.timeout = timeout;
                    var downloadHandler = new DownloadHandlerFile(path);
                    downloadHandler.removeFileOnAbort = true;
                    request.downloadHandler = downloadHandler;

                    if (!url.Contains(CLOUDFRONT_IDENTIFIER)) // Required to prevent CORS errors in WebGL
                    {
                        foreach (var header in CommonHeaders.GetRequestHeaders())
                        {
                            request.SetRequestHeader(header.Key, header.Value);
                        }
                    }

                    var asyncOperation = request.SendWebRequest();
                    while (!asyncOperation.isDone && !token.IsCancellationRequested)
                    {
                        await Task.Yield();
                        ProgressChanged?.Invoke(request.downloadProgress);
                    }

                    token.ThrowCustomExceptionIfCancellationRequested();

                    if (request.downloadedBytes == 0 || request.isHttpError || request.isNetworkError)
                    {
                        throw new CustomException(FailureType.DownloadError, request.error);
                    }

                    var byteLength = (long) request.downloadedBytes;
                    var info = new FileInfo(path);

                    while (info.Length != byteLength)
                    {
                        info.Refresh();
                        await Task.Yield();
                    }

                    // Reading file since can't access raw bytes from downloadHandler
                    var bytes = File.ReadAllBytes(path);

                    return new Response(
                        string.Empty,
                        bytes,
                        request.GetResponseHeader(LAST_MODIFIED));
                }
            }

            throw new CustomException(FailureType.NoInternetConnection, NO_INTERNET_CONNECTION);
        }

        public async Task<Texture2D> DownloadTexture(string url, CancellationToken token)
        {
            if (HasInternetConnection)
            {
                using (var request = UnityWebRequestTexture.GetTexture(url))
                {
                    var asyncOperation = request.SendWebRequest();
                    while (!asyncOperation.isDone && !token.IsCancellationRequested)
                    {
                        await Task.Yield();
                        ProgressChanged?.Invoke(request.downloadProgress);
                    }

                    token.ThrowCustomExceptionIfCancellationRequested();

                    if (request.isNetworkError || request.isHttpError)
                    {
                        throw new CustomException(FailureType.DownloadError, request.error);
                    }

                    if (request.downloadHandler is DownloadHandlerTexture downloadHandlerTexture)
                    {
                        return downloadHandlerTexture.texture;
                    }
                }
            }

            throw new CustomException(FailureType.NoInternetConnection, NO_INTERNET_CONNECTION);
        }
    }
}
