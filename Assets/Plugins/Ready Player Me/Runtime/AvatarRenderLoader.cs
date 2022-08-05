using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class AvatarRenderLoader
    {
        private const string FULL_BODY_POSTURE_WARNING = "Cannot use FullBodyPostureTransparent render scene type with HalfBody Avatars";
        private const string ARMATURE_MALE = "ArmatureTargetMale";
        private const string ARMATURE_FEMALE = "ArmatureTargetFemale";
        private const string BODY_TYPE_FULL = "fullbody-";
        private const string BODY_TYPE_HALF = "halfbody-";
        private const string DEFAULT_RENDER_SCENE = "portrait-v1";

        private readonly AvatarRenderDownloader avatarRenderDownloader;
        private readonly UrlProcessor urlProcessor;
        private readonly MetadataDownloader metadataDownloader;

        private AvatarRenderSettings renderSettings;
        private AvatarMetadata avatarMetadata;
        private AvatarUri avatarUri;

        private readonly Dictionary<AvatarRenderScene, string> renderSceneMap = new Dictionary<AvatarRenderScene, string>
        {
            { AvatarRenderScene.Portrait, "portrait-v1" },
            { AvatarRenderScene.PortraitTransparent, "portrait-v1-transparent" },
            { AvatarRenderScene.FullBodyPostureTransparent, "posture-v1-transparent" }
        };

        /// Called upon failure
        public Action<FailureType, string> OnFailed { get; set; }

        /// Called upon success.
        public Action<Texture2D> OnCompleted { get; set; }

        public AvatarRenderLoader()
        {
            urlProcessor = new UrlProcessor();
            urlProcessor.OnFailed = Fail;
            urlProcessor.OnCompleted = uri =>
            {
                avatarUri = uri;
                DownloadMetadata();
            };

            metadataDownloader = new MetadataDownloader();
            metadataDownloader.OnFailed = Fail;
            metadataDownloader.OnCompleted = CreateRenderRequestParameters;

            avatarRenderDownloader = new AvatarRenderDownloader();
            avatarRenderDownloader.OnFailed = Fail;
            avatarRenderDownloader.OnCompleted = DownloadComplete;
        }

        public void LoadRender(
            string url,
            AvatarRenderScene renderScene,
            string renderBlendShapeMesh = null,
            Dictionary<string, float> renderBlendShapes = null
        )
        {
            renderSettings = new AvatarRenderSettings
            {
                Model = url,
                Scene = renderScene,
                BlendShapeMesh = renderBlendShapeMesh,
                BlendShapes = renderBlendShapes
            };

            urlProcessor.Create(renderSettings.Model);
        }

        private void DownloadMetadata()
        {
            metadataDownloader.Download(avatarUri.MetadataUrl).Run();
        }

        private void CreateRenderRequestParameters(AvatarMetadata metadata)
        {
            avatarMetadata = metadata;

            if (avatarMetadata.BodyType == BodyType.HalfBody && renderSettings.Scene == AvatarRenderScene.FullBodyPostureTransparent)
            {
                OnFailed?.Invoke(FailureType.AvatarRenderError, FULL_BODY_POSTURE_WARNING);
                return;
            }

            renderSettings.Armature = avatarMetadata.OutfitGender == OutfitGender.Masculine ? ARMATURE_MALE : ARMATURE_FEMALE;
            var bodyType = avatarMetadata.BodyType == BodyType.FullBody ? BODY_TYPE_FULL : BODY_TYPE_HALF;
            var sceneType = renderSceneMap.ContainsKey(renderSettings.Scene) ? renderSceneMap[renderSettings.Scene] : DEFAULT_RENDER_SCENE;

            var requestParameters = new Dictionary<string, object>
            {
                { "model", avatarUri.ModelUrl },
                { "scene", bodyType + sceneType }
            };

            if (renderSettings.Armature != null) requestParameters.Add("armature", renderSettings.Armature);

            if (renderSettings.BlendShapeMesh != null && renderSettings.BlendShapes != null)
            {
                var blendShapeMesh = new Dictionary<string, object>();
                blendShapeMesh.Add(renderSettings.BlendShapeMesh, renderSettings.BlendShapes);
                requestParameters.Add("blendShapes", blendShapeMesh);
            }

            RequestAvatarRender(requestParameters);
        }

        private void RequestAvatarRender(Dictionary<string, object> requestParameters)
        {
            var json = JsonConvert.SerializeObject(requestParameters);
            var bytes = Encoding.UTF8.GetBytes(json);

            avatarRenderDownloader.RequestAvatarRenderUrl(bytes).Run();
        }

        private void DownloadComplete(Texture2D renderTexture)
        {
            OnCompleted?.Invoke(renderTexture);
        }

        private void Fail(FailureType type, string message)
        {
            OnFailed?.Invoke(type, message);
        }
    }
}
