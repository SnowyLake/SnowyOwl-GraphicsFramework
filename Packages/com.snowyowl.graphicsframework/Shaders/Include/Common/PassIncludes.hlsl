#pragma once

#include_with_pragmas "Packages/com.snowyowl.graphicsframework/Shaders/Include/Generated/GlobalShaderDefines.hlsl"

// -------------------------------------
// Macro Defines
#if !defined(SWYO_BXDF_STYLIZED)
    #define NOT_USE_ADDITIONAL_DATA
#endif

#if defined(_REFLECTION_NONE)
    #define _ENVIRONMENTREFLECTIONS_OFF
#endif


// -------------------------------------
// Shader Include
#if defined(SWYO_SHADER_CHARACTER_LIT_STYLIZED)
    #include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Character/CharacterLitStylizedInput.hlsl"
#endif

#if defined(SWYO_SHADER_CHARACTER_FACE_LIT_STYLIZED)
    #include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Character/CharacterFaceLitStylizedInput.hlsl"
#endif

#if defined(SWYO_SHADER_SCENE_LIT_STARDAND)
    #include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Scene/SceneLitStandardInput.hlsl"
#endif

#include "Packages/com.snowyowl.graphicsframework/Shaders/Include/Common/Lighting.hlsl"