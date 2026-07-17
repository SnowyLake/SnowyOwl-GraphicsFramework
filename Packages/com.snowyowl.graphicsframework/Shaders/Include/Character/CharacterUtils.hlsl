#pragma once

#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/CommonUtils.hlsl"
#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/GlobalIllumination.hlsl"

// -------------------------------------
// Global Defines
#define SHADOWRAMP_SAMPLE_OFFSET 0.03125


// -------------------------------------
// Global Variables
half _CharacterDirectIntensity;
half _CharacterIndirectIntensity;
half4 _CharacterCustomMainLightColor;


// -------------------------------------
// Textures Declaration
#if defined(_SHADOW_RAMP_ON)
    TEXTURE2D(_ShadowRamp);      SAMPLER(sampler_ShadowRamp);
#endif

// -------------------------------------
// Sample Texture Functions
half StylizedFaceDiffuse(TEXTURE2D_PARAM(lightMask, lightMaskSampler), float2 uv, float3 lightDirction, half diffuseStep, half diffuseStepOffset)
{
    half faceDiffuse = 1.0;
#if defined(_LIGHTING_MASK_ON)
    #if defined(_SKIN_ON)
        // character skinned mesh (with bone)
        float2 frontDir = normalize(TransformObjectToWorldDir(float3(0, 1, 0)).xz);     // +y
        float2 rightDir = normalize(TransformObjectToWorldDir(float3(0, 0, 1)).xz);     // +z
    #else
        // character static mesh (none bone)
        float2 frontDir = normalize(TransformObjectToWorldDir(float3(0, 0, 1)).xz);     // +z
        float2 rightDir = normalize(TransformObjectToWorldDir(float3(-1, 0, 0)).xz);    // -x
    #endif
    float2 lightDir = normalize(lightDirction.xz);
    float faceRoL = dot(rightDir, lightDir);
    float faceFoL = dot(frontDir, lightDir);
    // Calc face sdf uv, flip based on light dir
    float2 faceUV = uv;
    faceUV.x = (faceRoL >= 0.0) ? faceUV.x : 1 - faceUV.x;
    float faceSDF = SAMPLE_TEXTURE2D(lightMask, lightMaskSampler, faceUV).b;
    // Align lighting angle
    faceFoL = ACosFast4(faceFoL) / PI;
    // Apply diffuse offset and step offset
    faceDiffuse = smoothstep(saturate(faceFoL - diffuseStep), saturate(faceFoL + diffuseStep), faceSDF);
    faceDiffuse += diffuseStepOffset;
#endif
    return faceDiffuse;
}

half4 StylizedShadowRamp(TEXTURE2D_PARAM(shadowRamp, shadowRampSampler), SwyoLightContext mainLightCtx, half diffuseStep, half diffuseStepOffset)
{
    // Calc ShadowRamp UV
    half shadowRampIndex = 0.0;
#if defined(_PROPERTY_LUT_ON)
    // TODO
#endif
    half2 shadowRampUV = half2(0.5, shadowRampIndex);
    
#if defined(SWYO_SHADER_CHARACTER_STYLIZED_LIT_FACE)
    // LitFace does not need SecondShadow, only Sample Ramp U: [0.5~1]
    shadowRampUV.x = data.diffuseStepOffset * 0.5 + 0.5;
#else
    
    // half diffuse = min(mainLightCtx.NL, mainLightCtx.light.shadowAttenuation) + diffuseStepOffset;
    half diffuse = mainLightCtx.NL + diffuseStepOffset;
    
    // diffuse: [0~1], Shadow-Diffuse Gradient Area, Sample Ramp U: [0.5~1]
    if (diffuse >= 0.0)
    {
        shadowRampUV.x = min(smoothstep(0, diffuseStep, saturate(diffuse)), mainLightCtx.light.shadowAttenuation) * 0.5 + 0.5;
    }
    // diffuse: [-1~0], SecondShadow-Shadow Gradient Area, Sample Ramp U: [0~0.5]
#if defined(_SECOND_SHADOW_ON)
    else
    {
        shadowRampUV.x = smoothstep(0, 1, saturate(diffuse + 1)) * 0.5;
    }
#endif
#endif
    
    return SAMPLE_TEXTURE2D(shadowRamp, shadowRampSampler, shadowRampUV);
}