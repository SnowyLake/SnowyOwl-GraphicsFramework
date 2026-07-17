using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace SnowyOwl.GraphicsFramework
{
    [Serializable]
    public class MPLPixelLayout
    {
        [TableColumnWidth(50)]
        public MPLPixelLayoutType layoutType;
        
        [HorizontalGroup("Layout"), VerticalGroup("Layout/1"), TableColumnWidth(400), LabelWidth(80)]
        [ShowIf("@layoutType == MPLPixelLayoutType.Scalar4")]
        [ValidateInput("IsScalar", "Prototype must be scalar type!")]
        public MPLItemPrototype channelR;
        
        [HorizontalGroup("Layout"), VerticalGroup("Layout/2"), TableColumnWidth(400), LabelWidth(80)]
        [ShowIf("@layoutType == MPLPixelLayoutType.Scalar4")]
        [ValidateInput("IsScalar", "Prototype must be scalar type!")]
        public MPLItemPrototype channelG;
        
        [HorizontalGroup("Layout"), VerticalGroup("Layout/1"), TableColumnWidth(400), LabelWidth(80)]
        [ShowIf("@layoutType == MPLPixelLayoutType.Scalar4")]
        [ValidateInput("IsScalar", "Prototype must be scalar type!")]
        public MPLItemPrototype channelB;
        
        [HorizontalGroup("Layout"), VerticalGroup("Layout/2"), TableColumnWidth(400), LabelWidth(80)]
        [ShowIf("@layoutType != MPLPixelLayoutType.Vector4")]
        [ValidateInput("IsScalar", "Prototype must be scalar type!")]
        public MPLItemPrototype channelA;
        
        [HorizontalGroup("Layout"), VerticalGroup("Layout/1"), TableColumnWidth(400), LabelWidth(80)]
        [ShowIf("@layoutType == MPLPixelLayoutType.Vector3AndScalar1")]
        [ValidateInput("IsVector3", "Prototype must be vector3 type!")]
        public MPLItemPrototype channelRGB;
        
        [HorizontalGroup("Layout"), VerticalGroup("Layout/1"), TableColumnWidth(400), LabelWidth(80)]
        [ShowIf("@layoutType == MPLPixelLayoutType.Vector4")]
        [ValidateInput("IsVector4", "Prototype must be vector4 type!")]
        public MPLItemPrototype channelRGBA;
        
        public static bool IsScalar(MPLItemPrototype protptype)
        {
            if (!protptype)
            {
                return true;
            }
            return protptype.itemValueType == MPLItemValueType.Scalar;
        }
        
        public static bool IsVector3(MPLItemPrototype protptype)
        {
            if (!protptype)
            {
                return true;
            }
            return protptype.itemValueType == MPLItemValueType.Vector3;
        }
        
        public static bool IsVector4(MPLItemPrototype protptype)
        {
            if (!protptype)
            {
                return true;
            }
            return protptype.itemValueType == MPLItemValueType.Vector4;
        }
    }
        
    [CreateAssetMenu(fileName = "propertylut_templete.asset", menuName = CoreUtils.CreateAssetMenuPrefix + "Material Property LUT/Templete", order = CoreUtils.EditorPriority.Default)]
    public class MPLTemplete : ScriptableObject
    {
        public int groupCount = k_DefaultGroupCount;
        public int groupPixelCount = k_DefaultGroupPixelCount;
        
        public UnityEngine.Object targetHLSL;
        
        [PropertyOrder(1)]
        [TableList(ShowPaging = false, ShowIndexLabels = true)]
        public List<MPLItemPrototype> itemPrototypes = new();
        
        [PropertyOrder(2), PropertySpace(30)]
        [TableList(ShowPaging = false, ShowIndexLabels = true)]
        public List<MPLPixelLayout> pixelLayouts = Enumerable.Range(0, k_DefaultGroupPixelCount)
                                                             .Select(i => new MPLPixelLayout())
                                                             .ToList();

        private const int k_DefaultGroupCount = 10;
        private const int k_DefaultGroupPixelCount = 10;

#if UNITY_EDITOR
        [PropertyOrder(1), ButtonGroup("ItemPrototypes")]
        private void AddItemPrototype()
        {
            var prototype = CreateInstance<MPLItemPrototype>();
            prototype.itemName = $"Item{itemPrototypes.Count}";
            prototype.displayName = $"Item{itemPrototypes.Count}";
            prototype.name = $"{prototype.itemName}_{prototype.itemValueType}";

            AssetDatabase.AddObjectToAsset(prototype, this);

            itemPrototypes.Add(prototype);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [PropertyOrder(1), ButtonGroup("ItemPrototypes")]
        private void UpdateItemPrototypes()
        {
            var path = AssetDatabase.GetAssetPath(this);
            var prototypeAssets = AssetDatabase.LoadAllAssetsAtPath(path).Where(obj => obj != this).ToList();
            foreach (var prototypeAsset in prototypeAssets)
            {
                if (!itemPrototypes.Contains(prototypeAsset))
                {
                    DestroyImmediate(prototypeAsset, true);
                }
                else
                {
                    if (prototypeAsset is MPLItemPrototype prototype)
                    {
                        prototype.name = $"{prototype.itemName}_{prototype.itemValueType}";
                    }
                }
            }
            
            // Refresh presets
            var presets = MPLUtils.GetPresets(this);
            foreach (var preset in presets)
            {
                preset.Reinitialize();
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [PropertyOrder(2), ButtonGroup("PixelLayouts")]
        private void UpdatePixelLayouts()
        {
            if (pixelLayouts.Count < groupPixelCount)
            {
                for (var i = pixelLayouts.Count; i < groupPixelCount; i++)
                {
                    pixelLayouts.Add(new MPLPixelLayout());
                }
            }
            else if (pixelLayouts.Count > groupPixelCount)
            {
                pixelLayouts.RemoveRange(groupPixelCount, pixelLayouts.Count - groupPixelCount);
            }
            
            MPLShaderCodeUtils.Generate(this, targetHLSL);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [PropertyOrder(2), ButtonGroup("PixelLayouts")]
        private void UpdateAllAssets()
        {
            // Refresh generators
            var generators = MPLUtils.GetGenerators(this);
            foreach (var generator in generators)
            {
                generator.Reinitialize();
                generator.GenerateTexture();
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }

}