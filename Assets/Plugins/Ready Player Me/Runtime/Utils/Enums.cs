using System.ComponentModel;
using UnityEngine;

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
        DownloadError,
        MetadataDownloadError,
        MetadataParseError,
        ModelDownloadError,
        ModelImportError,
        DirectoryAccessError,
        AvatarProcessError,
        AvatarRenderError,
        OperationCancelled
    }

    public enum AvatarRenderScene
    {
        [Description("Upper body render")] Portrait,
        [Description("Upper body render with transparent background")] PortraitTransparent,
        [Description("Posed full body render with transparent background")] FullBodyPostureTransparent
    }

#region Avatar API

    public enum Pose
    {
        APose,
        TPose
    }

    public enum MeshLod
    {
        [InspectorName("High (LOD0)")]
        High,
        [InspectorName("Medium (LOD1)")]
        Medium,
        [InspectorName("Low (LOD2)")]
        Low
    }

    public enum TextureAtlas
    {
        None,
        [InspectorName("High (1024)")]
        High,
        [InspectorName("Medium (512)")]
        Medium,
        [InspectorName("Low (256)")]
        Low
    }

#endregion
}
