using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SnowyOwl.GraphicsFramework
{
    public class SetupRenderingVolumeRendererFeature : BaseRendererFeature
    {
        [Serializable]
        public class Settings
        {
            [NonSerialized] public RenderPassEvent passEvent;
            public List<RendererFeatureCameraType> cameraTypes;
        }

        public Settings settings = new()
        {
            passEvent = RenderPassEvent.BeforeRendering,
            cameraTypes = new List<RendererFeatureCameraType>
            {
                RendererFeatureCameraType.MainCamera,
                RendererFeatureCameraType.RTCamera,
            }
        };
        
        private SetupRenderingVolumeRenderPass m_Pass;
        
        public override void Create()
        {
            BeforeCreate();
            m_Pass = new SetupRenderingVolumeRenderPass(settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!enable || !RendererFeatureUtils.CheckCameraType(renderingData.cameraData, settings.cameraTypes))
            {
                return;
            }
            renderer.EnqueuePass(m_Pass);
        }

        protected override void Dispose(bool disposing)
        {
            m_Pass?.Dispose(disposing);
            m_Pass = null;
        }
    }
    
    public class SetupRenderingVolumeRenderPass : BaseRenderPass
    {
        private class PassData
        {
            public VolumeStack volumeStack;
        }
        private readonly PassData m_PassData = new();
        private readonly ProfilingSampler m_ProfilingSampler = new(nameof(SetupRenderingVolumeRenderPass));

        public SetupRenderingVolumeRenderPass(SetupRenderingVolumeRendererFeature.Settings settings)
        {
            renderPassEvent = settings.passEvent;
        }
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            m_PassData.volumeStack = VolumeManager.instance.stack;

            var cmd = RenderingUtils.GetCommandBuffer();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                ExecutePass(cmd, m_PassData);
            }
            RenderingUtils.ExecuteAndReleaseCommandBuffer(context, cmd);
        }

        private static void ExecutePass(CommandBuffer cmd, PassData passData)
        {
            var characterLighting = passData.volumeStack.GetComponent<SwyoCharacterLighting>();
            cmd.SetGlobalFloat(SwyoShaderPropertyId.CharacterDirectIntensity, characterLighting.directIntensity.value);
            cmd.SetGlobalFloat(SwyoShaderPropertyId.CharacterIndirectIntensity, characterLighting.indirectIntensity.value);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {

        }
        
        public override void Dispose(bool disposing)
        {
            
        }
    }
}
