#pragma once

// -------------------------------------
// Includes
#include_with_pragmas "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/PassIncludes.hlsl"


// -------------------------------------
// Input
struct OpaqueOutlineAttributes
{
    float4 positionOS   : POSITION;
    float4 tangentOS    : TANGENT;
    float3 normalOS     : NORMAL;
    float2 texcoord0    : TEXCOORD0;
    half4 color         : COLOR;
};

struct OpaqueOutlineVaryings
{
    float4 positionCS   : SV_POSITION;
    float2 uv0          : TEXCOORD0;
};

half _OpaqueOutlineDistanceFadeFactor;


// -------------------------------------
// Entry
OpaqueOutlineVaryings OpaqueOutlineVertex(OpaqueOutlineAttributes input)
{
    OpaqueOutlineVaryings output = (OpaqueOutlineVaryings)0;

    float3 positionVS = NormalExpandOutlineInVS(input.positionOS, input.normalOS, input.tangentOS, _OutlineWidth, _OpaqueOutlineDistanceFadeFactor, input.color);
    
    output.positionCS = TransformWViewToHClip(positionVS);
#if defined(_ALPHATEST_ON)
    output.uv0 = TRANSFORM_TEX(input.texcoord0, _BaseMap);
#endif
    
    return output;
}

half4 OpaqueOutlineFragment(OpaqueOutlineVaryings input) : SV_TARGET
{
    half4 finalColor = COLOR4_BLACK_ALPHA1;
    half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv0) * _BaseColor;

#if !defined(SWYO_DEPTH_PRIMING_ON) || !defined(_OPAQUE_OUTLINE_COLOR_PASS)
    SwyoAlpha(baseColor.a, _Cutoff);
#endif
    
    finalColor.rgb = lerp(_OutlineColor.rgb, _OutlineColor.rgb * baseColor.rgb, _OutlineColor.a);
    
    return finalColor;
}