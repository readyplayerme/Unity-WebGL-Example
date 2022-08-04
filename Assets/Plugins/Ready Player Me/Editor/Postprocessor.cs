using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReadyPlayerMe
{
    public class Postprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (var item in importedAssets)
            {
                // TODO Find a better way
                if (item.Contains("RPM_EditorImage_"))
                {
                    UpdateAlwaysIncludedShaderList();
                    AddRpmDefineSymbol();
                    return;
                }
            }
        }

        #region Environment Settings

        private const string RPM_SYMBOL = "READY_PLAYER_ME";

        private static void AddRpmDefineSymbol()
        {
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defineString = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            var symbols = new HashSet<string>(defineString.Split(';')) { RPM_SYMBOL };
            var newDefineString = string.Join(";", symbols.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, newDefineString);
        }

        #endregion

        #region Animation Settings

        private const string ANIMATION_ASSET_PATH = "Assets/Plugins/Ready Player Me/Resources/Animations";

        private void OnPreprocessModel()
        {
            var modelImporter = assetImporter as ModelImporter;
            UpdateAnimationFileSettings(modelImporter);
        }

        private void UpdateAnimationFileSettings(ModelImporter modelImporter)
        {
            void SetModelImportData()
            {
                if (modelImporter is null) return;
                modelImporter.useFileScale = false;
                modelImporter.animationType = ModelImporterAnimationType.Human;
            }

            if (assetPath.Contains(ANIMATION_ASSET_PATH))
            {
                SetModelImportData();
            }
        }

        #endregion

        #region Shader Settings

        private const string INCLUDE_SHADER_PROPERTY = "m_AlwaysIncludedShaders";
        private const string GRAPHICS_SETTING_PATH = "ProjectSettings/GraphicsSettings.asset";

        private static readonly string[] AlwaysIncludeShader = new string[4];

        private static readonly string[] ShaderNames =
        {
            "Standard (Specular)",
            "Standard Transparent (Specular)",
            "Standard (Metallic)",
            "Standard Transparent (Metallic)"
        };

        private static string GetShaderRoot()
        {
            var pipeline = GraphicsSettings.defaultRenderPipeline == null
                ? "GLTFUtility"
                : "GLTFUtility/URP";
            return pipeline;
        }

        private static void UpdateAlwaysIncludedShaderList()
        {
            for (var i = 0; i < AlwaysIncludeShader.Length; i++)
            {
                AlwaysIncludeShader[i] = $"{GetShaderRoot()}/{ShaderNames[i]}";
            }

            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>(GRAPHICS_SETTING_PATH);
            var serializedGraphicsObject = new SerializedObject(graphicsSettings);
            var shaderIncludeArray = serializedGraphicsObject.FindProperty(INCLUDE_SHADER_PROPERTY);
            var includesShader = false;

            foreach (var includeShaderName in AlwaysIncludeShader)
            {
                var shader = Shader.Find(includeShaderName);
                if (shader == null)
                {
                    break;
                }

                for (var i = 0; i < shaderIncludeArray.arraySize; ++i)
                {
                    var shaderInArray = shaderIncludeArray.GetArrayElementAtIndex(i);
                    if (shader == shaderInArray.objectReferenceValue)
                    {
                        includesShader = true;
                        break;
                    }
                }

                if (!includesShader)
                {
                    var newArrayIndex = shaderIncludeArray.arraySize;
                    shaderIncludeArray.InsertArrayElementAtIndex(newArrayIndex);
                    var shaderInArray = shaderIncludeArray.GetArrayElementAtIndex(newArrayIndex);
                    shaderInArray.objectReferenceValue = shader;
                    serializedGraphicsObject.ApplyModifiedProperties();
                }
            }

            AssetDatabase.SaveAssets();
        }

        #endregion
    }
}
