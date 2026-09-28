using UnityEngine;
using UnityEngine.Rendering;

namespace SnowyOwl.GraphicsFramework
{
    internal static class SetupShaderParametersPerCamera
    {
        /// <summary>
        /// Register a per-camera keyword reset for renderers without the setup feature.
        /// </summary>
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterDepthPrimingKeywordReset()
        {
            RenderPipelineManager.beginCameraRendering -= ResetDepthPrimingKeyword;
            RenderPipelineManager.beginCameraRendering += ResetDepthPrimingKeyword;
        }

        /// <summary>
        /// Clear the previous camera's depth priming keyword before rendering begins.
        /// </summary>
        private static void ResetDepthPrimingKeyword(ScriptableRenderContext context, Camera camera)
        {
            RenderingUtils.SetGlobalKeyword(SwyoShaderKeywords.DepthPrimingOn, false);
        }
    }
}
