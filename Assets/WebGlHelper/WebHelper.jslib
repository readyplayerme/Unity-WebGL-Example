mergeInto(LibraryManager.library, {

    ShowReadyPlayerMeFrame: function () {
        var rpmContainer = document.getElementById("rpm-container");
        rpmContainer.style.display = "block";
    },
  
    HideReadyPlayerMeFrame: function () {
        var rpmContainer = document.getElementById("rpm-container");
        rpmContainer.style.display = "none";
    },
        
    SetupRpm: function (url, targetGameObjectName){
        setupRpmFrame(UTF8ToString(url), UTF8ToString(targetGameObjectName));
    },
});