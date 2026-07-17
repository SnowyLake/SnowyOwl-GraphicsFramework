using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SnowyOwl.GraphicsFramework
{
    public enum SwyoRenderQueueMode
    {
        Auto = 0,
        Custom = 1,
    }
    public enum SwyoBlendMode
    {
        /// <summary>
        /// Use this for alpha blend mode.
        /// </summary>
        Alpha,   // Old school alpha-blending mode, fresnel does not affect amount of transparency

        /// <summary>
        /// Use this for premultiply blend mode.
        /// </summary>
        Premultiply, // Physically plausible transparency mode, implemented as alpha pre-multiply

        /// <summary>
        /// Use this for additive blend mode.
        /// </summary>
        Additive,

        /// <summary>
        /// Use this for multiply blend mode.
        /// </summary>
        Multiply
    }
    
    public static class MaterialMeshUtils
    {
        // Material Functions
        public static bool IsMaterialTransparent(Material material)
        {
            if (material == null)
            {
                return false;
            }
            
            bool isTransparent = false;
            if (material.HasProperty(SwyoShaderPropertyId.Surface_Type))
                isTransparent |= (int)material.GetFloat(SwyoShaderPropertyId.Surface_Type) == 1;
            isTransparent |= material.shader.FindSubshaderTagValue(0, SwyoShaderTagId.RenderType) == SwyoShaderTagId.Transparent;
            isTransparent |= material.renderQueue >= RenderQueueRange.transparent.lowerBound;
                
            return isTransparent;
        }

        public static void SetMaterialRenderQueue(Material material)
        {
            
        }
        
        public static Material GetSharedMaterial(GameObject obj)
        {
            if (obj.TryGetComponent<Renderer>(out var r))
            {
                return r.sharedMaterial;
            }
            Debug.LogError($"MaterialMeshUtils.GetSharedMaterial Error, GameObject {obj.name} don't have shared material!");
            return null;
        }
        
        public static MaterialPropertyBlock GetMaterialOverride(Renderer renderer)
        {
            var mpb = new MaterialPropertyBlock();
            if (renderer)
            {
                renderer.GetPropertyBlock(mpb);
                return mpb;
            }
            Debug.LogError("MaterialMeshUtils.GetMaterialOverride: renderer is null!");
            return null;
        }
        
        public static void SetMaterialOverride(Renderer renderer, MaterialPropertyBlock mpb)
        {
            if (renderer)
            {
                renderer.SetPropertyBlock(mpb);
            }
            else
            {
                Debug.LogError("MaterialMeshUtils.SetMaterialOverride Error, renderer is null!");
            }
        }
        
        public static bool HasMaterialOverride(Renderer renderer)
        {
            if (renderer)
            {
                return renderer.HasPropertyBlock();
            }
            Debug.LogError("MaterialMeshUtils.HasMaterialOverride Error, renderer is null!");
            return false;
        }
        
        public static void RemoveMaterialOverride(Renderer renderer)
        {
            SetMaterialOverride(renderer, null);
        }
        
        
        public static void SetMaterialOverrideValue<T>(Renderer renderer, int nameID, T value)
        {
            using var builder = MaterialOverrideBuilder.Create(renderer);
            builder.SetValue(nameID, value);
        }
        
        public static void SetMaterialOverrideCustomSH(Renderer renderer, bool toggle, in SHCoefficients coefficients = default)
        {
            using var builder = MaterialOverrideBuilder.Create(renderer);
            if (toggle)
            {
                builder.SetValue(SwyoShaderPropertyId.UseCustomSH, 1.0f);
                builder.SetValue(SwyoShaderPropertyId.CustomSHAr, coefficients.SHAr);
                builder.SetValue(SwyoShaderPropertyId.CustomSHAg, coefficients.SHAg);
                builder.SetValue(SwyoShaderPropertyId.CustomSHAb, coefficients.SHAb);
                builder.SetValue(SwyoShaderPropertyId.CustomSHBr, coefficients.SHBr);
                builder.SetValue(SwyoShaderPropertyId.CustomSHBg, coefficients.SHBg);
                builder.SetValue(SwyoShaderPropertyId.CustomSHBb, coefficients.SHBb);
                builder.SetValue(SwyoShaderPropertyId.CustomSHC, coefficients.SHC);
            }
            else
            {
                builder.SetValue(SwyoShaderPropertyId.UseCustomSH, 0.0f);
            }
        }

        public static void SetMaterialOverrideCustomSH(Renderer renderer, bool toggle, in SphericalHarmonicsL2 coefficients = default)
        {
            var shCoefficients = RenderingUtils.GetShaderSHCoefficients(coefficients);
            SetMaterialOverrideCustomSH(renderer, toggle, shCoefficients);
        }
        
        // Mesh Functions
        public static Mesh GetSharedMesh(GameObject obj)
        {
            if (obj.TryGetComponent<MeshFilter>(out var mf))
            {
                return mf.sharedMesh;
            }
            if (obj.TryGetComponent<SkinnedMeshRenderer>(out var smr))
            {
                return smr.sharedMesh;
            }
            Debug.LogError($"MaterialMeshUtils.GetSharedMesh Error, GameObject {obj.name} don't have shared mesh!");
            return null;
        }
    }

    public struct MaterialOverrideBuilder : IDisposable
    {
        public MaterialPropertyBlock Block { get; }
        private Renderer m_Renderer;

        private MaterialOverrideBuilder(Renderer renderer, bool clearBlock)
        {
            Block = new MaterialPropertyBlock();
            m_Renderer = renderer;
            if (m_Renderer)
            {
                m_Renderer.GetPropertyBlock(Block);
                if (clearBlock)
                {
                    Block.Clear();
                }
            }
            else
            {
                Debug.LogError("MaterialOverrideBuilder Error: Renderer is null!");
            }
        }

        public void SetValue<T>(int nameID, T value)
        {
            switch (value)
            {
                case int intValue:
                    Block.SetInteger(nameID, intValue);
                    break;
                case float floatValue:
                    Block.SetFloat(nameID, floatValue);
                    break;
                case Color colorValue:
                    Block.SetColor(nameID, colorValue);
                    break;
                case Vector4 vectorValue:
                    Block.SetVector(nameID, vectorValue);
                    break;
                case Texture textureValue:
                    Block.SetTexture(nameID, textureValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value),"MaterialOverrideBuilder Error: Shader don't support this property type!");
            }
        }
        
        public void Dispose()
        {
            if (m_Renderer && Block != null)
            {
                m_Renderer.SetPropertyBlock(Block);
            }
            else
            {
                Debug.LogError("MaterialOverrideBuilder Error: renderer is null!");
            }
        }
        
        public static MaterialOverrideBuilder Create(Renderer renderer, bool clearBlock = false)
        {
            return new MaterialOverrideBuilder(renderer, clearBlock);
        }
    }
} 