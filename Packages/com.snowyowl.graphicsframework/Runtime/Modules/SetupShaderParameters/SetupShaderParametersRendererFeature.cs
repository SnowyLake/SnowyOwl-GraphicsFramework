using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SnowyOwl.GraphicsFramework
{
    public class SetupShaderParametersRendererFeature : BaseRendererFeature
    {
        [Serializable]
        public class Settings
        {
            public List<RendererFeatureCameraType> cameraTypes;
        }

        public Settings settings = new()
        {
            cameraTypes = new List<RendererFeatureCameraType>
            {
                RendererFeatureCameraType.MainCamera,
                RendererFeatureCameraType.RTCamera,
            }
        };

        /// <summary>
        /// Global shader parameters do not require a render pass.
        /// </summary>
        public override void Create()
        {
        }

        /// <summary>
        /// Apply depth priming and filtered volume values after camera setup.
        /// </summary>
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            var useDepthPriming = renderer is UniversalRenderer universalRenderer
                                  && !UniversalRenderer.IsOffscreenDepthTexture(in renderingData.cameraData)
                                  && universalRenderer.useDepthPriming;
            RenderingUtils.SetGlobalKeyword(SwyoShaderKeywords.DepthPrimingOn, useDepthPriming);

            if (!enable || !RendererFeatureUtils.CheckCameraType(renderingData.cameraData, settings.cameraTypes))
            {
                return;
            }

            var characterLighting = VolumeManager.instance.stack.GetComponent<SwyoCharacterLighting>();
            Shader.SetGlobalFloat(SwyoShaderPropertyId.CharacterDirectIntensity, characterLighting.directIntensity.value);
            Shader.SetGlobalFloat(SwyoShaderPropertyId.CharacterIndirectIntensity, characterLighting.indirectIntensity.value);
        }

        /// <summary>
        /// Keep the renderer feature hook without enqueuing a pass.
        /// </summary>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
        }
    }
}
