using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Unity.Collections;
using Unity.Mathematics;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
    using System.IO;
    using UnityEditor;
#endif

namespace SnowyOwl.GraphicsFramework
{
    [Serializable]
    public class MPLItemData
    {
        [HideInInspector]
        public MPLItemPrototype prototype;
        [HideInInspector]
        public Vector4 value;
        
        [ShowIf("@prototype.itemType == MPLItemType.Float")]
        [ShowInInspector, EnableGUI, LabelText("@prototype.displayName")]
        public float Float
        {
            get => value.x;
            set => this.value.x = value;
        }
        [ShowIf("@prototype.itemType == MPLItemType.Int")]
        [ShowInInspector, EnableGUI, LabelText("@prototype.displayName")]
        public int Int
        {
            get => (int)value.x;
            set => this.value.x = value;
        }
        [ShowIf("@prototype.itemType == MPLItemType.FloatSlider")]
        [ShowInInspector, EnableGUI, LabelText("@prototype.displayName")]
        [PropertyRange("@prototype.valueMin", "@prototype.valueMax")]
        public float FloatSlider
        {
            get => value.x;
            set => this.value.x = value;
        }
        [ShowIf("@prototype.itemType == MPLItemType.IntSlider")]
        [ShowInInspector, EnableGUI, LabelText("@prototype.displayName")]
        [PropertyRange("@prototype.valueMin", "@prototype.valueMax")]
        public int IntSlider
        {
            get => (int)value.x;
            set => this.value.x = value;
        }
        [ShowIf("@prototype.itemType == MPLItemType.Bool")]
        [ShowInInspector, EnableGUI, LabelText("@prototype.displayName")]
        public bool Bool
        {
            get => value.x.Equals(1);
            set => this.value.x = value ? 1 : 0;
        }
        [ShowIf("@prototype.itemType == MPLItemType.Vector3")]
        [ShowInInspector, EnableGUI, LabelText("@prototype.displayName")]
        [VectorPropertyRange("@prototype.valueMin", "@prototype.valueMax")]
        public Vector3 Vector3
        {
            get => value;
            set => this.value = value;
        }
        [ShowIf("@prototype.itemType == MPLItemType.Color3")]
        [ShowInInspector, EnableGUI, LabelText("@prototype.displayName")]
        [CustomColorUsage(false)]
        public Color Color3
        {
            get => value;
            set => this.value = value;
        }
        [ShowIf("@prototype.itemType == MPLItemType.Vector4")]
        [ShowInInspector, EnableGUI, LabelText("@prototype.displayName")]
        [VectorPropertyRange("@prototype.valueMin", "@prototype.valueMax")]
        public Vector4 Vector4
        {
            get => value;
            set => this.value = value;
        }
        [ShowIf("@prototype.itemType == MPLItemType.Color4")]
        [ShowInInspector, EnableGUI, LabelText("@prototype.displayName")]
        public Color Color4
        {
            get => value;
            set => this.value = value;
        }

        public half GetScalarHalfValue() => new(value.x);
        public half3 GetVector3HalfValue()
        {
            if (MPLUtils.IsColor(prototype.itemType))
            {
                var color = new Color(value.x, value.y, value.z, value.w).linear;
                return new half3(new Vector3(color.r, color.g, color.b));
            }
            return new half3((Vector3)value);
        }
        public half4 GetVector4HalfValue()
        {
            if (MPLUtils.IsColor(prototype.itemType))
            {
                var color = new Color(value.x, value.y, value.z, value.w).linear;
                return new half4(new Vector4(color.r, color.g, color.b, color.a));
            }
            return new half4(value);
        }
    }

    [Serializable]
    public class MPLItemDataWrapper
    {
        [ListDrawerSettings(IsReadOnly = true, DefaultExpandedState = false, ShowPaging = false)]
        public List<MPLItemData> value = new();
    }
    
    [Serializable]
    public class MPLItemGroup
    {
        [TableColumnWidth(100)]
        public string name = "";
        
