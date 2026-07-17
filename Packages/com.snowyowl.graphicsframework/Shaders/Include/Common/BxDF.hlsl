#ifndef SWYO_BXDF_INCLUDED
#define SWYO_BXDF_INCLUDED

#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/BxDFBase.hlsl"

#if defined(SWYO_BXDF_STANDARD)
SwyoLightingResult SwyoBxDFStandard(SwyoLightContext lightCtx, SwyoInputData inputData, SwyoSurfaceData surfaceData, SwyoBxDFData bxdfData)
{
    SwyoLightingResult result = (SwyoLightingResult)0;

    result.diffuse = SwyoBxDFStandardDiffuse(lightCtx);
    result.diffuseColor = result.diffuse * bxdfData.base.diffuse * lightCtx.light.color;
    
#if !defined(_SPECULARHIGHLIGHTS_OFF)
    result.specular = SwyoBxDFStandardSpecular(lightCtx, bxdfData, surfaceData.specular) * result.diffuse;
    result.specularColor = result.specular * bxdfData.base.specular * lightCtx.light.color;
#endif
    
    return result;
}
#endif

#if defined(SWYO_BXDF_SIMPLE)
SwyoLightingResult SwyoBxDFSimple(SwyoLightContext lightCtx, SwyoInputData inputData, SwyoSurfaceData surfaceData, SwyoBxDFData bxdfData)
{
    SwyoLightingResult result = (SwyoLightingResult)0;
    return result;
}
#endif

#if defined(SWYO_BXDF_STYLIZED)
SwyoLightingResult SwyoBxDFStylized(SwyoLightContext lightCtx, SwyoInputData inputData, SwyoSurfaceData surfaceData, SwyoAdditionalData additionalData, SwyoBxDFData bxdfData)
{
    SwyoLightingResult result = (SwyoLightingResult)0;

    result.diffuse = SwyoBxDFStylizedDiffuse(lightCtx, additionalData.diffuseStep, additionalData.diffuseStepOffset);
    result.diffuseColor = SwyoBxDFStylizedDiffuseColor(lightCtx, bxdfData, result.diffuse, additionalData.shadowScale, additionalData.shadowColor);

#if !defined(_SPECULARHIGHLIGHTS_OFF)
    result.specular = SwyoBxDFStylizedSpecular(lightCtx, bxdfData, surfaceData.specular) * result.diffuse;
    result.specularColor = SwyoBxDFStylizedSpecularColor(lightCtx, bxdfData, result.specular, additionalData.specularColor);
#endif    
    return result;
}
#endif

#if defined(SWYO_BXDF_COMPLEX_CLEARCOAT)
SwyoLightingResult SwyoBxDFComplexClearCoat(SwyoLightContext lightCtx, SwyoInputData inputData, SwyoSurfaceData surfaceData, SwyoAdditionalData additionalData, SwyoBxDFData bxdfData)
{
    SwyoLightingResult result = (SwyoLightingResult)0;
    return result;
}
#endif

#if defined(SWYO_BXDF_COMPLEX_SUBSURFACE)
SwyoLightingResult SwyoBxDFComplexSubsurface(SwyoLightContext lightCtx, SwyoInputData inputData, SwyoSurfaceData surfaceData, SwyoAdditionalData additionalData, SwyoBxDFData bxdfData)
{
    SwyoLightingResult result = (SwyoLightingResult)0;
    return result;
}
#endif


SwyoLightingResult SwyoIntegrateBxDF(SwyoLightContext lightCtx, SwyoInputData inputData, SwyoSurfaceData surfaceData, SwyoAdditionalData additionalData, SwyoBxDFData bxdfData)
{
#if defined(SWYO_BXDF_STANDARD)
    return SwyoBxDFStandard(lightCtx, inputData, surfaceData, bxdfData);
#elif defined(SWYO_BXDF_SIMPLE)
    return SwyoBxDFSimple(lightCtx, inputData, surfaceData, additionalData, bxdfData);
#elif defined(SWYO_BXDF_STYLIZED)
    return SwyoBxDFStylized(lightCtx, inputData, surfaceData, additionalData, bxdfData);
#elif defined(SWYO_BXDF_COMPLEX_CLEARCOAT)
    return SwyoBxDFComplexClearCoat(lightCtx, inputData, surfaceData, additionalData, bxdfData);
#elif defined(SWYO_BXDF_COMPLEX_SUBSURFACE)
    return SwyoBxDFComplexSubsurface(lightCtx, inputData, surfaceData, additionalData, bxdfData);
#endif
}

#endif // SWYO_COMMON_LIGHTING_DATA_INCLUDED