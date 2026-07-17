#pragma once

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/BRDF.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/CommonData.hlsl"
#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/CommonInput.hlsl"


// ------------------------------------------------
// Math Functions
// acos快速拟合版本, 4阶多项式逼近, 4 VGRP, 16 ALU, 7*10^-5弧度精度
// https://zhuanlan.zhihu.com/p/130428432
// Ref: Handbook of Mathematical Functions (chapter : Elementary Transcendental Functions), M. Abramowitz and I.A. Stegun, Ed.
float ACosFast4(float in_cos)
{
    float x1 = abs(in_cos);
    float x2 = x1 * x1;
    float x3 = x2 * x1;
    float s = -0.2121144f * x1 + 1.5707288f;
    s = 0.0742610f * x2 + s;
    s = -0.0187293f * x3 + s;
    s = sqrt(1.0f - x1) * s;

    return in_cos >= 0.0f ? s : 3.1415926535897932384626433f - s;
}

float3 RotateAroundYInDegrees(float3 vertex, float degrees)
{
    float alpha = degrees * PI / 180.0;
    float sina, cosa;
    sincos(alpha, sina, cosa);
    float2x2 m = float2x2(cosa, -sina, sina, cosa);
    return float3(mul(m, vertex.xz), vertex.y).xzy;
}


// ------------------------------------------------
// Common Functions
void SwyoInitializeTextureData(float2 uv, out SwyoTextureData data)
{
    data = (SwyoTextureData)0;
    data.baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
    
#if defined(_LIGHTING_MASK_ON)
    data.lightingMask = SAMPLE_TEXTURE2D(_LightingMask, sampler_LightingMask, uv);
#endif
    
#if defined(_NORMALMAP_ON)
    data.normalMap = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv);
#endif
}

InputData SwyoInputDataConvertToURP(SwyoInputData swyoData)
{
    InputData urpData = (InputData)0;

    urpData.positionCS = swyoData.positionCS;
    urpData.positionWS = swyoData.positionWS;
    urpData.normalWS = swyoData.normalWS;
    urpData.viewDirectionWS = swyoData.viewDirectionWS;
    urpData.shadowCoord = swyoData.shadowCoord;
    urpData.bakedGI = swyoData.bakedGI;
    urpData.normalizedScreenSpaceUV = swyoData.normalizedScreenSpaceUV;
    urpData.shadowMask = swyoData.shadowMask;
    urpData.tangentToWorld = swyoData.tangentToWorld;
#if defined(DEBUG_DISPLAY)
    urpData.staticLightmapUV = swyoData.staticLightmapUV;
    urpData.vertexSH = swyoData.vertexSH;
#endif

    return urpData;
}

SurfaceData SwyoSurfaceDataConvertToURP(SwyoSurfaceData swyoData)
{
    SurfaceData urpData = (SurfaceData)0;
    
    urpData.albedo = swyoData.albedo;
    urpData.metallic = swyoData.metallic;
    urpData.smoothness = swyoData.smoothness;
    urpData.normalTS = swyoData.normalTS;
    urpData.emission = swyoData.emission;
    urpData.occlusion = swyoData.occlusion;
    urpData.alpha = swyoData.alpha;

    return urpData;
}

half SwyoAlpha(half alpha, half cutoff)
{
#if !defined(ALPHATEST_OFF)
    alpha = AlphaDiscard(alpha, cutoff);
#endif
    
    return alpha;
}

half3 GetNormalTS(float4 normalMap, half scale = half(1.0))
{
#if defined(_NORMALMAP_ON)
    return UnpackNormalScale(normalMap, scale);
#else
    return half3(0.0h, 0.0h, 1.0h);
#endif
}

half3 SampleOverlayCubeMap(TEXTURECUBE_PARAM(cubeMap, cubeMapSampler), half4 cubeMapHDR, half3 tintColor, float3 viewDirectionWS, float3 normalWS, 
                           half smoothness, half scale, half smoothnessOffset, half rotate, half isClamp01 = 0.0)
{
    half sampleSmoothness = saturate(smoothness + smoothnessOffset);
    half mip = PerceptualRoughnessToMipmapLevel(PerceptualSmoothnessToPerceptualRoughness(sampleSmoothness));
    float3 R = reflect(-viewDirectionWS, normalWS);
    R = RotateAroundYInDegrees(R, rotate);
    half4 cubeMapValue = SAMPLE_TEXTURECUBE_LOD(cubeMap, cubeMapSampler, R, mip);
    half3 cubeMapColor = DecodeHDREnvironment(cubeMapValue, cubeMapHDR);
    cubeMapColor = cubeMapColor * scale * tintColor;
    cubeMapColor = lerp(cubeMapColor, saturate(cubeMapColor), isClamp01);
    return cubeMapColor;
}

