#ifndef SWYO_CHARACTER_STYLIZED_LIT_INPUT_INCLUDED
#define SWYO_CHARACTER_STYLIZED_LIT_INPUT_INCLUDED

// -------------------------------------
// Include
#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Generated/PropertyLUTCharacterStylizedDefines.hlsl"
#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Character/CharacterUtils.hlsl"


// -------------------------------------
// CBuffer
CBUFFER_START(UnityPerMaterial)
    // Texture ST
    float4 _BaseMap_ST;

    // Colors
    half4 _BaseColor;
    half4 _SpecularColor;
    half4 _ShadowColor;
    half4 _EmissionColor;
    half4 _RimColor;
    half4 _LocalCubeMap_HDR;
    half4 _LocalCubeMapTintColor;
    half4 _OutlineColor;

    // AlphaTest
    half _Cutoff;

    // Lighting
    half _Metallic;
    half _Smoothness;
    half _Specular;
    half _ViewSpecular;
    half _DiffuseStep;
    half _EmissionScale;
    half _NormalScale;

    // Shadow
    half _ShadowScale;

    // Rim
    half _RimScale;
    half _RimRange;
    half _RimPart;

    // Matcap
    half _MatcapScale;
    half _MatcapBlendFactor;

    // Outline
    half _OutlineWidth;
CBUFFER_END

#define _Occlusion 1.0

// -------------------------------------
// Initialize Function
void SwyoInitializeSurfaceData(out SwyoSurfaceData outSurfaceData, SwyoTextureData textureData)
{
    outSurfaceData = (SwyoSurfaceData)0;
    
    outSurfaceData.albedo = textureData.baseMap.rgb * _BaseColor.rgb;
    outSurfaceData.alpha = textureData.baseMap.a * _BaseColor.a;
#if defined(_LIGHTING_MASK_ON)
    half4 lightingMask = textureData.lightingMask;
#else
    half4 lightingMask = COLOR4_WHITE_ALPHA1;
#endif
    outSurfaceData.metallic = saturate(_Metallic);
    outSurfaceData.smoothness = saturate(lightingMask.g * _Smoothness); 
    outSurfaceData.occlusion = _Occlusion;
    outSurfaceData.emission = lightingMask.a * _EmissionScale * _EmissionColor.rgb;
    outSurfaceData.normalTS = GetNormalTS(textureData.normalMap, _NormalScale);
}

void SwyoInitializeLightingData(out SwyoLightingData outLightingData, SwyoTextureData textureData)
{
    outLightingData = (SwyoLightingData)0;

    half4 lightingMask = COLOR4_WHITE_ALPHA1;
#if defined(_LIGHTING_MASK_ON)
    lightingMask = textureData.lightingMask;
#endif
    outLightingData.primarySpecularScale = lightingMask.r * _Specular;
    outLightingData.indirectDiffuseScale = _CharacterIndirectIntensity;
    outLightingData.indirectSpecularScale = 1.0;
}

void SwyoInitializeAdditionalData(out SwyoAdditionalData outAdditionalData, SwyoLightContext mainLightCtx, SwyoInputData inputData, SwyoSurfaceData surfaceData, SwyoTextureData textureData, float4 uv)
{
    outAdditionalData = (SwyoAdditionalData)0;
    
#if defined(_LIGHTING_MASK_ON)
    half diffuseStepOffset = textureData.lightingMask.b;
#else
    half diffuseStepOffset = 0.5;
#endif
    outAdditionalData.diffuseStep = _DiffuseStep;
    outAdditionalData.diffuseStepOffset = clamp(diffuseStepOffset * 2.0 - 1.0, -1.0, 1.0);
    outAdditionalData.specularColor = _SpecularColor.rgb;
    outAdditionalData.shadowScale = _ShadowScale;
#if defined(_SHADOW_RAMP_ON)
    outAdditionalData.shadowColor = StylizedShadowRamp(TEXTURE2D_ARGS(_ShadowRamp, sampler_ShadowRamp), mainLightCtx, outAdditionalData.diffuseStep, outAdditionalData.diffuseStepOffset);
#else
    outAdditionalData.shadowColor = _ShadowColor.rgb;
#endif
}


#endif