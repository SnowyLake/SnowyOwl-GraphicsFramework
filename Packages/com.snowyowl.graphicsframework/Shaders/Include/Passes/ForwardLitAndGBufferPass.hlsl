#pragma once

// -------------------------------------
// Includes
#include_with_pragmas "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/PassIncludes.hlsl"


// -------------------------------------
// Macros
#if defined(SWYO_DEPTH_PRIMING_ON) && !defined(_SURFACE_TYPE_TRANSPARENT)
    #define ALPHATEST_OFF
#endif


// -------------------------------------
// Input
struct ForwardLitAttributes
{
    float4 positionOS   : POSITION;
    float4 tangentOS    : TANGENT;
    float3 normalOS     : NORMAL;
    float2 texcoord0    : TEXCOORD0;
    float2 texcoord1    : TEXCOORD1;
    float2 texcoord2    : TEXCOORD2;
    half4 color         : COLOR;
};

struct ForwardLitVaryings
{
    float4 positionCS   : SV_POSITION;
    float4 uv0          : TEXCOORD0;
    float4 uv1          : TEXCOORD1;
    float3 positionWS   : TEXCOORD2;
    float3 normalWS     : TEXCOORD3;
#if defined(_NORMALMAP_ON)
    half4 tangentWS     : TEXCOORD4;
#endif
    half4 color         : TEXCOORD5;
    half3 vertexSH      : TEXCOORD6;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};


// -------------------------------------
// Functions
void SwyoInitializeInputData(ForwardLitVaryings input, half3 normalTS, out SwyoInputData outInputData)
{
    outInputData = (SwyoInputData)0;
    
    outInputData.mainUV = input.uv0;

    outInputData.positionCS = input.positionCS;
    outInputData.positionWS = input.positionWS;

#if defined(_NORMALMAP_ON)
    float sign = input.tangentWS.w;      // should be either +1 or -1
    float3 bitangent = sign * cross(input.normalWS.xyz, input.tangentWS.xyz);
    outInputData.tangentToWorld = half3x3(input.tangentWS.xyz, bitangent, input.normalWS);
    outInputData.normalWS = TransformTangentToWorld(normalTS, outInputData.tangentToWorld);
#else
    outInputData.normalWS = input.normalWS;
#endif
    outInputData.normalWS = NormalizeNormalPerPixel(outInputData.normalWS);
    outInputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);

#if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    outInputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
#else
    outInputData.shadowCoord = float4(0, 0, 0, 0);
#endif
    outInputData.shadowMask = 0;
    outInputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(outInputData.positionCS);
    outInputData.bakedGI = SWYO_SAMPLE_GI(input.uv1, input.vertexSH, outInputData.normalWS);
    outInputData.fresnel = Pow4(1.0 - saturate(dot(outInputData.normalWS, outInputData.viewDirectionWS)));
#if defined(DEBUG_DISPLAY)
    #if !defined(LIGHTMAP_ON)
        outInputData.vertexSH = input.vertexSH;
    #endif
#endif
}


