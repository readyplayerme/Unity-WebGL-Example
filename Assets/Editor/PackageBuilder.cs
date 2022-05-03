using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PackageBuilder
{
    private static string versionNumber => "0.1.0";
    private static string packageName => $"Assets/ReadyPlayerMe_WebGlExample_{versionNumber}.unitypackage";
    private static string versionPopupMessage => $"Have you updated the package version number? Current package version is {versionNumber}";

    [MenuItem("Ready Player Me/Build WebGL Package")]
    private static void BuildPackage()
    {
        bool proceed = EditorUtility.DisplayDialog("Version Number Warning", versionPopupMessage, "Proceed", "Cancel");

        if (proceed)
        {
            EditorPrefs.SetBool("BuildStarted", true);
            Debug.Log("Package Builder Started");
        }
        
        CreatePackage();
    }
    
    private static void CreatePackage()
    {
        string[] paths = Directory.GetFiles(@"Assets\", "*", SearchOption.AllDirectories);

        AssetDatabase.ExportPackage(paths, packageName);

        Debug.Log($"PackageBuilder.BuildPackage: Package is built and saved at { packageName }");
        EditorUtility.RevealInFinder(packageName);
    }
}
