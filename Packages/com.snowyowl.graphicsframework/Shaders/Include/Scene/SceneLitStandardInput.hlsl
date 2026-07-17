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
    // half4 _OverlayCubeMap_HDR;
    // half4 _OverlayCubeMapTintColor;

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
    half _IndirectDiffuseIntensity;
    half _IndirectSpecularIntensity;

    // Overlay CubeMap
    // half _OverlayCubeMapScale;
    // half _OverlayCubeMapSmoothnessOffset;
    // half _OverlayCubeMapRotate;

    // ScreenDoor
    half _ScreenDoorTransparency;
    half _ScreenDoorTiling;
CBUFFER_END


// -------------------------------------
// Initialize Function
void SwyoInitializeSurfaceData(SwyoTextureData textureData, out SwyoSurfaceData outSurfaceData)
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
    outSurfaceData.specular = _Specular;
    outSurfaceData.occlusion = LerpWhiteTo(lightingMask.b, _Occlusion);
    outSurfaceData.emission = outSurfaceData.albedo * _EmissionColor.rgb * _EmissionScale * lightingMask.a;
    
    outSurfaceData.normalTS = GetNormalTS(textureData.normalMap, _NormalScale);

    outSurfaceData.directIntensity = 1.0;
    outSurfaceData.indirectDiffuseIntensity = _IndirectDiffuseIntensity;
    outSurfaceData.indirectSpecularIntensity = _IndirectSpecularIntensity;
}