using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SnowyOwl.GraphicsFramework
{
    [Serializable]
    public class RendererFeatureFilter
    {
        public LayerMask layerMask = LayerMaskUtils.Everything;
        public RenderingLayerMask renderingLayerMask = RenderingLayerMask.Nothing;
        public List<RendererFeatureCameraType> cameraTypes = new() { RendererFeatureCameraType.MainCamera };
    }
    
    [Serializable]
    public class RendererFeatureSettings
    {
        public RendererFeatureFilter filter = new();

        // Hardcode in Custom RendererFeature
        [NonSerialized] public bool isOpaque;
        [NonSerialized] public List<string> passTags;
        [NonSerialized] public RenderPassEvent passEvent;
    }

    public enum RendererFeatureCameraType
    {
        None = -1,

        MainCamera,     // Regular Base Camera, Scene Rendering
        RTCamera,       // Special Base Camera, separate from the CameraStack, RenderTarget is a Custom RT
        OverlayCamera,  // Regular Overlay Camera, such as Character close-up Camera
        UICamera,       // Special Overlay Camera, UI Rendering, it should be at the end of the CameraStack
    }

    public static class RendererFeatureCameraDefines
    {
        private const string k_MainCamera = "MainCamera";
        private const string k_RTCamera = "RTCamera";
        private const string k_OverlayCamera = "OverlayCamera";
        private const string k_UICamera = "UICamera";
        
        public static readonly List<string> Names = new() { k_MainCamera, k_RTCamera, k_OverlayCamera, k_UICamera };
    }
    
    public static class RendererFeatureUtils
    {
        public static bool IsGameCamera(Camera camera)
        {
            return camera.cameraType == CameraType.Game;
        }
        public static bool IsSceneViewCamera(Camera camera)
        {
            return camera.cameraType == CameraType.SceneView;
        }
        
        public static RendererFeatureCameraType GetCameraType(Camera camera)
        {
            return (RendererFeatureCameraType)RendererFeatureCameraDefines.Names.IndexOf(camera.tag);
        }
        
        public static bool CheckCameraType(in CameraData cameraData, List<RendererFeatureCameraType> cameraTypes, bool ifSceneViewCamera = true)
        {
            var camera = cameraData.camera;
            if (IsSceneViewCamera(camera) && ifSceneViewCamera)
            {
                return true;
            }

            if (IsGameCamera(camera) && camera.enabled)
            {
                return cameraTypes.Any(type => GetCameraType(camera) == type);
            }
            
            return false;
        }
        
        public static T GetRendererFeature<T>(int rendererIndex = 0) where T : ScriptableRendererFeature
        {
            var urpAsset = CoreUtils.GetURPAsset();
            var rendererData = urpAsset?.RendererDatas[rendererIndex];
            if (rendererData)
            {
                foreach (var feature in rendererData.rendererFeatures)
                {
                    if (feature is T typedFeature)
                    {
                        return typedFeature;
                    }
                }
            }
            return null;
        }
    }
}
