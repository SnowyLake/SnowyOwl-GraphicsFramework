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
                
                int blendMode = (int)material.GetFloat(selfProperty.name);
                switch (blendMode)
                {
                    case (int)MaterialBlendMode.Alpha:
                        material.SetFloat(SwyoShaderPropertyId.SrcBlend, (int)BlendMode.SrcAlpha);
                        material.SetFloat(SwyoShaderPropertyId.DstBlend, (int)BlendMode.OneMinusSrcAlpha);
                        break;
                    case (int)MaterialBlendMode.Additive:
                        material.SetFloat(SwyoShaderPropertyId.SrcBlend, (int)BlendMode.SrcAlpha);
                        material.SetFloat(SwyoShaderPropertyId.DstBlend, (int)BlendMode.One);
                        break;
                    default:
                        // Custom Mode
                        break;
                }
            }
            else
            {
                material.SetFloat(SwyoShaderPropertyId.SrcBlend, (int)BlendMode.One);
                material.SetFloat(SwyoShaderPropertyId.DstBlend, (int)BlendMode.Zero);
            }
        }
    }
}