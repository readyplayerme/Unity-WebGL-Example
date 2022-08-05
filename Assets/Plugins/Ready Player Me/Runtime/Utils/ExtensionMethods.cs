using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace ReadyPlayerMe
{
    public static class ExtensionMethods
    {
        #region Coroutine Runner

        [ExecuteInEditMode]
        public class CoroutineRunner : MonoBehaviour
        {
            ~CoroutineRunner()
            {
                Destroy(gameObject);
            }
        }

        private static CoroutineRunner operation;

        private const HideFlags HIDE_FLAGS = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy |
                                             HideFlags.HideInInspector | HideFlags.NotEditable |
                                             HideFlags.DontSaveInBuild;

        public static Coroutine Run(this IEnumerator iEnumerator)
        {
            CoroutineRunner[] operations = Resources.FindObjectsOfTypeAll<CoroutineRunner>();
            if (operations.Length == 0)
            {
                operation = new GameObject("[CoroutineRunner]").AddComponent<CoroutineRunner>();
                operation.hideFlags = HIDE_FLAGS;
                operation.gameObject.hideFlags = HIDE_FLAGS;
            }
            else
            {
                operation = operations[0];
            }

            return operation.StartCoroutine(iEnumerator);
        }

        public static void Stop(this Coroutine coroutine)
        {
            if (operation != null)
            {
                operation.StopCoroutine(coroutine);
            }
        }

        #endregion

        #region Get Picker

        private static readonly string[] HeadMeshNameFilter = { "Renderer_Head", "Renderer_Avatar" };

        private const string BEARD_MESH_NAME_FILTER = "Renderer_Beard";
        private const string TEETH_MESH_NAME_FILTER = "Renderer_Teeth";

        public static SkinnedMeshRenderer GetMeshRenderer(this GameObject gameObject, MeshType meshType)
        {
            SkinnedMeshRenderer mesh;
            var children = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();

            if (children.Count == 0)
            {
                Debug.Log("ExtensionMethods.GetMeshRenderer: No SkinnedMeshRenderer found on the Game Object.");
                return null;
            }

            switch (meshType)
            {
                case MeshType.BeardMesh:
                    mesh = children.FirstOrDefault(child => BEARD_MESH_NAME_FILTER == child.name);
                    break;
                case MeshType.TeethMesh:
                    mesh = children.FirstOrDefault(child => TEETH_MESH_NAME_FILTER == child.name);
                    break;
                case MeshType.HeadMesh:
                    mesh = children.FirstOrDefault(child => HeadMeshNameFilter.Contains(child.name));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(meshType), meshType, null);
            }

            if (mesh != null) return mesh;

            Debug.Log($"ExtensionMethods.GetMeshRenderer: Mesh type {meshType} not found on the Game Object.");
            return null;
        }

        #endregion
    }
}
