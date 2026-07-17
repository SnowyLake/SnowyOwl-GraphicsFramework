Shader "SnowyOwl GraphicsFramework/Character/Character Stylized Lit"
{
    Properties
    {
        _RasterizationSettings("# Rasterization", Float) = 0
            [KeywordEnum(Opaque, Transparent)] _Surface_Type("SurfaceType", Float) = 0
            [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Blend Source Mode", Float) = 1
            [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Blend Destination Mode", Float) = 0
            [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
            [Enum(Off, 0, On, 1)] _ZWrite("ZWrite Mode", Float) = 1
        
        _BaseSetting("# Base", Float) = 0
            _BaseMapNOTE("!NOTE BaseMap: RGB - BaseColor,  A - Alpha", Float) = 0
            [MainTexture] _BaseMap("BaseMap", 2D) = "white" {}
            [MainColor] _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
            [Toggle(_ALPHATEST_ON)] _AlphaClipOn("Enbale AlphaClip", Float) = 0.0
            _Cutoff("Cutoff [_ALPHATEST_ON]", Range(0.0, 1.0)) = 0.5
        
        _PropertyLUTSetting("# PropertyLUT", Float) = 0
            [Toggle(_PROPERTY_LUT_ON)] _PropertyLUTOn("Enable PropertyLUT", Float) = 0.0
            _PropertyLUT("PropertyLUT & [_PROPERTY_LUT_ON]", 2D) = "black" {}
        
        _LightingSetting("# Lighting", Float) = 0
            _DiffuseStep("DiffuseStep (1.0)", Range(0, 1)) = 1.0
            [Toggle(_LIGHTING_MASK_ON)] _LightingMaskOn("Enable LightingMask", Float) = 0.0
            _LightingMaskNOTE("!NOTE LightingMask: R - Spacular,  G - Smoothness,  B - DiffuseStepOffset,  A - PropertyLUT Mask [_LIGHTING_MASK_ON]", Float) = 0
            _LightingMask("LightMask & [_LIGHTING_MASK_ON]", 2D) = "linearGrey" {}
            _Metallic("Metallic (0.0)", Range(0.0, 2.0)) = 0.0
            _Smoothness("Smoothness (0.5)", Range(0.0, 2.0)) = 0.5
            _Specular("Main Specular (1.0)", Range(0.0, 2.0)) = 1.0
            _ViewSpecular("View Specular (0.0)", Range(0.0, 2.0)) = 0.0
            _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
            [Toggle(_SPECULARHIGHLIGHTS_OFF)] _SpecularHighlightsOff("Disable Specular Highlights", float) = 0.0
        _EmissionSetting("## Emission", Float) = 0
            _EmissionScale("Emission Scale", Range(0, 100)) = 0.0
            _EmissionColor("Emission Color", Color) = (1, 1, 1, 1)
        _NormalMapSetting("## NormalMap", Float) = 0
            [Toggle(_NORMALMAP_ON)] _EnableNormalMap("Enable NormalMap", Float) = 0.0
            [normal]_NormalMap("NormalMap & [_NORMALMAP_ON]", 2D) = "bump" {}
            _NormalScale("Normal Scale (1.0) [_NORMALMAP_ON]", Range(0.0, 2.0)) = 1.0
        
        _ShadowSetting("# Shadow", Float) = 0
            _ShadowScale("Shadow Scale (1.0)", Range(0, 1)) = 1
            [Toggle(_SHADOW_RAMP_ON)] _ShadowRampOn("Enable ShadowRamp", Float) = 0.0
            _ShadowColor("ShadowColor [!_SHADOW_RAMP_ON]", Color) = (0, 0, 0, 1)
            _ShadowRamp("ShadowRamp & [_SHADOW_RAMP_ON]", 2D) = "white" {}
            [Toggle(_SECOND_SHADOW_ON)] _SecondShadowOn("Enable SecondShadow [_SHADOW_RAMP_ON]", Float) = 0.0
            [Toggle(_RECEIVE_SHADOWS_OFF)] _ReceiveShadowsOff("Disable Receive Shadows", Float) = 0.0
        
        _GlobalIlluminationSetting("# Global Illumination", Float) = 0
            [Toggle(_ENVIRONMENTREFLECTIONS_OFF)] _EnvironmentReflectionsOff("Disable Environment Reflections", float) = 1.0
        
        _RimSetting("# Rim", Float) = 0
            [Toggle(_RIM_ON)] _RimOn("Enable Rim", Float) = 0.0
            [KeywordEnum(Fresnel, DepthOffset)] _Rim("Rim Mode [_RIM_ON]", Float) = 0.0
            _RimColor("Rim Color [_RIM_ON]", Color) = (1, 1, 1, 1)
            _RimScale("Rim Scale(1.0) [_RIM_ON]", Range(0.0, 10.0)) = 1.0
            _RimRange("Rim Range(0.5) [_RIM_ON]", Range(0.0, 1.0)) = 0.5
            [Enum(All, 0, LightOnly, 1, DarkOnly, 2)]_RimPart("Rim Part [_RIM_ON]", Float) = 0.0
        
        _MatcapSetting("# Matcap", Float) = 0
            [Toggle(_MATCAP_ON)] _MatcapOn("Enable Matcap", Float) = 0.0
            _MatcapMap("Matcap Map & [_MATCAP_ON]", 2D) = "black" {}
            _MatcapScale("Matcap Scale (1.0) [_MATCAP_ON]", Range(0, 10)) = 1
            _MatcapBlendFactorNOTE("!NOTE Matcap Blend Factor: 0 - Add, 1 - Mul [_MATCAP_ON]", Float) = 0
            _MatcapBlendFactor("Matcap Blend Factor [_MATCAP_ON]", Range(0, 1)) = 0.0
        
        _OutlineSetting("# Outline", Float) = 0
            _OutlineColor("Outline Color", Color) = (0.1, 0.1, 0.1, 1)
            _OutlineWidth("Outline Width(0.005)", Range(0.001, 0.1)) = 0.005
            [Toggle(_OUTLINE_NORMAL_SMOOTHED)] _OutlineNormalSmoothed("Outline Normal Already Smoothed (In Vertex Color RGB)", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "StylizedLit"
        }
        LOD 300
        
        HLSLINCLUDE
            // -------------------------------------
            // SnowyOwl Defines
            #define SWYO_SHADER_CHARACTER_LIT_STYLIZED
            #define SWYO_BXDF_STYLIZED
            #pragma target 4.5
        ENDHLSL
        
        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            
            // -------------------------------------
            // SnowyOwl Material Keywords
            #pragma shader_feature_local _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local _PROPERTY_LUT_ON
            #pragma shader_feature_local _LIGHTING_MASK_ON
            #pragma shader_feature_local _NORMALMAP_ON
            #pragma shader_feature_local _SHADOW_RAMP_ON
            #pragma shader_feature_local _SECOND_SHADOW_ON
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local _RIM_ON
            #pragma shader_feature_local _RIM_DEPTHOFFSET
            #pragma shader_feature_local _MATCAP_ON

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ _FORWARD_PLUS

            // -------------------------------------
            // Editor Only keywords
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma shader_feature_local _PROPERTY_LUT_EDITING

            // -------------------------------------
            // Shader Stages
            #pragma vertex ForwardLitVertex
            #pragma fragment ForwardLitFragment

            // -------------------------------------
            // Includes
            #include_with_pragmas "Packages/com.snowyowl.graphicsframework/Shaders/Include/Passes/ForwardLitAndGBufferPass.hlsl"
            ENDHLSL
        }

        Pass // ShadowCaster Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            
            // -------------------------------------
            // SnowyOwl Defines
            #define SWYO_PASS_SHADOWCASTER

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON
            
            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            // -------------------------------------
            // Shader Stages
            #pragma vertex ShadowCasterVertex
            #pragma fragment ShadowCasterFragment

            // -------------------------------------
            // Includes
            #include_with_pragmas "Packages/com.snowyowl.graphicsframework/Shaders/Include/Passes/ShadowCasterPass.hlsl"
            
            ENDHLSL
        }

        Pass // DepthOnly Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            
            // -------------------------------------
            // SnowyOwl Defines
            #define SWYO_PASS_DEPTHONLY

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON

            // -------------------------------------
            // Shader Stages
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Includes
            #include_with_pragmas "Packages/com.snowyowl.graphicsframework/Shaders/Include/Passes/DepthOnlyPass.hlsl"
            
            ENDHLSL
        }

        Pass // OpaqueOutline Pass
        {
            Name "OpaqueOutline"
            Tags
            {
                "LightMode" = "OpaqueOutline"
            }
            
            // -------------------------------------
            // Render State Commands
            Cull front

            HLSLPROGRAM
            
            // -------------------------------------
            // SnowyOwl Defines
            #define SWYO_PASS_OPAQUE_OUTLINE

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local _OUTLINE_NORMAL_SMOOTHED
            #pragma multi_compile _ _OPAQUE_OUTLINE_COLOR_PASS

            // -------------------------------------
            // Shader Stages
            #pragma vertex OpaqueOutlineVertex
            #pragma fragment OpaqueOutlineFragment

            // -------------------------------------
            // Includes
            #include_with_pragmas "Packages/com.snowyowl.graphicsframework/Shaders/Include/Passes/OpaqueOutlinePass.hlsl"

            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "Needle.MarkdownShaderGUI"
}
