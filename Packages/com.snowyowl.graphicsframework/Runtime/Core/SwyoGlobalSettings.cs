using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityObject = UnityEngine.Object;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SnowyOwl.GraphicsFramework
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "SwyoGlobalSettings.asset", menuName = CoreUtils.CreateAssetMenuPrefix + "SnowyOwl Global Settings", order = CoreUtils.EditorPriority.Core - 10)]
    public class SwyoGlobalSettings : ScriptableObject
    {
        private static SwyoGlobalSettings s_Instance;

        /// <summary>
        /// Get the settings asset selected in PlayerSettings or loaded for the Player.
        /// </summary>
        public static SwyoGlobalSettings Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    return GetPreloadedSettings();
                }
#endif
                return s_Instance;
            }
        }

        // --------------------------------
        // Render Pipeline Setting
        [Title("Render Pipeline Setting")]
        [PropertyOrder(0), OnInspectorGUI, EnableGUI]
        public bool EnableShaderDebugSymbols
        {
            get => m_EnableShaderDebugSymbols;
            set
            {
                m_EnableShaderDebugSymbols = value;
                shaderDefineGenerator.SetShaderDefine(ShaderDefines.DebugSymbolsOn, m_EnableShaderDebugSymbols);
            }
        }
        [SerializeField, HideInInspector]
        private bool m_EnableShaderDebugSymbols;
        
        [PropertyOrder(0), OnInspectorGUI, EnableGUI]
        public bool EnableDepthPriming
        {
            get => m_EnableDepthPriming;
            set
            {
                var mainRendererData = CoreUtils.GetRendererData();
                if (mainRendererData)
                {
                    m_EnableDepthPriming = value;
                    mainRendererData.depthPrimingMode = m_EnableDepthPriming ? DepthPrimingMode.Forced : DepthPrimingMode.Disabled;
                    CoreUtils.SetDirty(mainRendererData);
                }
                shaderDefineGenerator.SetShaderDefine(ShaderDefines.DepthPrimingOn, value);
            }
        }
        [SerializeField, HideInInspector]
        private bool m_EnableDepthPriming;
        
        
        // --------------------------------
        // Graphics Quality Control
        [Title("Graphics Quality Control")]
        [PropertyOrder(1)]
        public bool enableGraphicsQualityControl;
        

        // --------------------------------
        // Shader Define Generate
        [PropertyOrder(2)]
        public ShaderDefineGenerator shaderDefineGenerator;
        
        
        // --------------------------------
        // Assets Table
        [Title("Global Assets")]
        [PropertyOrder(9)]
        public SerializedDictionary<GlobalAssetDefines, UnityObject> globalAssets = new();

        /// <summary>
        /// Initialize the preloaded settings instance before Player initialization.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetInstance()
        {
#if UNITY_EDITOR
            s_Instance = GetPreloadedSettings();
#else
            s_Instance = null;
#endif
        }

        /// <summary>
        /// Register the first preloaded settings asset as the runtime instance.
        /// </summary>
        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (!s_Instance)
            {
                s_Instance = this;
            }
            else if (s_Instance != this && Application.isPlaying)
            {
                Debug.LogError("Multiple SwyoGlobalSettings assets are loaded. Only the first preloaded asset is used.", this);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Get the settings asset selected in PlayerSettings preloaded assets.
        /// </summary>
        private static SwyoGlobalSettings GetPreloadedSettings()
        {
            return PlayerSettings.GetPreloadedAssets().OfType<SwyoGlobalSettings>().FirstOrDefault();
        }
#endif

        /// <summary>
        /// Clear the runtime instance when its settings asset is unloaded.
        /// </summary>
        private void OnDisable()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }
    }

    public enum GlobalAssetDefines
    {
        DefaultSceneGraphicsManager,
        GraphicsQualityControllor,
    }
}
