using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AnimationExtractor : Editor
{
    public struct AnimClipData
    {
        public string AssetPath;
        public string AssetName;
        public string ClipDirectory;
    }

    private const string PREVIEW_ANIM_PREFIX = "__preview__";

    [MenuItem("Assets/Extract Animations", false, 9999)]
    private static AnimClipData ExtractAnimations()
    {
        var data = new AnimClipData();
        data.AssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        data.AssetName = Path.GetFileName(data.AssetPath);
        data.ClipDirectory = Path.GetDirectoryName(data.AssetPath);

        // Get all animations and create animation clip array to store them
        var assetObjects = AssetDatabase.LoadAllAssetsAtPath(data.AssetPath);
        var animationClips = new List<AnimationClip>();

        foreach (var assetObject in assetObjects)
        {
            // Only add valid animation clips
            if (assetObject is AnimationClip clip && !clip.name.StartsWith(PREVIEW_ANIM_PREFIX))
            {
                animationClips.Add(clip);
            }
        }

        foreach (var animationClip in animationClips)
        {
            var temp = new AnimationClip();
            // Copy, create and save assets
            EditorUtility.CopySerialized(animationClip, temp);
            var validatedName = string.Join("_", animationClip.name.Split(Path.GetInvalidFileNameChars()));
            AssetDatabase.CreateAsset(temp, $"{data.ClipDirectory}/{validatedName}.anim");
        }

        return data;
    }

    [MenuItem("Assets/Extract Animations and Delete File", false, 9999)]
    private static void ExtractAnimationsAndDeleteFile()
    {
        var data = ExtractAnimations();

        if (EditorUtility.DisplayDialog("File Deletion Warning", $"Are you sure you want to delete {data.AssetName}?",
                "Okay", "Cancel"))
        {
            AssetDatabase.DeleteAsset(data.AssetPath);
        }
    }

    [MenuItem("Assets/Extract Animations", true)]
    [MenuItem("Assets/Extract Animations and Delete File", true)]
    private static bool ExtractAnimationsValidation()
    {
        var activeObject = Selection.activeObject;
        if (!activeObject) return false;

        var assetPath = AssetDatabase.GetAssetPath(activeObject);
        var isFbx = Path.GetExtension(assetPath).ToLower() == ".fbx";
        var isGameObject = activeObject is GameObject;

        return isGameObject && isFbx;
    }
}
