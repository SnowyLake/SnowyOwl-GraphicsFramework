Shader "SnowyOwl GraphicsFramework/Scene/Scene Lit Standard"
{
    Properties
    {
        _RasterizationSettings("# Rasterization", Float) = 0
            [KeywordEnum(Opaque, Transparent)] _Surface_Type("!DRAWER SwyoRenderQueue SurfaceType", Float) = 0
            [Enum(SnowyOwl.GraphicsFramework.SwyoRenderQueueMode)] _RenderQueueMode("RenderQueue Mode", Float) = 0.0
            [IntRange] _RenderQueueOffset("RenderQueue Offset [_RenderQueueMode == 0]", Range(-50, 50)) = 0
            [Space]
            [Enum(SnowyOwl.GraphicsFramework.SwyoBlendMode)] _BlendMode("Blend Mode", Float) = 0
            [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Blend Source Mode", Float) = 1
            [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Blend Destination Mode", Float) = 0
            [Space]
            [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
            [Enum(Off, 0, On, 1)] _ZWrite("ZWrite Mode", Float) = 1

        _BaseSettings("# Base", Float) = 0
            _BaseMapNOTE("!NOTE BaseMap: RGB - Color,  A - Alpha", float) = 0
            [MainTexture] _BaseMap("BaseMap", 2D) = "white" {}
            [MainColor] _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
            [Toggle(_ALPHATEST_ON)] _AlphaClipOn("Enbale AlphaClip", Float) = 0.0
            _Cutoff("Cutoff [_ALPHATEST_ON]", Range(0.0, 1.0)) = 0.5
            
        _LightingSettings("# Lighting", Float) = 0
            [Toggle(_LIGHTING_MASK_ON)] _LightingMaskOn("Enable LightingMask", Float) = 0.0
            _LightingMaskNOTE("!NOTE LightingMask: R - Metallic,  G - Smoothness,  B - Occlusion,  A - Emission", float) = 0
            _LightingMask("LightingMask &", 2D) = "white" {}
			_Metallic("Metallic", Range(0.0, 2.0)) = 0.0
			_Smoothness("Smoothness", Range(0.0, 2.0)) = 0.5
            _Specular("Specular", Range(0.0, 2.0)) = 1.0
			_Occlusion("Occlusion [_LIGHTING_MASK_ON]", Range(0.0, 1.0)) = 1.0
            [Toggle(_SPECULARHIGHLIGHTS_OFF)] _SpecularHighlightsOff("Disable Specular Highlights", float) = 0.0
            [Space]
            _EmissionScale("Emission Scale", Range(0, 100)) = 0.0
            _EmissionColor("Emission Color", Color) = (1, 1, 1, 1)
            [Space]
            [Toggle(_NORMALMAP_ON)] _NormalMapOn("Enbale NormalMap", Float) = 0.0
            [Normal] _NormalMap("NormalMap &", 2D) = "bump" {}
            _NormalScale("Normal Scale [_NORMALMAP_ON]", Range(0.0, 2.0)) = 1.0
        
        _ShadowSettings("# Shadow", Float) = 0
            [Toggle(_RECEIVE_SHADOWS_OFF)] _ReceiveShadowsOff("Disable Receive Shadows", float) = 0.0

        _GlobalIlluminationSetting("# Global Illumination", Float) = 0
            _IndirectDiffuseIntensity("Indirect Diffuse Intensity", Range(0.0, 1.0)) = 1.0
			_IndirectSpecularIntensity("Indirect Specular Intensity", Range(0.0, 1.0)) = 1.0
            [KeywordEnum(Environment, Cubemap, Matcap, None)] _Reflection("Reflection Source", Float) = 0.0
            _ReflectionCubeMap("Reflection CubeMap & [_REFLECTION_CUBEMAP]", CUBE) = "black" {}
            _ReflectionMatcapMap("Reflection Matcap & [_REFLECTION_MATCAP]", 2D) = "black" {}
        
//        _OverlayCubeMapSetting("# Overlay CubeMap", Float) = 0
//            [Toggle(_OVERLAY_CUBEMAP_ON)] _OverlayCubeMapOn("Enable Overlay CubeMap", Float) = 0.0
//            _OverlayCubeMap("Overlay CubeMap &", CUBE) = "black" {}
//            _OverlayCubeMapTintColor("Overlay CubeMap Tint Color [_OVERLAY_CUBEMAP_ON]", Color) = (1, 1, 1, 1)
//            _OverlayCubeMapScale("Overlay CubeMap Scale [_OVERLAY_CUBEMAP_ON]", Range(0, 10)) = 1
//            _OverlayCubeMapSmoothnessOffset("Overlay CubeMap Smoothness Offset [_OVERLAY_CUBEMAP_ON]", Range(-1, 1)) = 0
//            _OverlayCubeMapRotate("Overlay CubeMap Rotate Angle [_OVERLAY_CUBEMAP_ON]", Range(0, 360)) = 0
        
        _ScreenDoorSettings( "# ScreenDoor", Float) = 1.0
            _ScreenDoorTransparency("ScreenDoor Transparency", Range(0.0, 1.0)) = 1.0
            _ScreenDoorTiling("ScreenDoor Tiling", Range(0.001, 1.0)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "UniversalMaterialType" = "Lit"
            "SwyoBxDF" = "Standard"
        }
        LOD 300
        
        HLSLINCLUDE
            // -------------------------------------
            // SnowyOwl Defines
            #pragma target 4.5
            #define SWYO_SHADER_SCENE_LIT_STARDAND
            #define SWYO_BXDF_STANDARD
            #define _SCREEN_DOOR_ON
            
            // -------------------------------------
            // Global Keywords
            #pragma shader_feature_local _ALPHATEST_ON
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            // -------------------------------------
            // Render State Commands
            Blend [_SrcBlend][_DstBlend], Zero OneMinusSrcAlpha
            ZWrite [_ZWrite]
            Cull [_Cull]

            HLSLPROGRAM
            
            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local _ _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
            #pragma shader_feature_local _LIGHTING_MASK_ON
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _NORMALMAP_ON
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local _ _REFLECTION_CUBEMAP _REFLECTION_MATCAP _REFLECTION_NONE
            // #pragma shader_feature_local _OVERLAY_CUBEMAP_ON

            // -------------------------------------
            // Shader LOD Keywords
            #pragma multi_compile _SCENE_LOD_HIGH _SCENE_LOD_MEDIUM _SCENE_LOD_LOW
            
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
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ LIGHTMAP_ON

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

        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite[_ZWrite]
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM
            
            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _LIGHTING_MASK_ON
            #pragma shader_feature_local _NORMALMAP_ON
            #pragma shader_feature_local _LOCAL_CUBEMAP_ON
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _ENVIRONMENTREFLECTIONS_OFF

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #pragma multi_compile _ _RENDER_PASS_ENABLED
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ LIGHTMAP_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
            
            // -------------------------------------
            // Shader Stages
            #pragma vertex GBufferVertex
            #pragma fragment GBufferFragment

            // -------------------------------------
            // Includes
            #include_with_pragmas "Packages/com.snowyowl.graphicsframework/Shaders/Include/Passes/ForwardLitAndGBufferPass.hlsl"
            
            ENDHLSL
        }

        Pass
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
            Cull [_Cull]

            HLSLPROGRAM

            // -------------------------------------
            // Unity defined keywords
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

        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ColorMask R
            Cull [_Cull]

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
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "Needle.MarkdownShaderGUI"
}