float2 GetMatcapUV(half3 normalWS, half3 viewDirWS, half3 normalOS)
{
    float3 customZ = -normalize(viewDirWS);
    float3 customX = normalize(cross(mul((float3x3)UNITY_MATRIX_I_V, float3(0, 1, 0)), customZ));
    float3 customY = normalize(cross(customZ, customX));
    float3x3 M = float3x3(
        customX.x, customY.x, customZ.x, 
        customX.y, customY.y, customZ.y, 
        customX.z, customY.z, customZ.z
    );
    float3 normalVS = mul(normalize(normalWS), M);
    float2 matcapUV = normalVS.xy * 0.5 + 0.5;

    return matcapUV;
}

half3 ApplyMatcap(TEXTURE2D_PARAM(matcapMap, matcapMapSampler), float2 uv, half3 inputColor, half3 albedo, half scale, half lerpFactor)
{
    half3 outputColor = inputColor;
#if defined(_PROPERTY_LUT_ON)
    UNITY_BRANCH
    if (LoadPropertyLUT_MatcapMask() > ZERO)
#endif
    {
        half3 matcap = SAMPLE_TEXTURE2D(matcapMap, matcapMapSampler, uv).rgb * scale;
        half3 matcapAdd = inputColor + matcap * albedo;
        half3 matcapMul = inputColor * matcap;
        
        outputColor = lerp(matcapAdd, matcapMul, lerpFactor);
    }
    
    return outputColor;
}

half3 RimLighting(half3 albedo, half3 rimColor, half rimScale, half rimRange, half rimPart, float clampedNV, float3 normalWS, float3 positionWS, half mainLightDiffuse)
{
    half rim = 0.0;
    
#if defined(_RIM_DEPTHOFFSET)
    float3 normalVS = TransformWorldToViewNormal(normalWS);
    float3 positionVS = TransformWorldToView(positionWS);
    float3 offsetPositionVS = float3(positionVS.xy + normalVS.xy * rimRange * 0.02, positionVS.z);  // Remap rim range [0~1] to [0~0.02]
    float2 offsetPositionNDC = ComputeNormalizedDeviceCoordinates(offsetPositionVS, UNITY_MATRIX_P);
    float offsetSceneDepth = SampleSceneDepth(offsetPositionNDC);
    float offsetSceneLinearDepth = LinearEyeDepth(offsetSceneDepth, _ZBufferParams);
    float sceneLinearDepth = -positionVS.z;
    float depthDiff = offsetSceneLinearDepth - sceneLinearDepth;
    rim = step(0.1, depthDiff);
#else
    rim = pow(1.0 - clampedNV, (1.0 - rimRange) * 10);  // Remap rim range [0~1] to [10~0]
    rim = smoothstep(0.0, 1.0, rim);
#endif

    if (any(rimPart))
    {
        rim *= lerp(mainLightDiffuse, 1.0 - mainLightDiffuse, rimPart - RIM_PART_LIGHT_ONLY);
    }

    return albedo * rimColor * rimScale * rim;
}

half UNormToSNorm(half uNormVal)
{
    return (uNormVal * 2.0) - 1.0;
}

half SNormToUNorm(half sNormVal)
{
    return (sNormVal + 1.0) * 0.5;
}

