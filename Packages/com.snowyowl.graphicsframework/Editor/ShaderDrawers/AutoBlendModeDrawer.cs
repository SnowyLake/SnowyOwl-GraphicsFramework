using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using Needle.ShaderGraphMarkdown;

namespace SnowyOwl.GraphicsFramework.Editor
{
    
    public class AutoBlendModeDrawer : MarkdownMaterialPropertyDrawer
    {
        private const string k_DisplayNamePrefix = "!DRAWER AutoBlendMode ";
        private const int k_ParameterCount = 0;
            
        /// <summary>
        /// Draws the blend mode and synchronizes blending for all selected materials.
        /// </summary>
        public override void OnDrawerGUI(MaterialEditor materialEditor, MaterialProperty[] properties, DrawerParameters parameters)
        {
            var material = materialEditor?.target as Material;
            var selfProperty = ShaderDrawerUtils.GetSelfProperty(parameters, properties, k_DisplayNamePrefix);
            if (!material || selfProperty == null)
            {
                return;
            }
            
            var hasSurfaceType = material.HasProperty(SwyoShaderPropertyId.Surface_Type);
            int surfaceType = (int)MaterialSurfaceType.Opaque;
            if (hasSurfaceType)
            {
                surfaceType = (int)material.GetFloat(SwyoShaderPropertyId.Surface_Type);
            }

            if (!hasSurfaceType || surfaceType == (int)MaterialSurfaceType.Transparent)
            {
                var displayName = ShaderDrawerUtils.GetDisyplayName(parameters, string.Empty, k_ParameterCount);
                materialEditor.ShaderProperty(selfProperty, displayName);
            }

            foreach (var target in materialEditor.targets)
            {
                var mat = (Material)target;
                SetupBlendMode(mat, selfProperty.name);
            }
        }

        /// <summary>
        /// Applies URP Lit blend factors and keywords while preserving custom blend factors.
        /// </summary>
        private static void SetupBlendMode(Material material, string propertyName)
        {
            bool transparent = !material.HasProperty(SwyoShaderPropertyId.Surface_Type) || (int)material.GetFloat(SwyoShaderPropertyId.Surface_Type) == (int)MaterialSurfaceType.Transparent;
            int blendMode = (int)material.GetFloat(propertyName);
            bool preserveSpecular = transparent && blendMode is (int)MaterialBlendMode.Alpha or (int)MaterialBlendMode.Additive &&
                                    material.HasProperty(SwyoShaderPropertyId.BlendModePreserveSpecular) && material.GetFloat(SwyoShaderPropertyId.BlendModePreserveSpecular) > 0;
            RenderingUtils.SetLocalKeyword(material, SwyoShaderKeywords.AlphaPremultiplyOn, preserveSpecular);

            if (transparent)
            {
                switch (blendMode)
                {
                    case (int)MaterialBlendMode.Alpha:
                        SetBlendFactors(material, BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha, BlendMode.One, BlendMode.OneMinusSrcAlpha);
                        break;
                    case (int)MaterialBlendMode.Additive:
                        SetBlendFactors(material, BlendMode.SrcAlpha, BlendMode.One, BlendMode.One, BlendMode.One);
                        break;
                    default:
                        // Custom Mode
                        break;
                }
                if (preserveSpecular)
                {
                    material.SetFloat(SwyoShaderPropertyId.SrcBlend, (int)BlendMode.One);
                }
            }
            else
            {
                SetBlendFactors(material, BlendMode.One, BlendMode.Zero, BlendMode.One, BlendMode.Zero);
            }
        }

        /// <summary>
        /// Sets color blend factors and optional independent alpha blend factors.
        /// </summary>
        private static void SetBlendFactors(Material material, BlendMode src, BlendMode dst, BlendMode srcAlpha, BlendMode dstAlpha)
        {
            material.SetFloat(SwyoShaderPropertyId.SrcBlend, (int)src);
            material.SetFloat(SwyoShaderPropertyId.DstBlend, (int)dst);
            if (material.HasProperty(SwyoShaderPropertyId.SrcBlendAlpha))
            {
                material.SetFloat(SwyoShaderPropertyId.SrcBlendAlpha, (int)srcAlpha);
            }
            if (material.HasProperty(SwyoShaderPropertyId.DstBlendAlpha))
            {
                material.SetFloat(SwyoShaderPropertyId.DstBlendAlpha, (int)dstAlpha);
            }
        }
    }
}
