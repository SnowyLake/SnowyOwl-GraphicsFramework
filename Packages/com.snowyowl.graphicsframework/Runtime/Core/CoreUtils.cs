using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
    using System.IO;
    using UnityEditor;
#endif


namespace SnowyOwl.GraphicsFramework
{
    public static class CoreUtils
    {
        public const string FrameworkName = "SnowyOwl GraphicsFramework";
        public const string AssetMenuItemPrefix = "Assets/Create/" + FrameworkName + "/";
        public const string CreateAssetMenuPrefix = FrameworkName + "/";
        public const string EditorMenuItemPrefix = FrameworkName + "/";
        
        public static class EditorPriority
        {
            public const int Core = 50;
            public const int Default = 100;
            public const int Other = 150;
        }
        
        public static UniversalRenderPipelineAsset GetURPAsset()
        {
            return UniversalRenderPipeline.asset;
        }
        
        /// <summary>
        /// Get the preloaded SnowyOwl global graphics settings.
        /// </summary>
        public static SwyoGlobalGraphicsSettings GetGlobalSettings()
        {
            return SwyoGlobalGraphicsSettings.Instance;
        }
        
        public static UniversalRendererData GetRendererData(int index = 0)
        {
            var urpAsset = GetURPAsset();
            if (urpAsset)
            {
                return urpAsset.RendererDatas[index] as UniversalRendererData;
            }

            return null;
        }
        
        public static void SetDirty(Object obj)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(obj);
#endif
        }
    }
}
