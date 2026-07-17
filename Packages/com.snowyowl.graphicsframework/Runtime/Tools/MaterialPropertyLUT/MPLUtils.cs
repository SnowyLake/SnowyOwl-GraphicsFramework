using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace SnowyOwl.GraphicsFramework
{
    [Serializable]
    public enum MPLItemType
    {
        Float = 0, Int, FloatSlider, IntSlider, Bool, Color3, Vector3, Color4, Vector4
    }
    
    [Serializable]
    public enum MPLItemValueType
    {
        Scalar = 0, Vector3, Vector4
    }
    
    [Serializable]
    public enum MPLPixelLayoutType
    {
        Vector3AndScalar1 = 0, Scalar4, Vector4
    }
    [Serializable]
    public enum MPLPixelChannel
    {
        r = 0, g, b, a, rgb, rgba
    }

    public static class MPLUtils
    {
        public static MPLItemValueType GetItemValueType(MPLItemType type)
        {
            return (int)type switch
            {
                <= (int)MPLItemType.Bool => MPLItemValueType.Scalar,
                <= (int)MPLItemType.Vector3 => MPLItemValueType.Vector3,
                _ => MPLItemValueType.Vector4
            };
        }

        public static bool IsSlider(MPLItemType type)
        {
            return type is MPLItemType.FloatSlider or MPLItemType.IntSlider;
        }
        
        public static bool IsVector(MPLItemType type)
        {
            return type is MPLItemType.Vector3 or MPLItemType.Vector4;
        }
        
        public static bool IsColor(MPLItemType type)
        {
            return type is MPLItemType.Color3 or MPLItemType.Color4;
        }
        
        public static bool RequireRange(MPLItemType type)
        {
            return IsSlider(type) || IsVector(type);
        }
        
        public static half GetPixelDataScalar(List<MPLItemData> itemDatas, MPLItemPrototype channelPrototype)
        {
            if (!channelPrototype)
            {
                return half.zero;
            }
            var itemData = itemDatas.Find(itemData => itemData.prototype == channelPrototype);
            return itemData?.GetScalarHalfValue() ?? half.zero;
        }
        public static half3 GetPixelDataVector3(List<MPLItemData> itemDatas, MPLItemPrototype channelPrototype)
        {
            if (!channelPrototype)
            {
                return half3.zero;
            }
            var itemData = itemDatas.Find(itemData => itemData.prototype == channelPrototype);
            return itemData?.GetVector3HalfValue() ?? half3.zero;
        }
        public static half4 GetPixelDataVector4(List<MPLItemData> itemDatas, MPLItemPrototype channelPrototype)
        {
            if (!channelPrototype)
            {
                return half4.zero;
            }
            var itemData = itemDatas.Find(itemData => itemData.prototype == channelPrototype);
            return itemData?.GetVector4HalfValue() ?? half4.zero;
        }
        
        public static bool IsValidPixel(MPLPixelLayout pixel)
        {
            return pixel.layoutType switch
            {
                MPLPixelLayoutType.Vector3AndScalar1 => pixel.channelRGB || pixel.channelA,
                MPLPixelLayoutType.Scalar4 => pixel.channelR || pixel.channelG || pixel.channelB || pixel.channelA,
                MPLPixelLayoutType.Vector4 => pixel.channelRGBA,
                _ => false
            };
        }
        
        public static int GetValidPixelCount(MPLTemplete templete)
        {
            var count = 0;
            foreach (var pixel in templete.pixelLayouts)
            {
                if (IsValidPixel(pixel))
                {
                    count++;
                }
            }
            return count;
        }
        
#if UNITY_EDITOR
        public static List<MPLPreset> GetPresets(MPLTemplete templete)
        {
            var presets = AssetDatabase.FindAssets("t:MPLPreset")
                                       .Select(AssetDatabase.GUIDToAssetPath)
                                       .Select(AssetDatabase.LoadAssetAtPath<MPLPreset>)
                                       .Where(preset => preset.templete == templete)
                                       .ToList();
            return presets;
        }
        
        public static List<MPLGenerator> GetGenerators(MPLTemplete templete)
        {
            var generators = AssetDatabase.FindAssets("t:MPLGenerator")
                                          .Select(AssetDatabase.GUIDToAssetPath)
                                          .Select(AssetDatabase.LoadAssetAtPath<MPLGenerator>)
                                          .Where(generator => generator.templete == templete)
                                          .ToList();
            return generators;
        }
        
        public static List<MPLTemplete> GetTempletes()
        {
            var templetes = AssetDatabase.FindAssets("t:MPLTemplete")
                                          .Select(AssetDatabase.GUIDToAssetPath)
                                          .Select(AssetDatabase.LoadAssetAtPath<MPLTemplete>)
                                          .ToList();
            return templetes;
        }
        
        public static MPLGenerator CreateGenerator(string templetePath)
        {
            var templete = AssetDatabase.LoadMainAssetAtPath(templetePath) as MPLTemplete;
            var generator = ScriptableObject.CreateInstance<MPLGenerator>();
            generator.Init(templete);
            return generator;
        }
        
        public static MPLPreset CreatePreset(string templetePath)
        {
            var templete = AssetDatabase.LoadMainAssetAtPath(templetePath) as MPLTemplete;
            var preset = ScriptableObject.CreateInstance<MPLPreset>();
            preset.Init(templete);
            return preset;
        }
#endif
    }
}