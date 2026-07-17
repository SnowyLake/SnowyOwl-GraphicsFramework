using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SnowyOwl.GraphicsFramework
{
    [Serializable]
    public struct RenderingLayerMask
    {
        public static readonly RenderingLayerMask Nothing = 0u;
        public static readonly RenderingLayerMask Everything = ~0u;
        public static readonly RenderingLayerMask LightLayers = 0xFF;
        
        public const uint LightLayersX = 0xFFFFFFFF;
        
        public uint value;
        
        public static implicit operator uint(RenderingLayerMask mask)
        {
            return mask.value;
        }
        public static implicit operator RenderingLayerMask(uint intVal)
        {
            RenderingLayerMask result = default;
            result.value = intVal;
            return result;
        }
    }
    
    public static class RenderingUtils
    {
        public static void SetGlobalKeyword(string keyword, bool state)
        {
            if (state)
            {
                Shader.EnableKeyword(keyword);
            }
            else
            {
                Shader.DisableKeyword(keyword);
            }
        }
        
        public static void SetGlobalKeyword(CommandBuffer cmd, string keyword, bool state)
        {
            if (state)
            {
                cmd.EnableShaderKeyword(keyword);
            }
            else
            {
                cmd.DisableShaderKeyword(keyword);
            }
        }

        public static void SetLocalKeyword(Material material, string keyword, bool state)
        {
            if (state)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }
        
        public static void SetLocalKeyword(ComputeShader cs, string keyword, bool state)
        {
            if (state)
            {
                cs.EnableKeyword(keyword);
            }
            else
            {
                cs.DisableKeyword(keyword);
            }
        }
        
        public static CommandBuffer GetCommandBuffer(bool uesPool = true)
        {
            return uesPool ? CommandBufferPool.Get() : new CommandBuffer();
        }
        
        public static void ExecuteAndClearCommandBuffer(ScriptableRenderContext ctx, CommandBuffer cmd)
        {
            ctx.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
        
        public static void ExecuteAndReleaseCommandBuffer(ScriptableRenderContext ctx, CommandBuffer cmd, bool isPooled = true)
        {
            ctx.ExecuteCommandBuffer(cmd);
            if (isPooled)
            {
                CommandBufferPool.Release(cmd);
            }
            else
            {
                cmd.Release();
            }
        }
        
        public static SHCoefficients GetShaderSHCoefficients(in SphericalHarmonicsL2 sh)
        {
            return new SHCoefficients(sh);
        }
        
        public static void ToggleRenderingLayerMask(GameObject obj, RenderingLayerMask toggleLayerMask, bool toggle)
        {
            if (obj.TryGetComponent<Renderer>(out var renderer))
            {
                renderer.renderingLayerMask = CommonUtils.ToggleBit(renderer.renderingLayerMask, toggleLayerMask, toggle);
            }
            else
            {
                Debug.LogError($"RenderingUtils.ToggleRenderingLayerMask Error, gameobject {obj.name} is null or don't have renderer!");
            }
        }
        
        public static void ToggleLightRenderingLayerMask(Light light, RenderingLayerMask toggleLayerMask, bool toggle)
        {
            if (!light)
            {
                var lightData = light.GetUniversalAdditionalLightData();
                lightData.renderingLayers = CommonUtils.ToggleBit(lightData.renderingLayers, toggleLayerMask, toggle);
            }
            else
            {
                Debug.LogError($"RenderingUtils.ToggleLightRenderingLayerMask Error, light {light.name} is null!");
            }
        }
        
        public static void ToggleLightShadowLayerMask(Light light, bool enableShadowLayer, RenderingLayerMask toggleLayerMask = default, bool toggle = false)
        {
            if (!light)
            {
                var lightData = light.GetUniversalAdditionalLightData();
                lightData.customShadowLayers = enableShadowLayer;
                if (enableShadowLayer)
                {
                    lightData.shadowRenderingLayers = CommonUtils.ToggleBit(lightData.shadowRenderingLayers, toggleLayerMask, toggle);
                }
            }
            else
            {
                Debug.LogError($"RenderingUtils.ToggleLightShadowRenderingLayerMask Error, light {light.name} is null!");
            }
        }

        public static void CreateRendererList(ScriptableRenderContext context, ref CullingResults cullResults, DrawingSettings ds, FilteringSettings fs, ref RendererList rl)
        {
            var param = new RendererListParams(cullResults, ds, fs);
            rl = context.CreateRendererList(ref param);
        }
        
        // Create a RendererList using a RenderStateBlock override is quite common so we have this optimized utility function for it
        public static void CreateRendererList(ScriptableRenderContext context, ref CullingResults cullResults, DrawingSettings ds, FilteringSettings fs, RenderStateBlock rsb, ref RendererList rl)
        {
            var param = new RendererListParams();
            unsafe
            {
                // Taking references to stack variables in the current function does not require any pinning (as long as you stay within the scope)
                // so we can safely alias it as a native array
                RenderStateBlock* rsbPtr = &rsb;
                var stateBlocks = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<RenderStateBlock>(rsbPtr, 1, Allocator.None);

                var shaderTag = ShaderTagId.none;
                var tagValues = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ShaderTagId>(&shaderTag, 1, Allocator.None);

                // Inside CreateRendererList (below), we pass the NativeArrays to C++ by calling GetUnsafeReadOnlyPtr
                // This will check read access but NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray does not set up the SafetyHandle (by design) so create/add it here
                // NOTE: we explicitly share the handle
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                var safetyHandle = AtomicSafetyHandle.Create();
                AtomicSafetyHandle.SetAllowReadOrWriteAccess(safetyHandle, true);

                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref stateBlocks, safetyHandle);
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref tagValues, safetyHandle);
#endif

                // Create & schedule the RL
                param = new RendererListParams(cullResults, ds, fs)
                {
                    tagValues = tagValues,
                    stateBlocks = stateBlocks
                };

                rl = context.CreateRendererList(ref param);

                // we need to explicitly release the SafetyHandle
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.Release(safetyHandle);
#endif
            }
        }
    }
}
