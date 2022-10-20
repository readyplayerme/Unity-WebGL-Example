using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReadyPlayerMe
{
    public class RenderRequestParameterProcessor : IOperation<AvatarContext>
    {
        private const string FULL_BODY_POSTURE_WARNING = "Cannot use FullBodyPostureTransparent render scene type with HalfBody Avatars";
        private const string ARMATURE_MALE = "ArmatureTargetMale";
        private const string ARMATURE_FEMALE = "ArmatureTargetFemale";
        private const string BODY_TYPE_FULL = "fullbody-";
        private const string BODY_TYPE_HALF = "halfbody-";
        private const string DEFAULT_RENDER_SCENE = "portrait-v1";

        private readonly Dictionary<AvatarRenderScene, string> renderSceneMap = new Dictionary<AvatarRenderScene, string>
        {
            { AvatarRenderScene.Portrait, "portrait-v1" },
            { AvatarRenderScene.PortraitTransparent, "portrait-v1-transparent" },
            { AvatarRenderScene.FullBodyPostureTransparent, "posture-v1-transparent" }
        };

        public int Timeout { get; set; }
        public Action<float> ProgressChanged { get; set; }

        public Task<AvatarContext> Execute(AvatarContext context, CancellationToken token)
        {
            try
            {
                context = CreateRenderRequestParameters(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            ProgressChanged?.Invoke(1);
            return Task.FromResult(context);
        }


        private AvatarContext CreateRenderRequestParameters(AvatarContext context)
        {
            var metadata = context.Metadata;
            var renderSettings = context.RenderSettings;

            if (metadata.BodyType == BodyType.HalfBody && renderSettings.Scene == AvatarRenderScene.FullBodyPostureTransparent)
            {
                throw new CustomException(FailureType.AvatarRenderError, FULL_BODY_POSTURE_WARNING);
            }

            renderSettings.Armature = metadata.OutfitGender == OutfitGender.Masculine ? ARMATURE_MALE : ARMATURE_FEMALE;
            var bodyType = metadata.BodyType == BodyType.FullBody ? BODY_TYPE_FULL : BODY_TYPE_HALF;
            var sceneType = renderSceneMap.ContainsKey(renderSettings.Scene) ? renderSceneMap[renderSettings.Scene] : DEFAULT_RENDER_SCENE;

            var requestParameters = new Dictionary<string, object>
            {
                { "model", context.AvatarUri.ModelUrl },
                { "scene", bodyType + sceneType }
            };

            requestParameters.Add("armature", renderSettings.Armature);

            if (renderSettings.BlendShapeMesh != null && renderSettings.BlendShapes != null)
            {
                var blendShapeMesh = new Dictionary<string, object>();
                blendShapeMesh.Add(renderSettings.BlendShapeMesh, renderSettings.BlendShapes);
                requestParameters.Add("blendShapes", blendShapeMesh);
            }

            var json = JsonConvert.SerializeObject(requestParameters);
            context.Bytes = Encoding.UTF8.GetBytes(json);
            return context;
        }
    }
}
