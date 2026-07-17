using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace SnowyOwl.GraphicsFramework
{
    public enum ShaderDefineType
    {
        Macro = 0,
        Pragma,
        PragmaMultiCompile,
    }
    
    public static class ShaderDefines
    {
        public const string DebugSymbolsOn = "enable_d3d11_debug_symbols";
        public const string DepthPrimingOn = "SWYO_DEPTH_PRIMING_ON";
    }
    
    [Serializable]
    public class ShaderDefineObject
    {
        public bool enable;
        public string defineText;
        public ShaderDefineType defineType;

        public ShaderDefineObject(string define, ShaderDefineType type, bool defaultEnable)
        {
            defineText = define;
            defineType = type;
            enable = defaultEnable;
        }
    }
    
    [Serializable]
    public class ShaderDefineGenerator
    {
        public const string k_GlobalShaderDefines = "Packages/com.snowyowl.graphicsframework/Shaders/Include/Generated/GlobalShaderDefines.hlsl";
        
        [ReadOnly]
        public List<ShaderDefineObject> shaderDefineTable = new()
        {
            new ShaderDefineObject(ShaderDefines.DebugSymbolsOn, ShaderDefineType.Pragma, false),
            new ShaderDefineObject(ShaderDefines.DepthPrimingOn, ShaderDefineType.Macro, false)
        };
        
        public void Generate()
        {
#if UNITY_EDITOR
            var lines = new List<string>();
            
            lines.Add($"#pragma once");
            lines.Add(string.Empty);
            
            foreach (var shaderDefine in shaderDefineTable)
            {
                var definePrefix = shaderDefine.defineType switch
                {
                    ShaderDefineType.Macro => "#define",
                    ShaderDefineType.Pragma => "#pragma",
                    ShaderDefineType.PragmaMultiCompile => "#pragma multi_compile _",
                    _ => ""
                };
            
                lines.Add($"{(shaderDefine.enable ? "" : "// ")}{definePrefix} {shaderDefine.defineText}");
            };
            
            File.WriteAllLines(k_GlobalShaderDefines, lines);
            AssetDatabase.Refresh();
            
            Debug.Log($"Regenerate global shader define file: {k_GlobalShaderDefines}");
#endif
        }

        public void SetShaderDefine(string defineText, bool enable)
        {
            if (!AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(k_GlobalShaderDefines))
            {
                return;
            }
            var defineObject = shaderDefineTable.Find(defineObj => defineObj.defineText == defineText);
            defineObject.enable = enable;
            Generate();
        }
    }
}