// Normal expand outline in view space, return positionVS
float3 NormalExpandOutlineInVS(float4 positionOS, float3 normalOS, float4 tangentOS, half width, half distanceFactor, half4 smoothedNormalData)
{
#if defined(_OUTLINE_NORMAL_SMOOTHED)
    normalOS.xyz = normalize(normalOS.xyz);
    tangentOS.xyz = normalize(tangentOS.xyz);
    float3 bitangent = cross(normalOS.xyz, tangentOS.xyz) * tangentOS.w * GetOddNegativeScale();
    float3x3 tangentToObject = float3x3(tangentOS.x, bitangent.x, normalOS.x,
                             tangentOS.y, bitangent.y, normalOS.y,
                             tangentOS.z, bitangent.z, normalOS.z);
    float3 smoothedNormalTS = smoothedNormalData.xyz * 2.0 - 1.0;
    float3 smoothedNormalOS = mul(tangentToObject, smoothedNormalTS);
    normalOS = smoothedNormalOS;
    width *= smoothedNormalData.w;
#endif 

    float3 positionWS = TransformObjectToWorld(positionOS.xyz);
    float3 positionVS = TransformWorldToView(positionWS);
    float3 normalVS = TransformWorldToViewNormal(TransformObjectToWorldNormal(normalOS));
    
    // Keeps the outline width constant at different FOVs
    float fovFactor = -(positionVS.z / unity_CameraProjection[1].y);
    
    // Keeps the outline from being too width when it's too far from the camera
    float dist = distance(positionWS, GetCameraPositionWS());
    float distFactor = saturate(pow(dist, -distanceFactor));
    
    float outlineWidth = width * fovFactor * distFactor;
    positionVS.xy += normalVS.xy * outlineWidth;
    return positionVS;
}

// Calculate billboard vertex position, normal and tangent
// billboardMode 0: Spherical
// billboardMode 1: CylinDrical
float3 TransformToBillboard(float4 positionOS, inout float3 normalOS, inout float3 tangentOS, half billboardMode)
{
    // 1. Construct the rotation matrix in viewport space
    // 2. Mode switching
    //UNITY_MATRIX_V[1].xyz == world space camera Up unit vector
    float3 upCamVec = lerp(normalize(UNITY_MATRIX_V._m10_m11_m12), float3(0, 1, 0), billboardMode);
    //UNITY_MATRIX_V[2].xyz == -1 * world space camera Forward unit vector
    float3 forwardCamVec = -normalize(UNITY_MATRIX_V._m20_m21_m22);
    //UNITY_MATRIX_V[0].xyz == world space camera Right unit vector
    float3 rightCamVec = normalize(UNITY_MATRIX_V._m00_m01_m02);
    float4x4 rotationCamMatrix = float4x4(rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1);
    // Convert normal and tangent
    normalOS = normalize(mul(float4(normalOS, 0), rotationCamMatrix)).xyz;
    tangentOS.xyz = normalize(mul(float4(tangentOS.xyz, 0), rotationCamMatrix)).xyz;
    // Get scaling values, the vector lengths of each of the first three rows and columns correspond to the scaling values in the X, Y, and Z axes, respectively.
    float3 positionWS = positionOS.xyz;
    positionWS.x *= length(UNITY_MATRIX_M._m00_m10_m20);
    positionWS.y *= length(UNITY_MATRIX_M._m01_m11_m21);
    positionWS.z *= length(UNITY_MATRIX_M._m02_m12_m22);
    // Below a fixed coordinate system, we use left multiplication; below a non-fixed coordinate system, we use right multiplication.
    // positionWS = mul(vpositionWS, rotationCamMatrix);
    positionWS = (positionWS.x * rotationCamMatrix._m00_m01_m02_m03
                + positionWS.y * rotationCamMatrix._m10_m11_m12_m13
                + positionWS.z * rotationCamMatrix._m20_m21_m22_m23
                + rotationCamMatrix._m30_m31_m32_m33).xyz;
    // The last column is the world coordinates of the centre of the model, added are the offsets of the chi-square coordinates, without them it will be at the origin.
    positionWS.xyz += UNITY_MATRIX_M._m03_m13_m23;
    return positionWS;
}


// -------------------------------------
// Screen Door
void ScreenDoorInternal(half transparency, float2 screenUV, half tiling)
{
    float2 screenPos = screenUV * _ScaledScreenParams.xy * tiling;
    clip(transparency - DitherBayer4x4[(uint(screenPos.x) % 4) * 4 + uint(screenPos.y) % 4]);
}

void ScreenDoor(half transparency, float2 screenUV, half tiling)
{
    UNITY_BRANCH
    if (transparency < 0.99)
    {
        ScreenDoorInternal(transparency, screenUV, tiling);
    }
}