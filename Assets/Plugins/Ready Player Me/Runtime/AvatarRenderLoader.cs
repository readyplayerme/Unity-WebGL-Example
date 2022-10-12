using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class AvatarRenderLoader
    {
        public int Timeout { get; set; }

        /// Called upon failure
        public Action<FailureType, string> OnFailed { get; set; }

        /// Called upon success.
        public Action<Texture2D> OnCompleted { get; set; }

        public Action<float, string> ProgressChanged { get; set; }

        public async void LoadRender(
            string url,
            AvatarRenderScene renderScene,
            string renderBlendShapeMesh = null,
            Dictionary<string, float> renderBlendShapes = null
        )
        {
            var renderSettings = new AvatarRenderSettings
            {
                Model = url,
                Scene = renderScene,
                BlendShapeMesh = renderBlendShapeMesh,
                BlendShapes = renderBlendShapes
            };

            var context = new AvatarContext();
            context.Url = renderSettings.Model;
            context.RenderSettings = renderSettings;

            var executor = new OperationExecutor<AvatarContext>(new IOperation<AvatarContext>[]
            {
                new UrlProcessor(),
                new MetadataDownloader(),
                new RenderRequestParameterProcessor(),
                new AvatarRenderDownloader()
            });
            executor.ProgressChanged += ProgressChanged;
            executor.Timeout = Timeout;

            try
            {
                context = await executor.Execute(context);
            }
            catch (CustomException exception)
            {
                OnFailed(exception.FailureType, exception.Message);
                return;
            }

            OnCompleted?.Invoke((Texture2D) context.Data);
        }
    }
}
