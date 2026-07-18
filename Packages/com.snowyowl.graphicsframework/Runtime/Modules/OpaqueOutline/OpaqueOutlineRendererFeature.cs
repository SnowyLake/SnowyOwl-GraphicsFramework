using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SnowyOwl.GraphicsFramework
{
    public class OpaqueOutlineRendererFeature : BaseRendererFeature
    {
        [Serializable]
        public class Settings : RendererFeatureSettings
        {
            [Range(0.0f, 1.0f)]
            public float distanceFadeFactor = 0.1f;
            
            [NonSerialized]
            public RenderPassEvent depthPassEvent;
        }

        public Settings settings = new()
        {
            isOpaque = true,
            passTags = new List<string> { "OpaqueOutline" },
            passEvent = RenderPassEvent.AfterRenderingOpaques,
            depthPassEvent = RenderPassEvent.AfterRenderingPrePasses,
        };
        
        private OpaqueOutlineRenderPass m_ColorPass;
        private OpaqueOutlineRenderPass m_DepthPass;
        
        public override void Create()
        {
            BeforeCreate();
            m_ColorPass = new OpaqueOutlineRenderPass(settings, false);
            m_DepthPass = new OpaqueOutlineRenderPass(settings, true);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!enable || !RendererFeatureUtils.CheckCameraType(renderingData.cameraData, settings.filter.cameraTypes))
            {
                return;
            }
            renderer.EnqueuePass(m_ColorPass);
            var urpRenderer = (renderer as UniversalRenderer)!;
            if (urpRenderer.CopyDepthMode == CopyDepthMode.ForcePrepass || urpRenderer.depthPrimingMode == DepthPrimingMode.Forced)
            {
                renderer.EnqueuePass(m_DepthPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            m_ColorPass?.Dispose(disposing);
            m_DepthPass?.Dispose(disposing);
            m_ColorPass = null;
            m_DepthPass = null;
        }
    }
    
    public class OpaqueOutlineRenderPass : BaseRenderPass
    {
        private class PassData
        {
            public RendererList rendererList;

            public bool isDepthPass;
            public float distanceFadeFactor;
        }

        private readonly PassData m_PassData = new();

        private readonly OpaqueOutlineRendererFeature.Settings m_Settings;
        private readonly ProfilingSampler m_ProfilingSampler;
        private readonly FilteringSettings m_FilteringSettings;
        private RenderStateBlock m_RenderStateBlock;
        private readonly List<ShaderTagId> m_PassTags;
        private readonly bool m_IsDepthPass;

        public OpaqueOutlineRenderPass(OpaqueOutlineRendererFeature.Settings settings, bool isDepthPass)
        {
            m_Settings = settings;
            m_IsDepthPass = isDepthPass;
            m_ProfilingSampler = new ProfilingSampler(m_IsDepthPass ? "OpaqueOutline Depth Pass" : "OpaqueOutline Color Pass");
            var renderQueueRange = settings.isOpaque ? RenderQueueRange.opaque : RenderQueueRange.transparent;
            m_FilteringSettings = new FilteringSettings(renderQueueRange, settings.filter.layerMask, settings.filter.renderingLayerMask);
            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            m_PassTags = settings.passTags.Select(tag => new ShaderTagId(tag)).ToList();
            renderPassEvent = isDepthPass ? settings.depthPassEvent : settings.passEvent;
        }
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var renderer = (renderingData.cameraData.renderer as UniversalRenderer)!;

            if (m_IsDepthPass)
            {
                if (renderer.useDepthPriming && (renderingData.cameraData.renderType == CameraRenderType.Base || renderingData.cameraData.clearDepth))
                {
                    ConfigureTarget(renderer.cameraDepthTargetHandle);
                }
                else
                {
                    ConfigureTarget(renderer.DepthTexture);
                }
            }
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var sortFlags = m_Settings.isOpaque ? renderingData.cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;

            if (!m_IsDepthPass)
            {
                if (renderingData.cameraData.renderer.useDepthPriming && m_Settings.isOpaque && (renderingData.cameraData.renderType == CameraRenderType.Base || renderingData.cameraData.clearDepth))
                {
                    m_RenderStateBlock.depthState = new DepthState(false, CompareFunction.Equal);
                    m_RenderStateBlock.mask |= RenderStateMask.Depth;
                    
                    sortFlags = SortingCriteria.SortingLayer | SortingCriteria.RenderQueue | SortingCriteria.OptimizeStateChanges | SortingCriteria.CanvasOrder;
                }
                else if (m_RenderStateBlock.depthState.compareFunction == CompareFunction.Equal)
                {
                    m_RenderStateBlock.depthState = new DepthState(true, CompareFunction.LessEqual);
                    m_RenderStateBlock.mask |= RenderStateMask.Depth;
                }
            }
            
            var drawingSettings = CreateDrawingSettings(m_PassTags, ref renderingData, sortFlags);

            RenderingUtils.CreateRendererList(context, ref renderingData.cullResults, drawingSettings, m_FilteringSettings, m_RenderStateBlock, ref m_PassData.rendererList);
            
            m_PassData.isDepthPass = m_IsDepthPass;
            m_PassData.distanceFadeFactor = m_Settings.distanceFadeFactor;

            var cmd = RenderingUtils.GetCommandBuffer();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                ExecutePass(cmd, m_PassData);
            }
            RenderingUtils.ExecuteAndReleaseCommandBuffer(context, cmd);
        }

        private static void ExecutePass(CommandBuffer cmd, PassData passData)
        {
            RenderingUtils.SetGlobalKeyword(cmd, SwyoShaderKeywords.OpaqueOutlineColorPass, !passData.isDepthPass);
            cmd.SetGlobalFloat(SwyoShaderPropertyId.OpaqueOutlineDistanceFadeFactor, passData.distanceFadeFactor);
            cmd.DrawRendererList(passData.rendererList);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            RenderingUtils.SetGlobalKeyword(cmd, SwyoShaderKeywords.OpaqueOutlineColorPass, false);
        }
        
        public override void Dispose(bool disposing)
        {
            
        }
    }
}