        [TableColumnWidth(500), HideLabel]
        [ValueDropdown("@MPLGenerator.GetPresetDatas()", AppendNextDrawer = true)]
        public MPLItemDataWrapper itemDatas = new();
    }
    
    public class MPLGenerator : ScriptableObject
    {
        [Sirenix.OdinInspector.ReadOnly]
        public MPLTemplete templete;
        
        [SerializeField, HideInInspector]
        private Texture2D texture;
        
        [ShowInInspector]
        public Texture2D Texture => texture;
        
        [InfoBox("Ctrl+S will automatically refresh the LUT!")]
        [TableList(ShowIndexLabels = true, IsReadOnly = true, AlwaysExpanded = true, ShowPaging = false)]
        [OnValueChanged("OnValueChanged", true)]
        [PropertyOrder(1)]
        public MPLItemGroup[] itemGroups;
        
        private static List<ValueDropdownItem<MPLItemDataWrapper>> s_PresetDatas = new();
        
        public bool isDirty { get; private set; }
        
#if UNITY_EDITOR
        public void Init(MPLTemplete inTemplete)
        {
            templete = inTemplete;

            itemGroups = new MPLItemGroup[templete.groupCount];
            for (var i = 0; i < itemGroups.Length; i++)
            {
                itemGroups[i] = new MPLItemGroup();
                foreach (var prototype in templete.itemPrototypes)
                {
                    var itemData = new MPLItemData
                                   {
                                       prototype = prototype,
                                       value = prototype.defaultValue
                                   };
                    itemGroups[i].itemDatas.value.Add(itemData);
                }
            }
        }
        
        [OnInspectorInit]
        public void Reinitialize()
        {
            UpdateItemGroups();
            CollectPresetDatas(templete);
        }
        
        public void OnValueChanged()
        {
            isDirty = true;
        }

        private void UpdateItemGroups()
        {
            // Adjust the number of groups
            if (itemGroups.Length != templete.groupCount)
            {
                var oldItemGroupCount = itemGroups.Length;
                
                Array.Resize(ref itemGroups, templete.groupCount);
                
                if (oldItemGroupCount < itemGroups.Length)
                {
                    for (var i = oldItemGroupCount; i < itemGroups.Length; i++)
                    {
                        itemGroups[i] = new MPLItemGroup();
                        foreach (var prototype in templete.itemPrototypes)
                        {
                            var itemData = new MPLItemData
                            {
                                prototype = prototype,
                                value = prototype.defaultValue
                            };
                            itemGroups[i].itemDatas.value.Add(itemData);
                        }
                    }
                }
            }

            // Adjust the number of group items
            foreach (var itemGroup in itemGroups)
            {
                var oldItemDatas = itemGroup.itemDatas;
                var newItemDatas = new List<MPLItemData>();
                foreach (var prototype in templete.itemPrototypes)
                {
                    var oldItemDataIndex = oldItemDatas.value.FindIndex(itemData => itemData.prototype == prototype);
                    if (oldItemDataIndex != -1)
                    {
                        newItemDatas.Add(oldItemDatas.value[oldItemDataIndex]);
                    }
                    else
                    {
                        var itemData = new MPLItemData
                        {
                            prototype = prototype,
                            value = prototype.defaultValue
                        };
                        newItemDatas.Add(itemData);
                    }
                }
                itemGroup.itemDatas.value = newItemDatas;
            }
        }

