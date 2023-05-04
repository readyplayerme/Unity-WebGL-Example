mergeInto(LibraryManager.library, {

    ShowReadyPlayerMeFrame: function () {
        showRpm();
    },
  
    HideReadyPlayerMeFrame: function () {
        hideRpm();
    },
        
    SetupRpm: function (partner){
        setupRpmFrame(UTF8ToString(partner));
    },
});