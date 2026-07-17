#ifndef SWYO_DEFERRED_INCLUDED
#define SWYO_DEFERRED_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

#define k_SwyoMaterialFlagReceiveShadowsOff        (1 << 0) // Does not receive dynamic shadows
#define k_SwyoMaterialFlagSpecularHighlightsOff    (1 << 1) // Does not receivce specular

struct SwyoGBufferOutput
{
    half4 GBuffer0 : SV_Target0;
    half4 GBuffer1 : SV_Target1;
    half4 GBuffer2 : SV_Target2;
    half4 GBuffer3 : SV_Target3; // Camera color attachment

#ifdef _RENDER_PASS_ENABLED
    float GBuffer4 : SV_Target4;
#endif
};

FragmentOutput SwyoSurfaceDataToGbuffer(SwyoSurfaceData surfaceData, SwyoInputData inputData, half3 giAndEmission)
{
    FragmentOutput output = (FragmentOutput)0;

    uint materialFlags = 0;

#if defined(_RECEIVE_SHADOWS_OFF)
    materialFlags |= kMaterialFlagReceiveShadowsOff;
#endif
    
#if defined(_SPECULARHIGHLIGHTS_OFF)
    materialFlags |= kMaterialFlagSpecularHighlightsOff;
#endif
    
    output.GBuffer0 = half4(surfaceData.albedo.rgb, PackMaterialFlags(materialFlags));
    output.GBuffer1 = half4(surfaceData.metallic, 0, 0, surfaceData.occlusion);
    output.GBuffer2 = half4(inputData.normalWS, UNormToSNorm(surfaceData.smoothness));
    output.GBuffer3 = half4(giAndEmission, 1);
    
#if _RENDER_PASS_ENABLED
    output.GBuffer4 = inputData.positionCS.z;
#endif

    #if OUTPUT_SHADOWMASK
    output.GBUFFER_SHADOWMASK = inputData.shadowMask; // will have unity_ProbesOcclusion value if subtractive lighting is used (baked)
    #endif
    #ifdef _WRITE_RENDERING_LAYERS
    uint renderingLayers = GetMeshRenderingLayer();
    output.GBUFFER_LIGHT_LAYERS = float4(EncodeMeshRenderingLayer(renderingLayers), 0.0, 0.0, 0.0);
    #endif
    
    return output;
}


// ---------------------------------
// Deferred Pass

struct Attributes
{
    float4 positionOS : POSITION;
    uint vertexID : SV_VertexID;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float3 screenUV : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings Vertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

    float3 positionOS = input.positionOS.xyz;
    
    output.positionCS = float4(positionOS.xy, UNITY_RAW_FAR_CLIP_VALUE, 1.0); // Force triangle to be on zfar

    output.screenUV = output.positionCS.xyw;
#if UNITY_UV_STARTS_AT_TOP
    output.screenUV.xy = output.screenUV.xy * float2(0.5, -0.5) + 0.5 * output.screenUV.z;
#else
    output.screenUV.xy = output.screenUV.xy * 0.5 + 0.5 * output.screenUV.z;
#endif

    return output;
}


#endif