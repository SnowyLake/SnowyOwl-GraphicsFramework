#pragma once

// -------------------------------------
// Includes
#include_with_pragmas "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/PassIncludes.hlsl"


// -------------------------------------
// Input
struct DepthOnlyAttributes
{
    float4 positionOS   : POSITION;
    float2 texcoord0    : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct DepthOnlyVaryings
{
    float4 positionCS   : SV_POSITION;
    float3 positionWS   : TEXCOORD0;
    float2 uv0          : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};


// -------------------------------------
// Entry
DepthOnlyVaryings DepthOnlyVertex(DepthOnlyAttributes input)
{
    DepthOnlyVaryings output = (DepthOnlyVaryings)0;
    UNITY_SETUP_INSTANCE_ID(input);

#if defined(_ALPHATEST_ON)
    output.uv0 = TRANSFORM_TEX(input.texcoord0, _BaseMap);
#endif
    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

    return output;
}

half DepthOnlyFragment(DepthOnlyVaryings input) : SV_TARGET
{
#if defined(_ALPHATEST_ON)
    half alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv0).a * _BaseColor.a;
    SwyoAlpha(alpha, _Cutoff);
#endif
   
#if defined(_DYNAMIC_FEATURE_ON)
    #if defined(_SCREEN_DOOR_ON)
        float2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
        ScreenDoor(_ScreenDoorTransparency, normalizedScreenSpaceUV, _ScreenDoorTiling);
    #endif
#endif

    return input.positionCS.z;
}