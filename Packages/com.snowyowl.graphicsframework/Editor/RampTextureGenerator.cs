using System.IO;

using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector;

using SnowyOwl;

namespace SnowyOwl.GraphicsFramework.Editor
{
    [CreateAssetMenu(fileName = "RampTexture.asset", menuName = CoreUtils.CreateAssetMenuPrefix + "RampTexture Generator", order = CoreUtils.EditorPriority.Default)]
    public class RampTextureGenerator : ScriptableObject
    {
        public int width = 256;
        public int heightPerRamp = 2;
        public bool autoSave;
        
        [SerializeField, ReadOnly]
        private Texture2D texture;
        
        [ListDrawerSettings(NumberOfItemsPerPage = 20), OnValueChanged("OnValueChanged", true)]
        public Gradient[] gradients = new Gradient[]{};
        
        private int count => gradients.Length;
        private int height => heightPerRamp * count;

        public Texture2D Texture => texture;
        
#if UNITY_EDITOR
        [Button]
        public void GenerateTexture()
        {
            var shouldAddObjectToAsset = false;
            
            if (!texture)
            {
                texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
                {
                    wrapMode = TextureWrapMode.Clamp
                };
                shouldAddObjectToAsset = true;
            }
            else
            {
                texture.Reinitialize(width, height);
            }
            
            texture.name = this.name;
            
            var colors = new Color[width * height];
            for (var i = 0; i < count; ++i)
            {
                var gradient = gradients[i];
                for (var j = 0; j < width; ++j)
                {
                    var t = (j + 0.5f) / width;
                    var color = gradient.Evaluate(t);         
                    for (var k = 0; k < heightPerRamp; ++k)
                    {
                        colors[(i * heightPerRamp + k) * width + j] = color;
                    }
                }
            }
            texture.SetPixels(colors);
            texture.Apply();

            if (shouldAddObjectToAsset)
            {
                AssetDatabase.AddObjectToAsset(texture, this);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void OnValueChanged()
        {
            if (autoSave)
            {
                GenerateTexture();
            }
        }
    #endif
    }
}
