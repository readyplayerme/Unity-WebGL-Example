mergeInto(LibraryManager.library, {

    ShowReadyPlayerMeFrame: function () {
        showRpm();
    },
  
    HideReadyPlayerMeFrame: function () {
        hideRpm();
    },
        
    SetupRpm: function (url, targetGameObjectName){
        setupRpmFrame(UTF8ToString(url), UTF8ToString(targetGameObjectName));
    },
    
    ReloadUrl: function (url){
        reloadUrl(UTF8ToString(url));
    }
    
});