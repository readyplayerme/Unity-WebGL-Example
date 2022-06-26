# Ready Player Me Unity WebGL Integration Example

This repository contains a Unity project that uses a custom WebGL template to embed the Ready Player Me character creator inside an application to seamlessly add avatars. This project can be used as a reference for anybody wanting to add Ready Player Me Avatars into their Unity WebGL application.

## Templates

![img-templates](https://user-images.githubusercontent.com/7085672/167348039-527638cb-203e-47bd-b754-6cc2123213a8.png)

**RPM_2019**

As the name suggests this template is specifically designed to work for Unity 2019 versions as the WebGL API changed in Unity 2020+.

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
    <div id="canvas-wrap">
      <div id="rpm-container">
        <iframe id="rpm-frame" class="rpm-frame" allow="camera *; microphone *"></iframe>
        <button id="rpm-hide-button" onclick="hideRpm()">Hide</button>
      </div>
      <canvas id="unity-canvas" width={{{WIDTH}}} height={{{HEIGHT}}}></canvas>
    </div>
```

To keep things clean we separated the javascript logic into two separate files. There is UnitySetup.js which is based off the Unity Default template, and ReadyPlayerMeFrame.js which we created to handle the setup of the iFrame that will run [readyplayer.me](https://demo.readyplayer.me/avatar). 

## Ready Player Me Frame 

All the logic for setting up the Ready Player Me iFrame and subscribing to events can be found in `RPMTemplate/TemplateData/ReadyPlayerMeFrame.js`

### Listen to website events

Before anything else, we add an event listener and bind it to a subscribe function so we can handle all events that the Ready Player Me website sends as you can see in the snippet below. 

```js
    window.addEventListener("message", subscribe);
    document.addEventListener("message", subscribe);
```

Next, we will look at the subscribe function that is called whenever we receive a message from the browser. To begin we set the src url for the iFrame

```js 
    rpmFrame.src = `https://${subdomain != "" ? subdomain : "demo"}.readyplayer.me/avatar?frameApi`;
```
 The subdomain value is loaded from the partner scriptable object. This can be set and saved using **Ready Player Me Avatar Loader window** accessed from the top toolbar at `Ready Player Me > Avatar Loader`. See the [Web Avatar Loader](/README##Web-Avatar-Loader) section to see how the subdomain value is loaded.

The next chunk of logic here is used to filter the events so that we only handle valid events as you can see by this conditional statement here. 

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
            rpmContainer.style.display = "block";
        },

        HideReadyPlayerMeFrame: function () {
            var rpmContainer = document.getElementById("rpm-container");
            rpmContainer.style.display = "none";
        },

        SetupRpm: function (partner){
            setupRpmFrame(UTF8ToString(partner));
        },
    }); 
```

As the names suggest there are 2 for showing and hiding the Ready Player Me iFrame, and 1 for setting up the iFrame. By creating this jslib and putting it into the Plugins folder we can import these into Unity Scripts to use them as we have here in the WebInterface.cs class.

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

The WebAvatarLoader.cs is a simple class based on the RuntimeTest.cs example, it handles the loading of the avatar loading and setup of the Ready Player Me iFrame. 

**It is important to note that this script is added to an object called `WebAvatarLoader` and it has a function called `OnWebViewAvatarGenerated`. Both the name and the function are required so that it can receive the SendMessage function from the browser as mentioned previously [here](/README#Sending-messages-to-Unity).**

In this script, we use the Start function to:
- get the partner subdomain that was saved using the `Ready Player Me > Avatar Loader` editor window
- setup the Ready Player Me iFrame using the WebInterface 
- initialize the AvatarLoader

```c#
    private void Start()
    {
        PartnerSO partner = Resources.Load<PartnerSO>("Partner");
        WebInterface.SetupRpmFrame(partner.Subdomain);
        avatarLoader = new AvatarLoader();
    }
```

Next, we have the `OnWebViewAvatarGenerated` function which is called from Javascript as mentioned previously. 

```c#
    public void OnWebViewAvatarGenerated(string avatarUrl)
    {
        LoadAvatar(avatarUrl);
    }
 ```

Here we run the `LoadAvatar` function which updates the `AvatarURL`, starts loading the avatar, and destroys the currently loaded avatar if it exists. 

```c#
    public void LoadAvatar(string avatarUrl)
    {
        AvatarURL = avatarUrl;
        avatarLoader.LoadAvatar(AvatarURL, OnAvatarImported, OnAvatarLoaded);
        if (avatar) Destroy(avatar);
    }
```

Lastly we have two functions used for handling the `avatarLoader.LoadAvatar` `OnAvatarImported` and `OnAvatarLoaded` callbacks. 

```c#
    private void OnAvatarImported(GameObject avatar)
    {
        Debug.Log($"Avatar imported. [{Time.timeSinceLevelLoad:F2}]");
    }

    private void OnAvatarLoaded(GameObject avatar, AvatarMetaData metaData)
    {
        this.avatar = avatar;
        Debug.Log($"Avatar loaded. [{Time.timeSinceLevelLoad:F2}]\n\n{metaData}");
    }
```


## Links
**Ready Player Me Unity SDK**
- [Documentation](https://docs.readyplayer.me/ready-player-me/integration-guides/unity)
- [Download](https://docs.readyplayer.me/ready-player-me/integration-guides/unity/unity-sdk-download)
- [Support](https://docs.readyplayer.me/ready-player-me/integration-guides/unity/troubleshooting)

**Resources** 
- [Unity WebGL Documentation](https://docs.unity3d.com/Manual/webgl-develop.html)

