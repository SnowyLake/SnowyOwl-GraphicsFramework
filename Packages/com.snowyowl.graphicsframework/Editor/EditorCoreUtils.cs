using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
    using UnityEditor;
#endif

using SnowyOwl.GraphicsFramework;
    
namespace SnowyOwl.GraphicsFramework.Editor
{
    public static class EditorCoreUtils
    {
        private const string k_DefaultURPShaderTemplete = "Packages/com.snowyowl.graphicsframework/Shaders/Templetes/DefaultURPShaderTemplete.shader";
        private const string k_DefaultHLSLTemplete = "Packages/com.snowyowl.graphicsframework/Shaders/Templetes/DefaultHLSLTemplate.hlsl";
        
        [MenuItem(CoreUtils.AssetMenuItemPrefix + "Shader Templete/Default URP Shader", priority = CoreUtils.EditorPriority.Other)]
        public static void CreateDefaultURPShader()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(k_DefaultURPShaderTemplete, "DefaultURP.shader");
        }
        
        [MenuItem(CoreUtils.AssetMenuItemPrefix + "Shader Templete/Default HLSL", priority = CoreUtils.EditorPriority.Other)]
        public static void CreateDefaultHLSL()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(k_DefaultHLSLTemplete, "Default.hlsl");
        }
    }
}
