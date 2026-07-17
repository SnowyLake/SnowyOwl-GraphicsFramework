using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityObject = UnityEngine.Object;
using Sirenix.OdinInspector;

namespace SnowyOwl.GraphicsFramework
{
    [CreateAssetMenu(fileName = "SwyoGlobalGraphicsSettings.asset", menuName = CoreUtils.CreateAssetMenuPrefix + "SnowyOwl Global Graphics Settings", order = CoreUtils.EditorPriority.Core - 10)]
    public class SwyoGlobalGraphicsSettings : ScriptableObject
    {
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
    }

    public enum GlobalAssetDefines
    {
        DefaultSceneGraphicsManager,
        GraphicsQualityControllor,
    }
}