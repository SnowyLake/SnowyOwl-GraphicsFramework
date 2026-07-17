#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"
#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/BxDFBase.hlsl"

half _UseCustomSH;
real4 _CustomSHAr;
real4 _CustomSHAg;
real4 _CustomSHAb;
real4 _CustomSHBr;
real4 _CustomSHBg;
real4 _CustomSHBb;
real4 _CustomSHC;

#define SWYO_GET_SH(SHx) lerp(unity_##SHx, _Custom##SHx, _UseCustomSH);

// Samples SH L0, L1 and L2 terms
half3 SwyoSampleSH(half3 normalWS)
{
    // LPPV is not supported in Ligthweight Pipeline
    real4 SHCoefficients[7];
    SHCoefficients[0] = SWYO_GET_SH(SHAr);
    SHCoefficients[1] = SWYO_GET_SH(SHAg);
    SHCoefficients[2] = SWYO_GET_SH(SHAb);
    SHCoefficients[3] = SWYO_GET_SH(SHBr);
    SHCoefficients[4] = SWYO_GET_SH(SHBg);
    SHCoefficients[5] = SWYO_GET_SH(SHBb);
    SHCoefficients[6] = SWYO_GET_SH(SHC);

    return max(half3(0, 0, 0), SampleSH9(SHCoefficients, normalWS));
}

// SH Vertex Evaluation. Depending on target SH sampling might be
// done completely per vertex or mixed with L2 term per vertex and L0, L1
// per pixel. See SampleSHPixel
half3 SwyoSampleSHVertex(half3 normalWS)
{
#if defined(EVALUATE_SH_VERTEX)
    return CustomSampleSH(normalWS);
#elif defined(EVALUATE_SH_MIXED)
    // no max since this is only L2 contribution
    real4 shBr = SWYO_GET_SH(SHBr);
    real4 shBg = SWYO_GET_SH(SHBg);
    real4 shBb = SWYO_GET_SH(SHBb);
    real4 shC = SWYO_GET_SH(SHC);
    return SHEvalLinearL2(normalWS, shBr, shBg, shBb, shC);
#endif

    // Fully per-pixel. Nothing to compute.
    return half3(0.0, 0.0, 0.0);
}

// SH Pixel Evaluation. Depending on target SH sampling might be done
// mixed or fully in pixel. See SampleSHVertex
half3 SwyoSampleSHPixel(half3 L2Term, half3 normalWS)
{
#if defined(EVALUATE_SH_VERTEX)
    return L2Term;
#elif defined(EVALUATE_SH_MIXED)
    real4 shAr = SWYO_GET_SH(SHAr);
    real4 shAg = SWYO_GET_SH(SHAg);
    real4 shAb = SWYO_GET_SH(SHAb);
    half3 res = L2Term + SHEvalLinearL0L1(normalWS, shAr, shAg, shAb);
    #ifdef UNITY_COLORSPACE_GAMMA
        res = LinearToSRGB(res);
    #endif
    return max(half3(0, 0, 0), res);
#endif

    // Default: Evaluate SH fully per-pixel
    return SwyoSampleSH(normalWS);
}

half3 SHEvalLinearL0(half4 shAr, half4 shAg, half4 shAb)
{
    return half3(shAr.a, shAg.a, shAb.a);
}

half3 SwyoSampleSH1()
{
    real4 shAr = SWYO_GET_SH(SHAr);
    real4 shAg = SWYO_GET_SH(SHAg);
    real4 shAb = SWYO_GET_SH(SHAb);
    return SHEvalLinearL0(shAr, shAg, shAb);
}

#if defined(LIGHTMAP_ON)
    #define SWYO_SAMPLE_GI(staticLmName, shName, normalWSName) SampleLightmap(staticLmName, 0, normalWSName)
#else
    #define SWYO_SAMPLE_GI(staticLmName, shName, normalWSName) SwyoSampleSHPixel(shName, normalWSName)
#endif


half3 SwyoGlobalIllumination(SwyoBxDFData bxdfData, half3 diffiseGI, half occlusion, half fresnel, float3 positionWS, half3 normalWS, half3 viewDirectionWS,
                             float2 normalizedScreenSpaceUV, float2 matcapUV, half indirectDiffuseIntensity, half indirectSpecularIntensity)
{
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half3 indirectDiffuse = diffiseGI * indirectDiffuseIntensity;
    
#if defined(_REFLECTION_CUBEMAP)
    half mip = PerceptualRoughnessToMipmapLevel(bxdfData.base.perceptualRoughness);
    half4 encodedIrradiance = half4(SAMPLE_TEXTURECUBE_LOD(_ReflectionCubeMap, sampler_ReflectionCubeMap, reflectVector, mip));
    half3 indirectSpecular = DecodeHDREnvironment(encodedIrradiance, _ReflectionCubeMap_HDR) * indirectSpecularIntensity;
#elif defined(_REFLECTION_MATCAP)
    half3 indirectSpecular = SAMPLE_TEXTURE2D(_ReflectionMatcapMap, sampler_ReflectionMatcapMap, matcapUV).rgb * indirectSpecularIntensity;
#else
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, positionWS, bxdfData.base.perceptualRoughness, indirectSpecularIntensity, normalizedScreenSpaceUV);
#endif
    
#if !defined(_REFLECTION_MATCAP)
    half3 color = SwyoEnvironmentBxDF(bxdfData, indirectDiffuse, indirectSpecular, fresnel);
#else
    half3 color = indirectDiffuse * bxdfData.base.diffuse + indirectSpecular * bxdfData.base.reflectivity;
#endif

    if (IsOnlyAOLightingFeatureEnabled())
    {
        color = half3(1, 1, 1); // "Base white" for AO debug lighting mode
    }
    
    return color * occlusion;
}