using System;
using Siccity.GLTFUtility;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class GltfUtilityAvatarImporter : IAvatarImporter
    {
        private const string TAG = nameof(GltfUtilityAvatarImporter);

        public Action<FailureType, string> OnFailed { get; set; }
        public Action<float> OnProgressChanged { get; set; }
        public Action<GameObject> OnCompleted { get; set; }

        private void Completed(GameObject avatar) => OnCompleted?.Invoke(avatar);

        // ReSharper disable once UnusedParameter.Local
        private void ProgressChanged(float progress, ImportType type) => OnProgressChanged?.Invoke(progress);

        public void Import(byte[] bytes)
        {
            SDKLogger.Log(TAG, "Importing avatar from byte array.");

            try
            {
#if UNITY_EDITOR || UNITY_WEBGL
                var avatar = Importer.LoadFromBytes(bytes, new ImportSettings());
                OnCompleted?.Invoke(avatar);
                OnProgressChanged?.Invoke(1);
#else
                    Importer.ImportGLBAsync(bytes, new ImportSettings(), Completed, ProgressChanged);
#endif
            }
            catch (Exception e)
            {
                Fail(FailureType.ModelImportError, $"Failed to import glb model from bytes. {e.Message}");
            }
        }

        public void Import(string path)
        {
            SDKLogger.Log(TAG, $"Importing avatar from path {path}");

            try
            {
#if UNITY_EDITOR || UNITY_WEBGL
                var avatar = Importer.LoadFromFile(path, new ImportSettings());
                OnProgressChanged?.Invoke(1);
                OnCompleted?.Invoke(avatar);
#else
                    Importer.ImportGLBAsync(path, new ImportSettings(), Completed, ProgressChanged);
#endif
            }
            catch (Exception e)
            {
                Fail(FailureType.ModelImportError, $"Failed to import glb model from path. {e.Message}");
            }
        }

        private void Fail(FailureType failureType, string message)
        {
            SDKLogger.Log(TAG, message);
            OnFailed?.Invoke(failureType, message);
        }
    }
}