        [ButtonGroup, PropertyOrder(1)]
        public void GenerateTexture()
        {
            isDirty = false;
            
            var shouldAddObjectToAsset = false;
            var width = templete.groupPixelCount;
            var height = templete.groupCount;
            if (!texture)
            {
                texture = new Texture2D(width, height, GraphicsFormat.R16G16B16A16_SFloat, 1, TextureCreationFlags.None);
                shouldAddObjectToAsset = true;
            }
            else
            {
                texture.Reinitialize(width, height);
            }
            texture.name = name;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            texture.anisoLevel = 0;
            
            var pixelDatas = new NativeArray<half4>(width * height, Allocator.Temp);
            for (var groupIdx = 0; groupIdx < height; ++groupIdx)
            {
                var itemDatas = itemGroups[groupIdx].itemDatas.value;
                
                for (var pixelIdx = 0; pixelIdx < width; pixelIdx++)
                {
                    var pixelData = half4.zero;
                    
                    if (pixelIdx < templete.pixelLayouts.Count)
                    {
                        var pixelLayout = templete.pixelLayouts[pixelIdx];

                        if (pixelLayout.layoutType == MPLPixelLayoutType.Vector3AndScalar1)
                        {
                            pixelData.xyz = MPLUtils.GetPixelDataVector3(itemDatas, pixelLayout.channelRGB);
                            pixelData.w = MPLUtils.GetPixelDataScalar(itemDatas, pixelLayout.channelA);
                        }
                        else if (pixelLayout.layoutType == MPLPixelLayoutType.Scalar4)
                        {
                            pixelData.x = MPLUtils.GetPixelDataScalar(itemDatas, pixelLayout.channelR);
                            pixelData.y = MPLUtils.GetPixelDataScalar(itemDatas, pixelLayout.channelG);
                            pixelData.z = MPLUtils.GetPixelDataScalar(itemDatas, pixelLayout.channelB);
                            pixelData.w = MPLUtils.GetPixelDataScalar(itemDatas, pixelLayout.channelA);
                        }
                        else //(pixelLayout.layoutType == MPLPixelLayoutType.Vector4)
                        {
                            pixelData.xyzw = MPLUtils.GetPixelDataVector4(itemDatas, pixelLayout.channelRGBA);
                        }
                    }
                    pixelDatas[groupIdx * width + pixelIdx] = pixelData;
                }
            }
            
            texture.SetPixelData(pixelDatas, 0);
            texture.Apply();
            
            pixelDatas.Dispose();

            if (shouldAddObjectToAsset)
            {
                AssetDatabase.AddObjectToAsset(texture, this);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            CollectPresetDatas(templete);
        }

        [ButtonGroup, PropertyOrder(1)]
        public void ExportToEXR()
        {
            GenerateTexture();
            
            var currentDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this))!;
            var absolutePath = EditorUtility.SaveFilePanel("保存文件", currentDir, name, "exr");

            if (string.IsNullOrEmpty(absolutePath))
            {
                return;
            }
            
            File.WriteAllBytes(absolutePath, texture.EncodeToEXR());
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var relativePath = CommonUtils.AbsoluteToRelativePath(absolutePath);
            var tImporter = AssetImporter.GetAtPath(relativePath) as TextureImporter;
            if (tImporter)
            {
                tImporter.mipmapEnabled = false;
                tImporter.sRGBTexture = false;
                tImporter.npotScale = TextureImporterNPOTScale.None;
                tImporter.wrapMode = TextureWrapMode.Clamp;
                tImporter.filterMode = FilterMode.Point;
                tImporter.anisoLevel = 0;
                
                var defaultSetting = tImporter.GetDefaultPlatformTextureSettings();
                defaultSetting.format = TextureImporterFormat.RGBAHalf;
                tImporter.SetPlatformTextureSettings(defaultSetting);
                AssetDatabase.ImportAsset(relativePath);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private static void CollectPresetDatas(MPLTemplete templete)
        {
            s_PresetDatas ??= new List<ValueDropdownItem<MPLItemDataWrapper>>();
            s_PresetDatas.Clear();

            var presets = MPLUtils.GetPresets(templete);

            foreach (var data in presets.SelectMany(perset => perset.datas))
            {
                s_PresetDatas.Add(new ValueDropdownItem<MPLItemDataWrapper>(data.name, new MPLItemDataWrapper
                {
                    value = data.value
                }));
            }
        }
        private static IEnumerable GetPresetDatas()
        {
            return s_PresetDatas;
        }
#endif
    }
    
#if UNITY_EDITOR
    public class MPLGeneratorModificationProcessor : AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                var generator = AssetDatabase.LoadMainAssetAtPath(path) as MPLGenerator;
                if (generator && generator.isDirty)
                {
                    generator.GenerateTexture();
                }
            }
            return paths;
        }
    }
#endif
}