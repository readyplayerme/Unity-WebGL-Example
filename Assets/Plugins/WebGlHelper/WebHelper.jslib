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