#pragma once

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/BRDF.hlsl"
#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/CommonUtils.hlsl"

inline void SwyoInitializeBxDFDataDirect(half3 albedo, half3 diffuse, half3 specular, half reflectivity, half smoothness, half alpha, out SwyoBxDFData outBxDFData)
{
    outBxDFData = (SwyoBxDFData)0;
    outBxDFData.base.albedo = albedo;
    outBxDFData.base.diffuse = diffuse;
    outBxDFData.base.specular = specular;
    outBxDFData.base.reflectivity = reflectivity;

    outBxDFData.base.perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(smoothness);
    outBxDFData.base.roughness           = max(PerceptualRoughnessToRoughness(outBxDFData.base.perceptualRoughness), HALF_MIN_SQRT);
    outBxDFData.base.roughness2          = max(outBxDFData.base.roughness * outBxDFData.base.roughness, HALF_MIN);
    outBxDFData.base.grazingTerm         = saturate(smoothness + reflectivity);
    outBxDFData.base.normalizationTerm   = outBxDFData.base.roughness * half(4.0) + half(2.0);
    outBxDFData.base.roughness2MinusOne  = outBxDFData.base.roughness2 - half(1.0);

    // Input is expected to be non-alpha-premultiplied while ROP is set to pre-multiplied blend.
    // We use input color for specular, but (pre-)multiply the diffuse with alpha to complete the standard alpha blend equation.
    // In shader: Cs' = Cs * As, in ROP: Cs' + Cd(1-As);
    // i.e. we only alpha blend the diffuse part to background (transmittance).
#if defined(_ALPHAPREMULTIPLY_ON)
    // TODO: would be clearer to multiply this once to accumulated diffuse lighting at end instead of the surface property.
    outBxDFData.diffuse *= alpha;
#endif
}

// Initialize BxDFData
inline void SwyoInitializeBxDFData(half3 albedo, half alpha, half metallic, half smoothness, out SwyoBxDFData outBxDFData)
{
    half oneMinusReflectivity = OneMinusReflectivityMetallic(metallic);
    half reflectivity = half(1.0) - oneMinusReflectivity;
    half3 bxdfDiffuse = albedo * oneMinusReflectivity;
    half3 bxdfSpecular = lerp(kDieletricSpec.rgb, albedo, metallic);

    SwyoInitializeBxDFDataDirect(albedo, bxdfDiffuse, bxdfSpecular, reflectivity, smoothness, alpha, outBxDFData);
}

half3 SwyoEnvironmentBxDFSpecular(SwyoBxDFData bxdfData, half fresnelTerm)
{
    float surfaceReduction = 1.0 / (bxdfData.base.roughness2 + 1.0);
    return half3(surfaceReduction * lerp(bxdfData.base.specular, bxdfData.base.grazingTerm, fresnelTerm));
}

half3 SwyoEnvironmentBxDF(SwyoBxDFData bxdfData, half3 indirectDiffuse, half3 indirectSpecular, half fresnelTerm)
{
    half3 c = indirectDiffuse * bxdfData.base.diffuse;
    c += indirectSpecular * SwyoEnvironmentBxDFSpecular(bxdfData, fresnelTerm);
    return c;
}


