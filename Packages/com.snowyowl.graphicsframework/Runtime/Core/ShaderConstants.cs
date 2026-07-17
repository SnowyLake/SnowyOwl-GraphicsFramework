using UnityEngine;
using UnityEngine.Rendering;

namespace SnowyOwl.GraphicsFramework
{
    public static class SwyoShaderPropertyId
    {
        // Common
        public static readonly int Surface_Type = Shader.PropertyToID("_Surface_Type");
        public static readonly int RenderQueueMode = Shader.PropertyToID("_RenderQueueMode");
        public static readonly int RenderQueueOffset = Shader.PropertyToID("_RenderQueueOffset");
        public static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        public static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        public static readonly int AlphaClipOn = Shader.PropertyToID("_AlphaClipOn");
        
        // GI
        public static readonly int UseCustomSH = Shader.PropertyToID("_UseCustomSH");
        public static readonly int CustomSHAr = Shader.PropertyToID("_CustomSHAr");
        public static readonly int CustomSHAg = Shader.PropertyToID("_CustomSHAg");
        public static readonly int CustomSHAb = Shader.PropertyToID("_CustomSHAb");
        public static readonly int CustomSHBr = Shader.PropertyToID("_CustomSHBr");
        public static readonly int CustomSHBg = Shader.PropertyToID("_CustomSHBg");
        public static readonly int CustomSHBb = Shader.PropertyToID("_CustomSHBb");
        public static readonly int CustomSHC  = Shader.PropertyToID("_CustomSHC");
        public static readonly int GlossyEnvironmentColor = Shader.PropertyToID("_GlossyEnvironmentColor");
        public static readonly int GlossyEnvironmentCubeMap = Shader.PropertyToID("_GlossyEnvironmentCubeMap");
        
        // Character Lighting
        public static readonly int CharacterDirectIntensity = Shader.PropertyToID("_CharacterDirectIntensity");
        public static readonly int CharacterIndirectIntensity = Shader.PropertyToID("_CharacterIndirectIntensity");
        
        // Material Property LUT
        public static readonly int PropertyLUTOn = Shader.PropertyToID("_PropertyLUTOn");
        public static readonly int PropertyLUT = Shader.PropertyToID("_PropertyLUT");
        public static readonly int PropertyLUTEditingEnable = Shader.PropertyToID("_PropertyLUTEditingEnable");
        public static readonly int PropertyLUTFunctionCount = Shader.PropertyToID("_PropertyLUTFunctionCount");
        public static readonly int PropertyLUTEditingData = Shader.PropertyToID("_PropertyLUTEditingData");
        
        // Opaque Outline
        public static readonly int OpaqueOutlineDistanceFadeFactor = Shader.PropertyToID("_OpaqueOutlineDistanceFadeFactor");
    }
    
    public static class SwyoShaderTagId
    {
        public static readonly ShaderTagId RenderType = new("RenderType");
        public static readonly ShaderTagId Transparent = new("Transparent");
    }
    
    public static class SwyoShaderKeywords
    {
        public const string MainLightShadows = "_MAIN_LIGHT_SHADOWS";
        
        // Material Property LUT
        public const string PropertyLUTOn = "_PROPERTY_LUT_ON";
        public const string PropertyLUTEditing = "_PROPERTY_LUT_EDITING";
        
        // OpaqueOutline
        public const string OpaqueOutlineColorPass = "_OPAQUE_OUTLINE_COLOR_PASS";
    }
    
    public enum MaterialSurfaceType
    {
        Opaque = 0, Transparent
    }
    public enum MaterialRenderQueueMode
    {
        Auto = 0, Custom
    }
    public enum MaterialBlendMode
    {
        Alpha = 0, Additive, Custom = 9
    }
}
