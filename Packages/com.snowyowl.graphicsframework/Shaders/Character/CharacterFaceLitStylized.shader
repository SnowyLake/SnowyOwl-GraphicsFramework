Shader "SnowyOwl GraphicsFramework/Character/Character Stylized Lit Face"
{
    Properties
    {
        _BaseSetting("# Base", float) = 0
            _BaseMapChannnelNOTE("!NOTE BaseMap: RGB - BaseColor,  A - PropertyLutMask", float) = 0
            [MainTexture] _BaseMap("BaseMap", 2D) = "white" { }
            [MainColor] _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
            _AdditionalMask("AdditionalMask & [_ADDITIONAL_MASK_ON]", 2D) = "black" { }
            _DiffuseStep("DiffuseStep", Range(0, 1)) = 0.01
            _DiffuseStepOffset("DiffuseStep Offset", Range(-1.0,  1.0)) = 0.0
            [Toggle(_PROPERTY_LUT_ON)] _PropertyLutOn("Enable PropertyLut", Float) = 0.0
            _PropertyLut("PropertyLut & [_PROPERTY_LUT_ON]", 2D) = "black" { }
        
        _LightSetting("# Lighting", float) = 0
            [Toggle(_LIGHTMASK_ON)] _LightMaskOn("Enable LightMask", Float) = 0.0
            _LightMaskNOTE("!NOTE LightMask: R - SpacularMask,  G - Smoothness,  B - DiffuseStepOffset,  A - Emission [_LIGHTMASK_ON]", float) = 0
            _LightMask("LightMask & [_LIGHTMASK_ON]", 2D) = "linearGrey" { }
            _Smoothness("Smoothness", Range(0.0, 2.0)) = 1.0
            _Specular("Specular", Range(0.0, 2.0)) = 1.0
            _SpecularColor("Additional Specular Color", Color) = (1, 1, 1, 1)
            [Toggle(_NORMALMAP)] _EnableNormalMap("Enable NormalMap", Float) = 0.0
            _BumpMap("Normal Map & [_NORMALMAP]", 2D) = "bump" { }
            _BumpScale("Normal Scale [_NORMALMAP]", range(0.0, 10.0)) = 1.0
            _EmissionColor("Emission Color", Color) = (1, 1, 1, 1)
            _EmissionScale("Emission Scale", Range(0, 100)) = 0.0
        
        _ShadowSetting("# Shadow", float) = 0
            _ShadowScale("Shadow Scale", Range(0, 1)) = 1
            [Toggle(_SHADOWMAP_ON)] _ShadowMapOn("Enable ShadowMap", Float) = 0.0
            _ShadowColor("ShadowColor [!_SHADOWMAP_ON]", Color) = (0, 0, 0, 1)
            [KeywordEnum(Ramp, SSS)] _ShadowMap("ShadowMap Type [_SHADOWMAP_ON]", Float) = 0.0
            _ShadowSSSMap("SSS Map & [_SHADOWMAP_ON && _SHADOWMAP_SSS]", 2D) = "white" { }
            _ShadowRampMap("Ramp Map & [_SHADOWMAP_ON && _SHADOWMAP_RAMP]", 2D) = "white" { }
            _ShadowColorWeight("ShadowColor Weight", Range(0, 1)) = 0
            [Toggle(_HQSHADOWS_ON)] _HQShadowsOn("Enable HQ Shadows", Float) = 0.0
            [Toggle(_RECEIVE_SHADOWS_OFF)] _DisableReceiveShadows("Disable Receive Shadows", Float) = 1.0
        
        _RimSetting("# Rim", float) = 0
            [Toggle(_RIM_ON)] _RimOn("Enable RimLight", Float) = 0.0
            [KeywordEnum(Fresnel, DepthOffset)] _Rim("Rim Mode [_RIM_ON]", Float) = 0.0
            _RimColor("Rim Color [_RIM_ON]", Color) = (1, 1, 1, 1)
            _RimScale("Rim Scale(1.0) [_RIM_ON]", Range(0.0, 10.0)) = 1.0
            _RimRange("Rim Range(0.5) [_RIM_ON]", Range(0.0, 1.0)) = 0.5
            _RimThreshold("Rim Threshold(0.1) [_RIM_ON]", Range(0.0, 1.0)) = 0.1
        
        _OutlineSetting("# Outline", float) = 0
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
            #define SWYO_SHADER_CHARACTER_FACE_LIT_STYLIZED
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
            #pragma shader_feature_local _PROPERTY_LUT_ON
            #pragma shader_feature_local _ADDITIONAL_MASK_ON
            #pragma shader_feature_local _LIGHTING_MASK_ON
            #pragma shader_feature_local _SHADOWMAP_ON
            #pragma shader_feature_local _SHADOWMAP_RAMP _SHADOWMAP_SSS
            #pragma shader_feature_local _HQSHADOWS_ON
            #pragma shader_feature_local _RIM_ON
            #pragma shader_feature_local _RIM_FRESNEL _RIM_DEPTHOFFSET
            
            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ _FORWARD_PLUS

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

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
            // Shader Stages
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Includes
            #include_with_pragmas "Packages/com.snowyowl.graphicsframework/Shaders/Include/Passes/DepthOnlyPass.hlsl"
            
            ENDHLSL
        }

        Pass // Outline Pass
        {
            Name "Outline"
            Tags
            {
                "LightMode" = "Outline"
            }
            
            // -------------------------------------
            // Render State Commands
            Cull front

            HLSLPROGRAM

            // -------------------------------------
            // SnowyOwl Material Keywords
            #pragma shader_feature_local _OUTLINE_NORMAL_SMOOTHED

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
