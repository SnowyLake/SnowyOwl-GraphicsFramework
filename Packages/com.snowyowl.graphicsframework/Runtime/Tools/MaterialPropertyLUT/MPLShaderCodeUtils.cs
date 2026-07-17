using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace SnowyOwl.GraphicsFramework
{
#if UNITY_EDITOR
    public static class MPLShaderCodeUtils
    {
        private const string k_SetupPropertyLUTMacro = "SWYO_SETUP_PROPERTY_LUT";
        private const string k_PropertyLUTPixelDataPrefix = "swyo_PropertyLUT_PixelData";
        private const string k_PropertyLUTFile = "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/PropertyLUTCommonDefines.hlsl";
        private const string k_PropertyLUTFallbackFile = "Packages/com.snowyowl.graphicsframework/Shaders/Include/Generated/PropertyLUTFallbackDefines.hlsl";

        public static void Generate(MPLTemplete templete, UnityEngine.Object targetHLSL)
        {
            if (targetHLSL == null)
            {
                Debug.LogError($"{templete.name} don't have target hlsl file!");
                return;
            }
            
            var targetPath = AssetDatabase.GetAssetPath(targetHLSL);
            if (!targetPath.EndsWith(".hlsl"))
            {
                Debug.LogError($"{targetHLSL} is not hlsl file!");
                return;
            }

            var lines = new List<string>();
            
            var targetUpperName = targetHLSL.name.ToUpper();
            
            lines.Add($"#ifndef {targetUpperName}_INCLUDED");
            lines.Add($"#define {targetUpperName}_INCLUDED");
            
            lines.Add(string.Empty);
            lines.Add($"#include \"{k_PropertyLUTFile}\"");
            lines.Add($"#include \"{k_PropertyLUTFallbackFile}\"");
            lines.Add(string.Empty);
            lines.Add($"#define MAX_BODY_PART_COUNT {templete.groupCount}");
            lines.Add(string.Empty);
            lines.Add($"#if defined({SwyoShaderKeywords.PropertyLUTOn})");
            lines.Add(string.Empty);

            foreach (var (index, pixel) in templete.pixelLayouts.WithIndex())
            {
                if (!MPLUtils.IsValidPixel(pixel))
                {
                    continue;
                }
                
                GenerateStaticVariable(lines, index);
                
                if (pixel.layoutType == MPLPixelLayoutType.Vector3AndScalar1)
                {
                    GenerateLoadFunction(lines, pixel.channelRGB, index, MPLPixelChannel.rgb);
                    GenerateLoadFunction(lines, pixel.channelA, index, MPLPixelChannel.a);
                }
                else if (pixel.layoutType == MPLPixelLayoutType.Scalar4)
                {
                    GenerateLoadFunction(lines, pixel.channelR, index, MPLPixelChannel.r);
                    GenerateLoadFunction(lines, pixel.channelG, index, MPLPixelChannel.g);
                    GenerateLoadFunction(lines, pixel.channelB, index, MPLPixelChannel.b);
                    GenerateLoadFunction(lines, pixel.channelA, index, MPLPixelChannel.a);
                }
                else
                {
                    GenerateLoadFunction(lines, pixel.channelRGBA, index, MPLPixelChannel.rgba);
                }
                lines.Add(string.Empty);
            }
            lines.Add(string.Empty);

            lines.Add("void SetupPropertyLUT(uint propertyId)");
            lines.Add("{");
            foreach (var (index, pixel) in templete.pixelLayouts.WithIndex())
            {
                if (!MPLUtils.IsValidPixel(pixel))
                {
                    continue;
                }
                var hasMacro = GenerateMacro(lines, pixel);
                lines.Add($"\t{k_PropertyLUTPixelDataPrefix}_{index} = LoadPropertyLUT({index}, propertyId);");
                if (hasMacro)
                {
                    lines.Add("#endif");
                }
            }
            lines.Add("}");
            lines.Add($"#undef {k_SetupPropertyLUTMacro}");
            lines.Add($"#define {k_SetupPropertyLUTMacro}(__propertyId) SetupPropertyLUT(__propertyId)");
            
            lines.Add(string.Empty);
            lines.Add("#endif");
            lines.Add(string.Empty);
            lines.Add("#endif");
            
            File.WriteAllLines(targetPath, lines);

            GenerateFallbackDefines();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Generate hlsl file: {targetPath}");
        }
        
        public static string GetChannelType(MPLPixelChannel channel)
        {
            return (int)channel switch
            {
                <= (int)MPLPixelChannel.a => "float",
                (int)MPLPixelChannel.rgb => "float3",
                _ => "float4"
            };
        }

        private static void GenerateStaticVariable(List<string> lines, int functionIndex)
        {
            var sb = new StringBuilder();
            sb.Append("static float4 ").Append($"{k_PropertyLUTPixelDataPrefix}_{functionIndex};");
            lines.Add(sb.ToString());
        }

        private static void GenerateLoadFunction(List<string> lines, MPLItemPrototype prototype, int functionIndex, MPLPixelChannel channel)
        {
            if (!prototype)
            {
                return;
            }
            
            lines.Add($"#undef LoadPropertyLUT_{prototype.itemName}");
            var sb = new StringBuilder();
            sb.Append(GetChannelType(channel)).Append(" ")
              .Append("LoadPropertyLUT_")
              .Append(prototype.itemName).Append("()").Append(" ")
              .Append("{ ")
              .Append($"return {k_PropertyLUTPixelDataPrefix}_{functionIndex}")
              .Append($".{Enum.GetName(typeof(MPLPixelChannel), channel)}").Append(";")
              .Append(" }");
            lines.Add(sb.ToString());
        }

        private static void GenerateLoadMacro(List<string> lines, MPLItemPrototype prototype)
        {
            if (!prototype)
            {
                return;
            }
            lines.Add($"#define LoadPropertyLUT_{prototype.itemName}() 0");
        }
        
        private static void MacroInfosAdd(List<(string macro, bool inv)> macroInfos, MPLItemPrototype prototype)
        {
            if (!prototype)
            {
                return;
            }
            macroInfos.Add((new string(prototype.conditional.Where(c => !char.IsWhiteSpace(c)).ToArray()), false));
        }
        
        private static bool GenerateMacro(List<string> lines, MPLPixelLayout pixel)
        {
            var macroInfos = new List<(string macro, bool inv)>();
            
            if (pixel.layoutType == MPLPixelLayoutType.Vector3AndScalar1)
            {
                MacroInfosAdd(macroInfos, pixel.channelRGB);
                MacroInfosAdd(macroInfos, pixel.channelA);
            }
            else if (pixel.layoutType == MPLPixelLayoutType.Scalar4)
            {
                MacroInfosAdd(macroInfos, pixel.channelR);
                MacroInfosAdd(macroInfos, pixel.channelG);
                MacroInfosAdd(macroInfos, pixel.channelB);
                MacroInfosAdd(macroInfos, pixel.channelA);
            }
            else
            {
                MacroInfosAdd(macroInfos, pixel.channelRGBA);
            }
            if (macroInfos.IsNullOrEmpty() || macroInfos.Any(info => info.macro == string.Empty))
            {
                return false;
            }

            macroInfos = macroInfos.Distinct().ToList();

            for (var i = 0; i < macroInfos.Count; i++)
            {
                var info = macroInfos[i];
                if (info.macro.Contains("!"))
                {
                    info.macro = info.macro.Replace("!", "");
                    info.inv = true;
                    macroInfos[i] = info;
                }
            }

            var sb = new StringBuilder();
            sb.Append("#if ");
            foreach (var (index, info) in macroInfos.WithIndex())
            {
                if (index != 0)
                {
                    sb.Append(" || ");
                }
                sb.Append($"{(info.inv ? "!" : "")}defined({info.macro})");
            }
            lines.Add(sb.ToString());

            return true;
        }

        public static void GenerateFallbackDefines()
        {
            var targetHLSL = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(k_PropertyLUTFallbackFile);
            if (targetHLSL == null)
            {
                Debug.LogError("PropertyLUT fallback file not found!");
            }
            
            var templetes = MPLUtils.GetTempletes();
            var lines = new List<string>();
            var targetUpperName = targetHLSL.name.ToUpper();
            
            lines.Add($"#ifndef {targetUpperName}_INCLUDED");
            lines.Add($"#define {targetUpperName}_INCLUDED");
            lines.Add(string.Empty);

            var pixels = templetes.SelectMany(templete => templete.pixelLayouts)
                                  .Where(MPLUtils.IsValidPixel)
                                  .ToList();

            var macros = new List<string>();
            foreach (var pixel in pixels)
            {
                if (pixel.layoutType == MPLPixelLayoutType.Vector3AndScalar1)
                {
                    GenerateLoadMacro(macros, pixel.channelRGB);
                    GenerateLoadMacro(macros, pixel.channelA);
                }
                else if (pixel.layoutType == MPLPixelLayoutType.Scalar4)
                {
                    GenerateLoadMacro(macros, pixel.channelR);
                    GenerateLoadMacro(macros, pixel.channelG);
                    GenerateLoadMacro(macros, pixel.channelB);
                    GenerateLoadMacro(macros, pixel.channelA);
                }
                else
                {
                    GenerateLoadMacro(macros, pixel.channelRGBA);
                }
            }

            macros = macros.Distinct().ToList();
            lines.AddRange(macros);
            lines.Add($"#define {k_SetupPropertyLUTMacro}");
            
            lines.Add(string.Empty);
            lines.Add("#endif");
            
            File.WriteAllLines(k_PropertyLUTFallbackFile!, lines);
        }
    }
#endif
}