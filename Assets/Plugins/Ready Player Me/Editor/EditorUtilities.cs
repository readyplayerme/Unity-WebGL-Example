using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    public static class EditorUtilities
    {
        private const string TAG = nameof(EditorUtilities);

        public static void CreatePrefab(GameObject source, string path)
        {
            PrefabUtility.SaveAsPrefabAssetAndConnect(source, path, InteractionMode.AutomatedAction, out var success);
            PrefabUtility.ApplyObjectOverride(source, path, InteractionMode.AutomatedAction);

            SDKLogger.Log(TAG, success ? $"Prefab created successfully at path: {path}" : "Prefab creation failed");

            AssetDatabase.Refresh();
            EditorUtility.SetDirty(source);
        }
    }
}
