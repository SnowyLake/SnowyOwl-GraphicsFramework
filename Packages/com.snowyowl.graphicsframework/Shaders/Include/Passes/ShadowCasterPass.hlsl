#pragma once

// -------------------------------------
// Includes
#include_with_pragmas "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/PassIncludes.hlsl"


// -------------------------------------
// Input
struct ShadowCasterAttributes
{
    float4 positionOS : POSITION;
    float3 normalOS   : NORMAL;
    float2 texcoord0  : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct ShadowCasterVaryings
{
    float4 positionCS : SV_POSITION;
    float2 uv0        : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

float3 _LightDirection;
float3 _LightPosition;


// -------------------------------------
// Functions
float4 SwyoGetShadowPositionHClip(ShadowCasterAttributes input)
{
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

#if _CASTING_PUNCTUAL_LIGHT_SHADOW
    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
#else
    float3 lightDirectionWS = _LightDirection;
#endif

    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

#if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#else
    positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#endif

    return positionCS;
}


// -------------------------------------
// Entry
ShadowCasterVaryings ShadowCasterVertex(ShadowCasterAttributes input)
{
    ShadowCasterVaryings output = (ShadowCasterVaryings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    
#if defined(_ALPHATEST_ON)
    output.uv0 = TRANSFORM_TEX(input.texcoord0, _BaseMap);
#endif
    output.positionCS = SwyoGetShadowPositionHClip(input);

    return output;
}

half4 ShadowCasterFragment(ShadowCasterVaryings input) : SV_TARGET
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
    
    return 0;
}