// --------------------------------
// BxDF Standard
half SwyoBxDFStandardDiffuse(SwyoLightContext lightCtx)
{
    half attenuation = lightCtx.light.shadowAttenuation * lightCtx.light.distanceAttenuation;
    half diffuse = lightCtx.clampedNL * attenuation;
    return diffuse;
}
// Computes the scalar specular term for Minimalist CookTorrance BRDF
// NOTE: needs to be multiplied with reflectance f0, i.e. specular color to complete
half SwyoBxDFStandardSpecular(SwyoLightContext lightCtx, SwyoBxDFData bxdfData, float specularScale = 1.0)
{
    half LoH = half(lightCtx.clampedLH);

    // GGX Distribution multiplied by combined approximation of Visibility and Fresnel
    // BRDFspec = (D * V * F) / 4.0
    // D = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2
    // V * F = 1.0 / ( LoH^2 * (roughness + 0.5) )
    // See "Optimizing PBR for Mobile" from Siggraph 2015 moving mobile graphics course
    // https://community.arm.com/events/1155

    // Final BRDFspec = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2 * (LoH^2 * (roughness + 0.5) * 4.0)
    // We further optimize a few light invariant terms
    // bxdfData.normalizationTerm = (roughness + 0.5) * 4.0 rewritten as roughness * 4.0 + 2.0 to a fit a MAD.
    float d = lightCtx.clampedNH * lightCtx.clampedNH * bxdfData.base.roughness2MinusOne + 1.00001f;

    half LoH2 = LoH * LoH;
    half specular = bxdfData.base.roughness2 / ((d * d) * max(0.1h, LoH2) * bxdfData.base.normalizationTerm);
    specular *= specularScale;

    // On platforms where half actually means something, the denominator has a risk of overflow
    // clamp below was added specifically to "fix" that, but dx compiler (we convert bytecode to metal/gles)
    // sees that specularTerm have only non-negative terms, so it skips max(0,..) in clamp (leaving only min(100,...))
#if REAL_IS_HALF
    specular = specular - HALF_MIN;
    // Update: Conservative bump from 100.0 to 1000.0 to better match the full float specular look.
    // Roughly 65504.0 / 32*2 == 1023.5,
    // or HALF_MAX / ((mobile) MAX_VISIBLE_LIGHTS * 2),
    // to reserve half of the per light range for specular and half for diffuse + indirect + emissive.
    specular = clamp(specular, 0.0, 1000.0); // Prevent FP16 overflow on mobiles
#endif

    return specular;
}



// --------------------------------
// BxDF Stylized
half SwyoBxDFStylizedDiffuse(SwyoLightContext lightCtx, half diffuseStep, half diffuseStepOffset)
{
    half diffuse = 0.0;
#if defined(_STYLIZED_SDF_FACE_ON)
    diffuse = diffuseStepOffset;
#else
    half attenuation = lightCtx.light.shadowAttenuation * lightCtx.light.distanceAttenuation;
    diffuse = smoothstep(0, diffuseStep, saturate(lightCtx.NL + diffuseStepOffset)) * attenuation;
#endif
    return diffuse;
}
half3 SwyoBxDFStylizedDiffuseColor(SwyoLightContext lightCtx, SwyoBxDFData bxdfData, half diffuse, half shadowScale, half3 shadowColor)
{
    half3 diffuseCol = bxdfData.base.diffuse;
    if (lightCtx.isMainLight)
    {
    #if defined(_SHADOW_RAMP_ON)
        diffuseCol = lerp(diffuseCol, diffuseCol * shadowColor.rgb, shadowScale);
    #else
        half3 shadowCol = lerp(diffuseCol, diffuseCol * shadowColor.rgb, shadowScale);
        diffuseCol = lerp(shadowCol, diffuseCol, diffuse);
    #endif
    }
    else
    {
        diffuseCol *= diffuse;
    }
    
    return diffuseCol * lightCtx.light.color;
}
half SwyoBxDFStylizedSpecular(SwyoLightContext lightCtx, SwyoBxDFData bxdfData, half mainSpecularScale)
{
    half mainSpecular = SwyoBxDFStandardSpecular(lightCtx, bxdfData, mainSpecularScale);
    half viewSpwcular = 0;
    half specular = max(mainSpecular, viewSpwcular);

    return specular;
}
half3 SwyoBxDFStylizedSpecularColor(SwyoLightContext lightCtx, SwyoBxDFData bxdfData, half specular, half3 specularColor)
{
    half3 specularCol = bxdfData.base.specular * specular * specularColor;
    return specularCol * lightCtx.light.color;
}