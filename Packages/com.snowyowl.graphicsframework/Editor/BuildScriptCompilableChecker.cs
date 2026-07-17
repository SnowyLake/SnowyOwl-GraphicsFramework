#if SBP_INCLUDE
using System;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.SceneManagement;

namespace SnowyOwl.GraphicsFramework.Editor
{
    public static class BuildScriptCompilableChecker
    {
        private const string k_ABName = "COMPILE_TEMP_AB";
        private const string k_OutputPath = "Temp/BuildScriptCompilableChecker";
        
        [MenuItem(CoreUtils.EditorMenuItemPrefix + "Build Script Compilable Checker")]
        public static void Run()
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (activeScene.isDirty)
            {
                EditorSceneManager.SaveScene(activeScene);
            }
        
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            const BuildAssetBundleOptions options = BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.UncompressedAssetBundle;
            
            var abb = new AssetBundleBuild
            {
                assetBundleName = k_ABName,
                assetNames = Array.Empty<string>()
            };
            
            var manifest = CompatibilityBuildPipeline.BuildAssetBundles(k_OutputPath,new []{abb} , options, buildTarget);
            if (manifest)
            {
                EditorUtility.DisplayDialog("Build Script Compilable Check", "Compile successful!", "Great!");
            }
            else
            {
                EditorUtility.DisplayDialog("Build Script Compilable Check", "Compile failed, please check the Log!", "Oh shit!");
            }
        }
    }
}
#endif