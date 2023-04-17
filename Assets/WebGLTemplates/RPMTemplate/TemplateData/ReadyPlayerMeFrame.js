rpmHideButton.onclick = function () {
    if (document.fullscreenElement) {
        canvasWrapper.requestFullscreen();
    }
    rpmContainer.style.display = "none";
};


function setupRpmFrame(url, targetGameObjectName) {
    const message = "message";
    const rpmFilter = "readyplayerme";
    const frameReadyEvent = "v1.frame.ready";
    const receivingFunctionName = "FrameMessageReceived";
    
    rpmFrame.src = url;
    window.addEventListener(message, subscribe);
    document.addEventListener(message, subscribe);
    
    function subscribe(event) {
        const json = parse(event);
        if (
            unityGame == null ||
            json?.source !== rpmFilter ||
            json?.eventName == null
        ) {
            return;
        }

        unityGame.SendMessage(
            targetGameObjectName,
            receivingFunctionName,
            event.data
        );
        
        // Subscribe to all events sent from Ready Player Me once frame is ready
        if (json.eventName === frameReadyEvent) {
            rpmFrame.contentWindow.postMessage(
                JSON.stringify({
                    target: rpmFilter,
                    type: "subscribe",
                    eventName: "v1.**",
                }),
                "*"
            );
        }
    }

    function parse(event) {
        try {
            return JSON.parse(event.data);
        } catch (error) {
            return null;
        }
    }
}

function showRpm() {
    rpmContainer.style.display = "block";
}

function hideRpm() {
    rpmContainer.style.display = "none";
}