// -------------------------------------
// ForwardLit Entry
ForwardLitVaryings ForwardLitVertex(ForwardLitAttributes input)
{
    ForwardLitVaryings output = (ForwardLitVaryings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    
    output.uv0.xy = TRANSFORM_TEX(input.texcoord0, _BaseMap);
    
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    output.positionCS = vertexInput.positionCS;
    output.positionWS = vertexInput.positionWS;

    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    output.normalWS = normalInput.normalWS;
#if defined(_NORMALMAP_ON)
    half sign = input.tangentOS.w * GetOddNegativeScale();
    output.tangentWS = half4(normalInput.tangentWS.xyz, sign);
#endif
    
    output.color = input.color;

#if defined(LIGHTMAP_ON)
    output.uv1.xy = input.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
#else
    output.vertexSH = SwyoSampleSHVertex(output.normalWS);
#endif

#if defined(_REFLECTION_MATCAP) || defined(_MATCAP_ON)
    half3 viewDirWS = GetCameraPositionWS() - output.positionWS;
    output.uv0.zw = GetMatcapUV(output.normalWS, viewDirWS, input.normalOS);
#endif

    return output;
}

half4 ForwardLitFragment(ForwardLitVaryings input) : SV_Target0
{
    half4 outputColor = COLOR4_BLACK_ALPHA0;
    
    SwyoInputData inputData;
    SwyoTextureData textureData;
    SwyoSurfaceData surfaceData;
    SwyoAdditionalData additionalData;
    SwyoBxDFData bxdfData;

    SwyoInitializeTextureData(input.uv0.xy, textureData);
    SwyoInitializeSurfaceData(textureData, surfaceData);
    
    SwyoInitializeInputData(input, surfaceData.normalTS, inputData);
   
#if defined(_DYNAMIC_FEATURE_ON)
    #if defined(_SCREEN_DOOR_ON) && !defined(ALPHATEST_OFF)
        ScreenDoor(_ScreenDoorTransparency, inputData.normalizedScreenSpaceUV, _ScreenDoorTiling);
    #endif
#endif
    
#if defined(_SURFACE_TYPE_TRANSPARENT)
    UNITY_BRANCH
    if (surfaceData.alpha <= ZERO)
    {
        return outputColor;
    }
#endif

    SwyoLightContext mainLightCtx = GetMainLightContext(inputData);
    
    SwyoInitializeAdditionalData(input.uv0, inputData, textureData, surfaceData, mainLightCtx, additionalData);
    SwyoInitializeBxDFData(surfaceData.albedo, surfaceData.alpha, surfaceData.metallic, surfaceData.smoothness, bxdfData);
    
    SwyoLightingResult mainLightingResult = (SwyoLightingResult)0;
    SwyoLightingAccumulator lightAccumulator = SwyoLighting(inputData, surfaceData, additionalData, bxdfData, mainLightCtx, mainLightingResult);
    
    outputColor.rgb = AccumulateLighting(lightAccumulator);
    outputColor.a = 1.0f;

#if defined(_OVERLAY_CUBEMAP_ON)
    outputColor.rgb += SampleOverlayCubeMap(TEXTURECUBE_ARGS(_OverlayCubeMap, sampler_OverlayCubeMap), _OverlayCubeMap_HDR, _OverlayCubeMapTintColor.rgb, inputData.viewDirectionWS, inputData.normalWS,
                                          surfaceData.smoothness, _OverlayCubeMapScale, _OverlayCubeMapSmoothnessOffset, _OverlayCubeMapRotate);
#endif

#if defined(_MATCAP_ON)
    outputColor.rgb = ApplyMatcap(TEXTURE2D_ARGS(_MatcapMap, sampler_MatcapMap), input.uv0.zw, outputColor.rgb, surfaceData.albedo, _MatcapScale, _MatcapBlendFactor);
#endif

#if defined(_RIM_ON)
    outputColor.rgb += RimLighting(surfaceData.albedo, _RimColor, _RimScale, _RimRange, _RimPart, mainLightCtx.clampedNV, inputData.normalWS, inputData.positionWS, mainLightingResult.diffuse);
#endif

#if defined(_SURFACE_TYPE_TRANSPARENT)
    outputColor.a *= surfaceData.alpha;
#endif

#if defined(DEBUG_DISPLAY)
    half4 debugColor;
    InputData urpInputData = SwyoInputDataConvertToURP(inputData);
    SurfaceData urpSurfaceData = SwyoSurfaceDataConvertToURP(surfaceData);
    SETUP_DEBUG_TEXTURE_DATA(urpInputData, input.uv0, _BaseMap);
    if (CanDebugOverrideOutputColor(urpInputData, urpSurfaceData, bxdfData.base, debugColor))
    {
        return debugColor;
    }
#endif
    
    return outputColor;
}



// -------------------------------------
// Gbuffer Includes
#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/Deferred.hlsl"


// -------------------------------------
// Gbuffer Macros
#if defined(ALPHATEST_OFF)
    #undef ALPHATEST_OFF
#endif


// -------------------------------------
// Gbuffer Entry
ForwardLitVaryings GBufferVertex(ForwardLitAttributes input)
{
    return ForwardLitVertex(input);
}

FragmentOutput GBufferFragment(ForwardLitVaryings input)
{
    SwyoInputData inputData;
    SwyoTextureData textureData;
    SwyoSurfaceData surfaceData;
    SwyoAdditionalData additionalData;
    SwyoBxDFData bxdfData;

    SwyoInitializeTextureData(input.uv0.xy, textureData);
    SwyoInitializeSurfaceData(textureData, surfaceData);
    
    SwyoInitializeInputData(input, surfaceData.normalTS, inputData);

    SwyoLightContext mainLightCtx = GetMainLightContext(inputData);
    
    SwyoInitializeAdditionalData(input.uv0, inputData, textureData, surfaceData, mainLightCtx, additionalData);
    SwyoInitializeBxDFData(surfaceData.albedo, surfaceData.alpha, surfaceData.metallic, surfaceData.smoothness, bxdfData);
    
    half3 gi = SwyoGlobalIllumination(bxdfData, inputData.bakedGI, surfaceData.occlusion, inputData.fresnel, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS,
                                      inputData.normalizedScreenSpaceUV, inputData.mainUV.zw, surfaceData.indirectDiffuseIntensity, surfaceData.indirectSpecularIntensity);
    half3 giAndEmission = gi + surfaceData.emission;
    
    return SwyoSurfaceDataToGbuffer(surfaceData, inputData, giAndEmission);
}