#pragma once

// -------------------------------------
// Include
#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Scene/SceneUtils.hlsl"


// -------------------------------------
// CBuffer
CBUFFER_START(UnityPerMaterial)
    float4 _BaseMap_ST;
    // Colors
    half4 _BaseColor;
    half4 _EmissionColor;
    // AlphaTest
    half _Cutoff;

    // BxDF
    half _Metallic;
    half _Smoothness;
    half _Specular;
    half _Occlusion;
    half _NormalScale;
    half _EmissionScale;

    // GI
    half _IndirectSpecularIntensity;

    // ScreenDoor
    half _ScreenDoorTransparency;
    half _ScreenDoorTiling;
CBUFFER_END


// -------------------------------------
// Initialize Function
void SwyoInitializeSurfaceData(out SwyoSurfaceData outSurfaceData, SwyoTextureData textureData)
{
    outSurfaceData = (SwyoSurfaceData)0;
    
    outSurfaceData.albedo = textureData.baseMap.rgb * _BaseColor.rgb;
    outSurfaceData.alpha = SwyoAlpha(textureData.baseMap.a * _BaseColor.a, _Cutoff);
    
    half4 lightingMask = COLOR4_WHITE_ALPHA1;
#if defined(_LIGHTING_MASK_ON)
    lightingMask = textureData.lightingMask;
#endif
    outSurfaceData.metallic = saturate(lightingMask.r * _Metallic);
    outSurfaceData.smoothness = saturate(lightingMask.g * _Smoothness); 
    outSurfaceData.occlusion = LerpWhiteTo(lightingMask.b, _Occlusion);
    outSurfaceData.emission = outSurfaceData.albedo * _EmissionColor.rgb * _EmissionScale * lightingMask.a;
    
    outSurfaceData.normalTS = GetNormalTS(textureData.normalMap, _NormalScale);
}

void SwyoInitializeLightingData(out SwyoLightingData outLightingData, SwyoTextureData textureData)
{
    outLightingData = (SwyoLightingData)0;

    outLightingData.primarySpecularScale = _Specular;
    outLightingData.indirectDiffuseScale = 1.0;
    outLightingData.indirectSpecularScale = _IndirectSpecularIntensity;
}
