function setupUnity(loaderUrl, buildUrl){

    var script = document.createElement("script");
    script.src = loaderUrl;
    script.onload = () => {
        unityGame = UnityLoader.instantiate("unity-container", buildUrl, {onProgress: UnityProgress});
    };
    document.body.appendChild(script);
}
