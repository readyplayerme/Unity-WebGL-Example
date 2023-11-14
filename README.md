# Ready Player Me Unity WebGL Integration Example DEPRECATED 

**Notice: This repository is now deprecated and no longer actively maintained.**

**The WebGL Example and all logic has been moved into our Ready Player Me Core package as a sample that can be imported from the inspector https://github.com/readyplayerme/rpm-unity-sdk-core/tree/main/Samples~/WebGLSample.**

This repository contains a Unity project that uses a custom WebGL template to embed the Ready Player Me character creator inside an application to seamlessly add avatars. This project can be used as a reference for anybody wanting to add Ready Player Me Avatars into their Unity WebGL application.

## Templates

![img-templates](https://user-images.githubusercontent.com/7085672/167348039-527638cb-203e-47bd-b754-6cc2123213a8.png)

***RPM_2019***

*We have removed the separate template for Unity 2019 as this version of Unity is no longer supported by Ready Player Me. If you are using Unity 2019 you can still copy the functionality you need from our RPMTemplate.*

**RPM_Template**

This is template was created to support Unity version 2020 or higher, making use of the latest features of Unity's WebGL packaging system.

## Quick Start 

Open the example project in Unity (version 2019 or higher) and open up the build settings window.

![img-build-settings](https://user-images.githubusercontent.com/7085672/167348062-269c55c0-497b-4ac8-90b9-57b65d5ab18c.png)

Make sure the example scene `Scenes/WebGLExample` is added at the top of the Scenes In Build section.

Then click on the `Player Settings..` in the bottom left corner. From there select Player from the menu on the left side and open the Resolution and Presentation section.

![img-player-settings](https://user-images.githubusercontent.com/7085672/167348092-22d6b37d-127c-4a2d-a0c7-06781867031f.png)

From here select the RPMTemplate (or RPM_2019 if using Unity version 2019).

Once that is done you can open up the Build Settings window again, make sure you have selected WebGL as the target platform then click Build And Run.

It will take some time to compile but once it is finished it should open up the WebGL application in your default browser.

# How It Works

## WebGL Template

To simplify things we will be looking at the RPM_Template as it is more relevant however the logic is very similar for the RPM_2019 template. 

As with all WebGL templates, there is an index.html which is based on the Default template that unity provides but with some extra wrappers (`canvas-wrap`) and an `<div id="rpm-container">`  that will hold our Ready Player me iFrame as you can see below. 

```html
<div id="canvas-wrap" >
    <!-- rpm-container div and contents required for RPM Web Creator -->
    <div id="rpm-container">
        <iframe id="rpm-frame" class="rpm-frame" allow="camera *; microphone *"></iframe>
        <button id="rpm-hide-button" onclick="hideRpm()">Hide</button>
    </div>
    <!-- rpm-container div and contents required for RPM Web Creator -->
    <canvas id="unity-canvas" ></canvas>
</div>
```

To keep things clean we separated the javascript logic into two separate files. There is UnitySetup.js which is based off the Unity Default template, and ReadyPlayerMeFrame.js which we created to handle the setup of the iFrame that will run [readyplayer.me](https://demo.readyplayer.me/avatar). 

## Ready Player Me Frame 

All the logic for setting up the Ready Player Me iFrame and subscribing to events can be found in `RPMTemplate/TemplateData/ReadyPlayerMe/ReadyPlayerMeFrame.js`

### Listen to website events

Before anything else, we add an event listener and bind it to a subscribe function so we can handle all events that the Ready Player Me website sends as you can see in the snippet below. 

```js
    window.addEventListener("message", subscribe);
    document.addEventListener("message", subscribe);
```
Then we set the src url for the iFrame

```js 
    rpmFrame.src = `https://${subdomain != "" ? subdomain : "demo"}.readyplayer.me/avatar?frameApi`;
```
 The subdomain value is loaded from the CoreSettings scriptable object. This can be set and saved using **Ready Player Me Settings window** accessed from the top toolbar at `Ready Player Me > Settings`. See the [Web Avatar Loader](/README##Web-Avatar-Loader) section to see how the subdomain value is loaded.

The next chunk of logic is inside the subscribe function, here we filter the events so that we only handle valid events as you can see by this conditional statement here. If the event source is invalid or is not `readyplayerme` then we ignore it. 

```js 
    const json = parse(event);
    if (
        unityGame == null ||
        json?.source !== "readyplayerme" ||
        json?.eventName == null
    ) {
        return;
    }
```


### Subscribing to Ready Player Me events

Next, we need to explicitly subscribe to our latest subscription service by listening for the `v1.frame.ready` event and sending a postMessage to trigger the subscription as you can see below. 

```js
    // Subscribe to all events sent from Ready Player Me once frame is ready
    if (json.eventName === "v1.frame.ready") {
        rpmFrame.contentWindow.postMessage(
            JSON.stringify({
                target: "readyplayerme",
                type: "subscribe",
                eventName: "v1.**",
            }),
            "*"
        );
    }
```

### Sending messages to Unity

To send messages from the Ready Player Me iFrame back to Unity we can use the `SendMessage()` function on the Unity instance inside Javascript. You can see this implemented in the snippet below. 

**Note the first two parameters of the `unityGame.SendMessage()` function are important and must match the names of the target game object and the desired function to call**

```js 
    // Get avatar GLB URL
    if (json.eventName === "v1.avatar.exported") {
        rpmContainer.style.display = "none";
        // Send message to a Gameobject in the current scene
        unityGame.SendMessage(
            "WebAvatarLoader", // Target GameObject name
            "OnWebViewAvatarGenerated", // Name of function to run
            json.data.url
        );
        console.log(`Avatar URL: ${json.data.url}`);
    }
```

As you can see we first check this `eventName` as we specifically want to handle it every time we get the `vr.avatar.exported` event (after an avatar has been created). Then we use the reference to the UnityInstance called `unityGame` and run the SendMessage function. As parameters we pass the following: 
- the name of the target object in the Unity scene that has the script and function we want to run (ReadyPlayerMeAvatar in this case)
- the name of the function we want to run (OnWebViewAvatarGenerated)
- the data we wish to pass, which is a URL in this case. 

### WebGL Helper

To communicate from Unity to the iFrame running Ready Player me we created a Javascript Library called WebHelper.jslib as shown below it has 3 functions.

```js
    mergeInto(LibraryManager.library, {
    
        ShowReadyPlayerMeFrame: function () {
            var rpmContainer = document.getElementById("rpm-container");
            rpmContainer.style.display = "block"; //show element
        },
    
        HideReadyPlayerMeFrame: function () {
            var rpmContainer = document.getElementById("rpm-container");
            rpmContainer.style.display = "none"; //hide element
        },
    
        SetupRpm: function (partner){
            setupRpmFrame(UTF8ToString(partner));
        },
    }); 
```

As the names suggest there are 2 for showing and hiding the Ready Player Me iFrame, and 1 for setting up the iFrame. By creating this jslib and putting it into the Plugins folder we can import these into Unity Scripts to use them as we have here in the [WebInterface](https://github.com/readyplayerme/Unity-WebGL-Example/blob/main/Assets/Scripts/WebInterface.cs) class.

```c#
    using System.Runtime.InteropServices;

    public static class WebInterface
    {
        [DllImport("__Internal")]
        private static extern void SetupRpm(string partner);

        [DllImport("__Internal")]
        private static extern void ShowReadyPlayerMeFrame();

        [DllImport("__Internal")]
        private static extern void HideReadyPlayerMeFrame();
    ...
```
Once declared this way they can be called like regular C# functions (on WebGL builds). 

## Web Avatar Loader

The [WebAvatarLoader.cs](https://github.com/readyplayerme/Unity-WebGL-Example/blob/main/Assets/Scripts/WebAvatarLoader.cs) is a simple class based on the RuntimeTest.cs example from our RPM SDK, it handles the loading of the avatar loading and setup of the Ready Player Me iFrame. 

**It is important to note that this script is added to an object called `WebAvatarLoader` and it has a function called `OnWebViewAvatarGenerated`. Both the name and the function are required so that it can receive the SendMessage function from the browser as mentioned previously [here](/README#Sending-messages-to-Unity).**

In this script, we use the Start function to:
- get the partner subdomain that was saved using the `Ready Player Me > Avatar Loader` editor window
- setup the Ready Player Me iFrame using the WebInterface 
- initialize the AvatarLoader

```c#
    private void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        CoreSettings partner = CoreSettingsHandler.CoreSettings;
        
        WebInterface.SetupRpmFrame(partner.Subdomain);
#endif
    }
```

*We wrap the logic here inside the condition `#if !UNITY_EDITOR && UNITY_WEBGL` to prevent errors in editor and compilation errors when building for a platform other than WebGL*

Next, we have the `OnWebViewAvatarGenerated` function which is called from Javascript as mentioned previously. 

```c#
    public void OnWebViewAvatarGenerated(string generatedUrl)
    {
        var avatarLoader = new AvatarObjectLoader();
        avatarUrl = generatedUrl;
        avatarLoader.OnCompleted += OnAvatarLoadCompleted;
        avatarLoader.OnFailed += OnAvatarLoadFailed;
        avatarLoader.LoadAvatar(avatarUrl);
    }
 ```

In here we create an instance of AvatarObjectLoader, set the avatarUrl to the generatedUrl from the iFrame a and trigger the the `LoadAvatar` function which updates the `AvatarURL`, starts loading the avatar, and destroys the currently loaded avatar if it exists. 
The code itself should be quite self explanatory but we will go through it step by step.
1. First we create an instance of the AvatarObjectLoader class
2. Then we set the avatarUrl to the generatedUrl from the iFrame
3. Next we subscribe to the `OnCompleted` and `OnFailed` events assigning local functions we will see later
4. Finally we call the `LoadAvatar` function

Lastly we have two functions used for handling the `AvatarObjectLoader` `OnCompleted` and `OnFailed ` callbacks. 

```c#
    private void OnAvatarLoadCompleted(object sender, CompletionEventArgs args)
    {
        if (avatar) Destroy(avatar);
        avatar = args.Avatar;
        if (args.Metadata.BodyType == BodyType.HalfBody)
        {
            avatar.transform.position = new Vector3(0, 1, 0);
        }
    }

    private void OnAvatarLoadFailed(object sender, FailureEventArgs args)
    {
        SDKLogger.Log(TAG,$"Avatar Load failed with error: {args.Message}");
    }
```

# Adding to an existing WebGL Template 

In some cases you may want to add the Ready Player Me iFrame to an existing WebGL template. 
For example you have a separate plugin that requires you to use their WebGL template but you also want to be able to integrate our Web avatar creator in your application. 

We created our WebGL RPMTemplate in are way that most of the code is separated and easy to move around. 
Below we will go through the steps required to add the Ready Player Me iFrame to an existing WebGL template.

1. Copy the entire ReadyPlayerMe folder from  `Assets\WebGLTemplates\RPMTemplate\TemplateData\ReadyPlayerMe` with all its contents
2. Paste it into your target WebGL Template so the new path is like this `Assets\WebGLTemplates\YourTemplate\TemplateData\ReadyPlayerMe`
3. Open the `index.html` file in your target WebGL Template and add the following code to the `<head>` section
    ```html
        <link rel="stylesheet" href="TemplateData/ReadyPlayerMe/RpmStyle.css">
    ```
4. Add the following code to of the `<body>` section inside the `canvas-wrap` but outside the `unity-canvas`
    ```html
      <div id="rpm-container">
        <iframe id="rpm-frame" class="rpm-frame" allow="camera *; microphone *"></iframe>
        <button id="rpm-hide-button" onclick="hideRpm()">Hide</button>
      </div>
    ```
5. Add the following code to the end of the `<body>` section
    ```html
    <script src="TemplateData/ReadyPlayerMe/RpmGlobal.js"></script> 
    <script src="TemplateData/ReadyPlayerMe/ReadyPlayerMeFrame.js"></script>
    ```
If done correctly then you should now be able to build your project and see the Ready Player Me iFrame in your WebGL build. 

Below is the full `index.html` file from our index.html file from our [RPMTemplate](https://github.com/readyplayerme/Unity-WebGL-Example/blob/main/Assets/WebGLTemplates/RPMTemplate/index.html) with the comments to highlight the lines you need to copy and paste. 

```html
<!doctype html>
<html lang="en-us">
  <head>
    <meta charset="utf-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <title>Unity WebGL Player | {{{ PRODUCT_NAME }}}</title>
    <link rel="stylesheet" href="TemplateData/style.css">
    <link rel="stylesheet" href="TemplateData/ReadyPlayerMe/RpmStyle.css"><!-- Required for RPM Web Creator -->
    <link rel="shortcut icon" href="TemplateData/favicon.ico" />
  </head>
  <body >
  <div id="unity-container" class="unity-desktop">
    <div id="canvas-wrap" >
      <!-- rpm-container div and contents required for RPM Web Creator -->
      <div id="rpm-container">
        <iframe id="rpm-frame" class="rpm-frame" allow="camera *; microphone *"></iframe>
        <button id="rpm-hide-button" onclick="hideRpm()">Hide</button>
      </div>
      <!-- rpm-container div and contents required for RPM Web Creator -->
      <canvas id="unity-canvas" ></canvas>
    </div>
  <div id="unity-loading-bar">
    <div id="unity-logo"></div>
    <div id="unity-progress-bar-empty">
      <div id="unity-progress-bar-full"></div>
    </div>
  </div>
  <div id="unity-warning"> </div>
  <div id="unity-footer" style="">
    <div id="unity-webgl-logo"></div>
    <div id="unity-fullscreen-button"></div>
    <div id="unity-build-title">{{{ PRODUCT_NAME }}}</div>
  </div>
</div>
    <script src="TemplateData/ReadyPlayerMe/RpmGlobal.js"></script> <!-- Required for RPM Web Creator -->
    <script src="TemplateData/Global.js"></script>
    <script src="TemplateData/UnitySetup.js"></script>
    <script src="TemplateData/ReadyPlayerMe/ReadyPlayerMeFrame.js"></script>  <!-- Required for RPM Web Creator -->
  </body>
</html>
```


## Links
**Ready Player Me Unity SDK**
- [Documentation](https://docs.readyplayer.me/ready-player-me/integration-guides/unity)
- [Download](https://github.com/readyplayerme/rpm-unity-sdk-core#readme)
- [Support](https://docs.readyplayer.me/ready-player-me/integration-guides/unity/troubleshooting)

**Resources** 
- [Unity WebGL Documentation](https://docs.unity3d.com/Manual/webgl-develop.html)

