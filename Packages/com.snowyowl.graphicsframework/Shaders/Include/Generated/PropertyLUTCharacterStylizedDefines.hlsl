#ifndef PROPERTYLUTCHARACTERSTYLIZEDDEFINES_INCLUDED
#define PROPERTYLUTCHARACTERSTYLIZEDDEFINES_INCLUDED

#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/PropertyLUTCommonDefines.hlsl"
#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Generated/PropertyLUTFallbackDefines.hlsl"

#define MAX_BODY_PART_COUNT 10

#if defined(_PROPERTY_LUT_ON)

static float4 swyo_PropertyLUT_PixelData_0;
#undef LoadPropertyLUT_Metallic
float LoadPropertyLUT_Metallic() { return swyo_PropertyLUT_PixelData_0.a; }

static float4 swyo_PropertyLUT_PixelData_1;
#undef LoadPropertyLUT_SpecularColor
float3 LoadPropertyLUT_SpecularColor() { return swyo_PropertyLUT_PixelData_1.rgb; }

static float4 swyo_PropertyLUT_PixelData_2;
#undef LoadPropertyLUT_MatcapMask
float LoadPropertyLUT_MatcapMask() { return swyo_PropertyLUT_PixelData_2.r; }
#undef LoadPropertyLUT_LocalCubeMapMask
float LoadPropertyLUT_LocalCubeMapMask() { return swyo_PropertyLUT_PixelData_2.g; }


void SetupPropertyLUT(uint propertyId)
{
	swyo_PropertyLUT_PixelData_0 = LoadPropertyLUT(0, propertyId);
	swyo_PropertyLUT_PixelData_1 = LoadPropertyLUT(1, propertyId);
#if defined(_MATCAP_ON) || defined(_LOCAL_CUBEMAP_ON)
	swyo_PropertyLUT_PixelData_2 = LoadPropertyLUT(2, propertyId);
#endif
}
#undef SWYO_SETUP_PROPERTY_LUT
#define SWYO_SETUP_PROPERTY_LUT(__propertyId) SetupPropertyLUT(__propertyId)

#endif

#endif
