#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// -------------------------------------
// Global Variable
half4 _GlobalAdditionalColor;
// Atmosphere Fog
half _AtmosphereFogEnable;


// -------------------------------------
// Texture & Sampler Declaration
TEXTURE2D(_BaseMap);                    SAMPLER(sampler_BaseMap);
float4 _BaseMap_TexelSize;
float4 _BaseMap_MipInfo;

#if defined(_LIGHTING_MASK_ON)
    TEXTURE2D(_LightingMask);           SAMPLER(sampler_LightingMask);
#endif

#if defined(_NORMALMAP_ON)
    TEXTURE2D(_NormalMap);              SAMPLER(sampler_NormalMap);
#endif

#if defined(_REFLECTION_CUBEMAP)
    TEXTURECUBE(_ReflectionCubeMap);    SAMPLER(sampler_ReflectionCubeMap);
    half4 _ReflectionCubeMap_HDR;
#endif

#if defined(_REFLECTION_MATCAP)
    TEXTURE2D(_ReflectionMatcapMap);    SAMPLER(sampler_ReflectionMatcapMap);
#endif

#if defined(_OVERLAY_CUBEMAP_ON)
    TEXTURECUBE(_OverlayCubeMap);       SAMPLER(sampler_OverlayCubeMap);
#endif

#if defined(_MATCAP_ON)
    TEXTURE2D(_MatcapMap);              SAMPLER(sampler_MatcapMap);
#endif




