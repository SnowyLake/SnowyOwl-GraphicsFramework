#ifndef SWYO_COMMON_DATAS_INCLUDED
#define SWYO_COMMON_DATAS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// ------------------------------------------------
// Macro Defines
// Colors
#define COLOR3_WHITE half3(1.0, 1.0, 1.0)
#define COLOR3_BLACK half3(0.0, 0.0, 0.0)
#define COLOR4_WHITE_ALPHA0 half4(1.0, 1.0, 1.0, 0.0)
#define COLOR4_WHITE_ALPHA1 half4(1.0, 1.0, 1.0, 1.0)
#define COLOR4_BLACK_ALPHA0 half4(0.0, 0.0, 0.0, 0.0)
#define COLOR4_BLACK_ALPHA1 half4(0.0, 0.0, 0.0, 1.0)
#define COLOR4_LINEAR_GRAY  half4(0.5, 0.5, 0.5, 0.5)
#define ZERO 0.00001

// Rim Part
#define RIM_PART_ALL                0
#define RIM_PART_LIGHT_ONLY         1
#define RIM_PART_DARK_ONLY          2


// -------------------------------------
// Static Const Define
// [Reference] https://en.wikipedia.org/wiki/Ordered_dithering
static const float DitherBayer2x2[4] =
{
    1.0 / 5.0, 3.0 / 5.0,
    4.0 / 5.0, 2.0 / 5.0
};

static const float DitherBayer4x4[16] =
{
    1.0  / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
    13.0 / 17.0, 5.0  / 17.0, 15.0 / 17.0,  7.0 / 17.0,
    4.0  / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
    16.0 / 17.0, 8.0  / 17.0, 14.0 / 17.0,  6.0 / 17.0
};

static const float DitherBayer8x8[64] =
{
    1.0  / 65.0, 33.0 / 65.0,  9.0 / 65.0, 41.0 / 65.0,  3.0 / 65.0, 35.0 / 65.0, 11.0 / 65.0, 43.0 / 65.0,
    49.0 / 65.0, 17.0 / 65.0, 57.0 / 65.0, 25.0 / 65.0, 51.0 / 65.0, 19.0 / 65.0, 59.0 / 65.0, 27.0 / 65.0,
    13.0 / 65.0, 45.0 / 65.0,  5.0 / 65.0, 37.0 / 65.0, 15.0 / 65.0, 47.0 / 65.0,  7.0 / 65.0, 39.0 / 65.0,
    61.0 / 65.0, 29.0 / 65.0, 53.0 / 65.0, 21.0 / 65.0, 63.0 / 65.0, 31.0 / 65.0, 55.0 / 65.0, 23.0 / 65.0,
    4.0  / 65.0, 36.0 / 65.0, 12.0 / 65.0, 44.0 / 65.0,  2.0 / 65.0, 34.0 / 65.0, 10.0 / 65.0, 42.0 / 65.0,
    52.0 / 65.0, 20.0 / 65.0, 60.0 / 65.0, 28.0 / 65.0, 50.0 / 65.0, 18.0 / 65.0, 58.0 / 65.0, 26.0 / 65.0,
    16.0 / 65.0, 48.0 / 65.0,  8.0 / 65.0, 40.0 / 65.0, 14.0 / 65.0, 46.0 / 65.0,  6.0 / 65.0, 38.0 / 65.0,
    64.0 / 65.0, 32.0 / 65.0, 56.0 / 65.0, 24.0 / 65.0, 62.0 / 65.0, 30.0 / 65.0, 54.0 / 65.0, 22.0 / 65.0,
};


// -------------------------------------
// Struct Defines
struct SwyoInputData
{
    float4  mainUV;
    float4  positionCS;
    float3  positionWS;
    float3  normalWS;
    half3   viewDirectionWS;
    float2  normalizedScreenSpaceUV;
    float4  shadowCoord;
    half4   shadowMask;
    half3   bakedGI;
    half3x3 tangentToWorld;
    half    fresnel;
#if defined(DEBUG_DISPLAY)
    half2   staticLightmapUV;
    float3  vertexSH;
#endif
};

struct SwyoTextureData
{
    half4 baseMap;
    half4 lightingMask;
    half4 normalMap;
};

struct SwyoSurfaceData
{
    half3 albedo;
    half alpha;
    half metallic;
    half smoothness;
    half specular;
    half occlusion;
    half3 emission;
    half3 normalTS;

    // Lighting Intensity
    half directIntensity;
    half indirectDiffuseIntensity;
    half indirectSpecularIntensity;
};


#define SWYO_STYLIZED_DATA      \
    half diffuseStep;           \
    half diffuseStepOffset;     \
    half3 specularColor;        \
    half shadowScale;           \
    half3 shadowColor;

#define SWYO_CLEARCOAT_DATA     \
    half clearCoatMask;         \
    half clearCoatSmoothness;

#define SWYO_SUBSURFACE_DATA    \
    half subsurfaceThickness;   \
    half3 subsurfaceColor;

struct SwyoAdditionalData
{
#if defined(SWYO_BXDF_STYLIZED)
    SWYO_STYLIZED_DATA
#endif

#if defined(SWYO_BXDF_COMPLEX_CLEARCOAT)
    SWYO_CLEARCOAT_DATA
#endif

#if defined(SWYO_BXDF_COMPLEX_SUBSURFACE)
    SWYO_SUBSURFACE_DATA
#endif
};

// Fallback for the standard or simple shader which doesn't use additional data
#if defined(NOT_USE_ADDITIONAL_DATA)
    #define SwyoInitializeAdditionalData(p0, p1, p2, p3, p4, data) data = (SwyoAdditionalData)0
#endif

struct SwyoBxDFData
{
    BRDFData base;
    BRDFData clearCoat;
};

struct SwyoLightingResult
{
    half  diffuse;
    half3 diffuseColor;
    half  specular;
    half3 specularColor;
};

struct SwyoLightingAccumulator
{
    half3 directMainLight;
    half3 directAdditionalLight;
    half3 indirect;
    half3 emission;
};

struct SwyoLightContext
{
    Light light;
    float NL;
    float NV;
    float clampedNL;
    float clampedNV;
    float clampedNH;
    float clampedLH;
    half isMainLight;
};

#endif // SWYO_COMMON_DATAS_INCLUDED