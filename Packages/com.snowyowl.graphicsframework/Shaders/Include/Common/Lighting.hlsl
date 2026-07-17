#ifndef SWYO_LIGHTING_INCLUDED
#define SWYO_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/BxDF.hlsl"
#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/GlobalIllumination.hlsl"

half4 CalculateShadowMask(half4 inputDataShadowMask)
{
    // To ensure backward compatibility we have to avoid using shadowMask input, as it is not present in older shaders
#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
    half4 shadowMask = inputDataShadowMask;
#elif !defined (LIGHTMAP_ON)
    half4 shadowMask = unity_ProbesOcclusion;
#else
    half4 shadowMask = half4(1, 1, 1, 1);
#endif

    return shadowMask;
}

// TODO: SSS
SwyoLightContext CreateLightContext(Light light, SwyoInputData inputData, half isMainLight = 0)
{
    SwyoLightContext ctx = (SwyoLightContext)0;

    ctx.light = light;
    ctx.isMainLight = isMainLight;
    
    float3 normalWSFloat3 = float3(inputData.normalWS);
    float3 lightDirectionWSFloat3 = float3(light.direction);
    float3 viewDirectionWSFloat3 = float3(inputData.viewDirectionWS);
    float3 halfDirFloat3 = SafeNormalize(lightDirectionWSFloat3 + viewDirectionWSFloat3);
    
    ctx.NL = dot(normalWSFloat3, lightDirectionWSFloat3);
    ctx.NV = dot(normalWSFloat3, viewDirectionWSFloat3);
    ctx.clampedNL = saturate(ctx.NL);
    ctx.clampedNV = saturate(ctx.NV);
    ctx.clampedNH = saturate(dot(normalWSFloat3, halfDirFloat3));
    ctx.clampedLH = saturate(dot(lightDirectionWSFloat3, halfDirFloat3));

    return ctx;
}

SwyoLightContext GetMainLightContext(SwyoInputData inputData)
{
    SwyoLightContext ctx = (SwyoLightContext)0;

    half4 shadowMask = CalculateShadowMask(inputData.shadowMask);
    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, shadowMask);
    ctx = CreateLightContext(mainLight, inputData, 1);

    return ctx;
}

half3 AccumulateLighting(SwyoLightingAccumulator lighting)
{
    half3 lightingAccumulation = 0;
    if (IsOnlyAOLightingFeatureEnabled())
    {
        return lighting.indirect; // Contains white + AO
    }

    if (IsLightingFeatureEnabled(DEBUGLIGHTINGFEATUREFLAGS_GLOBAL_ILLUMINATION))
    {
        lightingAccumulation += lighting.indirect;
    }

    if (IsLightingFeatureEnabled(DEBUGLIGHTINGFEATUREFLAGS_MAIN_LIGHT))
    {
        lightingAccumulation += lighting.directMainLight;
    }

    if (IsLightingFeatureEnabled(DEBUGLIGHTINGFEATUREFLAGS_ADDITIONAL_LIGHTS))
    {
        lightingAccumulation += lighting.directAdditionalLight;
    }

    if (IsLightingFeatureEnabled(DEBUGLIGHTINGFEATUREFLAGS_VERTEX_LIGHTING))
    {
        lightingAccumulation += 0;  // SnowyOwl RP doesn't support vertex lighting.
    }

    if (IsLightingFeatureEnabled(DEBUGLIGHTINGFEATUREFLAGS_EMISSION))
    {
        lightingAccumulation += lighting.emission;
    }

    return lightingAccumulation;
}

half3 GetLightingResultColor(SwyoLightingResult result)
{
    half3 color = result.diffuseColor + result.specularColor;
    return color;
}

SwyoLightingAccumulator SwyoLighting(SwyoInputData inputData, SwyoSurfaceData surfaceData, SwyoAdditionalData additionalData, SwyoBxDFData bxdfData, SwyoLightContext mainLightCtx,
                                     out SwyoLightingResult outMainLightingResult)
{
    SwyoLightingAccumulator lightAccumulator = (SwyoLightingAccumulator)0;
    outMainLightingResult = (SwyoLightingResult)0;
    
    uint meshRenderingLayers = GetMeshRenderingLayer();
    
    lightAccumulator.emission = surfaceData.emission;
    lightAccumulator.indirect = SwyoGlobalIllumination(bxdfData, inputData.bakedGI, surfaceData.occlusion, inputData.fresnel, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS,
                                                       inputData.normalizedScreenSpaceUV, inputData.mainUV.zw, surfaceData.indirectDiffuseIntensity, surfaceData.indirectSpecularIntensity);

#if defined(_LIGHT_LAYERS)
    if (IsMatchingLightLayer(mainLightCtx.light.layerMask, meshRenderingLayers))
#endif
    {
        outMainLightingResult = SwyoIntegrateBxDF(mainLightCtx, inputData, surfaceData, additionalData, bxdfData);
        lightAccumulator.directMainLight += GetLightingResultColor(outMainLightingResult);
    }
    // Modify indirect

#if defined(_ADDITIONAL_LIGHTS)
    uint pixelLightCount = GetAdditionalLightsCount();

    #if USE_FORWARD_PLUS
        for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
        {
            FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK

            Light light = GetAdditionalLight(lightIndex, inputData.positionWS);

        #if defined(_LIGHT_LAYERS)
            if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        #endif
            {
                SwyoLightContext lightCtx = CreateLightContext(light, inputData, 0);
                SwyoLightingResult lightingResult = SwyoIntegrateBxDF(lightCtx, inputData, surfaceData, additionalData, bxdfData);
                lightAccumulator.directAdditionalLight += GetLightingResultColor(lightingResult);
            }
        }
    #endif
    
    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, inputData.positionWS);

    #if defined(_LIGHT_LAYERS)
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    #endif
        {
            SwyoLightContext lightCtx = CreateLightContext(light, inputData, 0);
            SwyoLightingResult lightingResult = SwyoIntegrateBxDF(lightCtx, inputData, surfaceData, additionalData, bxdfData);
            lightAccumulator.directAdditionalLight += GetLightingResultColor(lightingResult);
        }
    LIGHT_LOOP_END
#endif
    
    lightAccumulator.directMainLight *= surfaceData.directIntensity;
    lightAccumulator.directAdditionalLight *= surfaceData.directIntensity;

    return lightAccumulator;
}

#endif // SWYO_COMMON_LIGHTING_INCLUDED