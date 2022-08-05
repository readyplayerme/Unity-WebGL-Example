using System.ComponentModel;

namespace ReadyPlayerMe
{
    public enum BodyType
    {
        None,
        [Description("fullbody")] FullBody,
        [Description("halfbody")] HalfBody
    }

    public enum OutfitGender
    {
        None,
        [Description("masculine")] Masculine,
        [Description("feminine")] Feminine,
        [Description("neutral")] Neutral
    }

    public enum MeshType
    {
        HeadMesh,
        BeardMesh,
        TeethMesh
    }

    public enum FailureType
    {
        None,
        NoInternetConnection,
        UrlProcessError,
        ShortCodeError,
        MetadataDownloadError,
        MetadataParseError,
        ModelDownloadError,
        ModelImportError,
        DirectoryAccessError,
        AvatarProcessError,
        AvatarRenderError
    }

    public enum ProgressType
    {
        LoaderStarted,
        UrlProcessed,
        MetadataDownloaded,
        ModelDownloaded,
        ModelImported,
        AvatarLoaded
    }

    public enum AvatarRenderScene
    {
        [Description("Upper body render")] Portrait,
        [Description("Upper body render with transparent background")] PortraitTransparent,
        [Description("Posed full body render with transparent background")] FullBodyPostureTransparent
    }
}
