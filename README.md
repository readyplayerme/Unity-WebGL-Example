# Ready Player Me Unity WebGL Integration Example

This repository contains an Unity project that uses a custom WebGL template to embed the Ready Player Me character creator inside an application to seamlessly add avatars. This project can set as a reference for anybody wanting to add Reday Player Me Avatars into their Unity WebGL application.

## Templates

![img-templates](https://user-images.githubusercontent.com/7085672/167348039-527638cb-203e-47bd-b754-6cc2123213a8.png)

### RPM_2019

As the name suggests this template is specifically designed to work for Unity 2019 versions as the WebGL API changed in Unity 2020+.

## RPM_Template

This is template was created to support Unity version 2020 or higher, making use of the latest features of Unity's WebGL packaging system.

## Quick Start 

Open the example project in Unity (version 2019 or higher) and open up the build settings window.

![img-build-settings](https://user-images.githubusercontent.com/7085672/167348062-269c55c0-497b-4ac8-90b9-57b65d5ab18c.png)

Make sure the example scene `Scenes/WebGLExample` is added at the top of the Scenes In Build section.

Then click on the `Player Settings..` in the bottom left corner. From there select Player from the menu on the left side and open the Resolution and Presentation section.

![img-player-settings](https://user-images.githubusercontent.com/7085672/167348092-22d6b37d-127c-4a2d-a0c7-06781867031f.png)

From here select the RPMTemplate (or RPM_2019 if using Unity version 2019).

Once that is done you can open up the Build Settings window again, make sure you have select WebGL as the target platform and click Build And Run.

It will take some time to compile but once it is finished it should open up the WebGL application in your default browser.
