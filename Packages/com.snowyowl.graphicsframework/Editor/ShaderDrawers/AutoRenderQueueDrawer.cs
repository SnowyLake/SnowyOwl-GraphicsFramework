using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using Needle.ShaderGraphMarkdown;

namespace SnowyOwl.GraphicsFramework.Editor
{
    public class AutoRenderQueueDrawer : MarkdownMaterialPropertyDrawer
    {
        private const string k_DisplayNamePrefix = "!DRAWER AutoRenderQueue ";
        private const int k_ParameterCount = 0;
            
        public override void OnDrawerGUI(MaterialEditor materialEditor, MaterialProperty[] properties, DrawerParameters parameters)
        {
            var material = materialEditor?.target as Material;
            var selfProperty = ShaderDrawerUtils.GetSelfProperty(parameters, properties, k_DisplayNamePrefix);
            if (!material || selfProperty == null)
            {
                return;
            }
            
            var displayName = ShaderDrawerUtils.GetDisyplayName(parameters, string.Empty, k_ParameterCount);
            materialEditor.ShaderProperty(selfProperty, displayName);
            
            int renderQueueMode = (int)MaterialRenderQueueMode.Auto;
            if (material.HasProperty(SwyoShaderPropertyId.RenderQueueMode))
            {
                renderQueueMode = (int)material.GetFloat(SwyoShaderPropertyId.RenderQueueMode);
            }
            
            if (renderQueueMode == (int)MaterialRenderQueueMode.Auto)
            {
                int surfaceType = (int)material.GetFloat(selfProperty.name);
                int renderQueueOffset = 0;
                if (material.HasProperty(SwyoShaderPropertyId.RenderQueueOffset))
                {
                    renderQueueOffset = (int)material.GetFloat(SwyoShaderPropertyId.RenderQueueOffset);
                }
                
                if (surfaceType == (int)MaterialSurfaceType.Opaque)
                {
                    int alphaClip = 0;
                    if (material.HasProperty(SwyoShaderPropertyId.AlphaClipOn))
                    {
                        alphaClip = (int)material.GetFloat(SwyoShaderPropertyId.AlphaClipOn);
                    }

                    if (alphaClip == 0)
                    {
                        material.renderQueue = (int)RenderQueue.Geometry;
                        material.SetOverrideTag("RenderType", "Opaque");
                    }
                    else
                    {
                        material.renderQueue = (int)RenderQueue.AlphaTest;
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                    }
                    
                }
                else
                {
                    material.renderQueue = (int)RenderQueue.Transparent;
                    material.SetOverrideTag("RenderType", "Transparent");
                }
                material.renderQueue += renderQueueOffset;
            }
        }
    }
}