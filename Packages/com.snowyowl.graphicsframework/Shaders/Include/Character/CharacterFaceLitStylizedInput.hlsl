#ifndef SWYO_CHARACTER_STYLIZED_LIT_INPUT_INCLUDED
#define SWYO_CHARACTER_STYLIZED_LIT_INPUT_INCLUDED

// -------------------------------------
// Include
#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Character/CharacterUtils.hlsl"


// -------------------------------------
// CBuffer
CBUFFER_START(UnityPerMaterial)
    float4 _BaseMap_ST;
    // Colors
    half4 _BaseColor;
    half4 _SpecularColor;
    half4 _ShadowColor;
    half4 _EmissionColor;
    half4 _RimColor;
    half4 _OutlineColor;

    // Stylized Diffuse
    half _DiffuseStep;
    half _DiffuseStepOffset;

    // Stylized Specular
    half _SpecularScale;

    // Stylized Shadow
    half _ShadowScale;
    half _ShadowColorWeight;

    // PBR
    half _Smoothness;
    half _Specular;
    half _NormalScale;
    half _EmissionScale;

    // Rim
    half _RimScale;
    half _RimRange;
    half _RimPart;

    // Outline
    half _OutlineWidth;
CBUFFER_END

#define _Metallic 0.0
#define _Occlusion 1.0
#define _Cutoff 1.0

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
    outSurfaceData.metallic = _Metallic;
    outSurfaceData.smoothness = saturate(lightingMask.g * _Smoothness); 
    outSurfaceData.specular = _Specular;
    outSurfaceData.occlusion = _Occlusion;
    outSurfaceData.emission = lightingMask.a * _EmissionScale * _EmissionColor.rgb;
    outSurfaceData.normalTS = GetNormalTS(textureData.normalMap, _NormalScale);

    outSurfaceData.directIntensity = _CharacterDirectIntensity;
    outSurfaceData.indirectDiffuseIntensity = _CharacterIndirectIntensity;
    outSurfaceData.indirectSpecularIntensity = 0.0;
}

void SwyoInitializeAdditionalData(float4 uv, SwyoInputData inputData, SwyoTextureData textureData, SwyoSurfaceData surfaceData, SwyoLightContext mainLightCtx, out SwyoAdditionalData outAdditionalData)
{
    outAdditionalData = (SwyoAdditionalData)0;
    
    outAdditionalData.diffuseStep = _DiffuseStep;
#if defined(_LIGHTING_MASK_ON)
    outAdditionalData.diffuseStepOffset = StylizedFaceDiffuse(TEXTURE2D_ARGS(_LightingMask, sampler_LightingMask), uv.xy, mainLightCtx.light.direction, _DiffuseStep, _DiffuseStepOffset);
#else
    outAdditionalData.diffuseStepOffset = 1;
#endif
    outAdditionalData.specularColor = _SpecularColor.rgb;
    outAdditionalData.shadowScale = _ShadowScale;
}

#endif // SWYO_CHARACTER_STYLIZED_LIT_INPUT_INCLUDED