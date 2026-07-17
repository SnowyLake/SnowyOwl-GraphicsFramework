#ifndef PROPERTY_LUT_COMMON_DEFINES_INCLUDED
#define PROPERTY_LUT_COMMON_DEFINES_INCLUDED

// -------------------------------------
// Include
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "../Generated/GlobalShaderDefines.hlsl"

// -------------------------------------
// Global Variables
#if defined(_PROPERTY_LUT_ON)

TEXTURE2D(_PropertyLUT);

#if defined(_PROPERTY_LUT_EDITING)
    StructuredBuffer<float4> _PropertyLUTEditingData;
    uint _PropertyLUTFunctionCount;
    half _PropertyLUTEditingEnable;
#endif

float4 LoadPropertyLUT(uint functionIndex, uint propertyId)
{
#if defined(_PROPERTY_LUT_EDITING)
    if (_PropertyLUTEditingEnable > 0)
    {
        return _PropertyLUTEditingData[propertyId * _PropertyLUTFunctionCount + functionIndex];
    }
    else
#endif
    {
        return LOAD_TEXTURE2D(_PropertyLUT, uint2(functionIndex, propertyId));
    }
}

#endif

#endif // PROPERTY_LUT_INCLUDE