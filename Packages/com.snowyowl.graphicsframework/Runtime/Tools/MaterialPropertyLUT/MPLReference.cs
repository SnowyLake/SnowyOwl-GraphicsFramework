using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Collections.LowLevel.Unsafe;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
    using Unity.Mathematics;
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif

namespace SnowyOwl.GraphicsFramework
{
    [ExecuteAlways]
    public class MPLReference : MonoBehaviour
    {
        [InlineButton("SetPropertyLUT")]
        public MPLGenerator generator;
        
#if UNITY_EDITOR
        private bool m_Editing;
        public bool editing
        {
            get => m_Editing;
            set
            {
                var r = GetComponent<Renderer>();
                var mat = r?.sharedMaterial;
                if (!generator || !r || !mat)
                {
                    return;
                }
                
                m_Editing = value;
                if (m_Editing)
                {
                    mat.EnableKeyword(SwyoShaderKeywords.PropertyLUTEditing);
                }
                else
                {
                    mat.DisableKeyword(SwyoShaderKeywords.PropertyLUTEditing);
                    r.SetPropertyBlock(null);
                    generator?.GenerateTexture();
                    m_ComputeBuffer?.Release();
                }
            }
        }
        
        [HideIf("editing"), PropertyOrder(-1)]
        [Button(ButtonSizes.Large, Name = "Realtime Editing"), GUIColor(0, 1, 0)]
        private void EditingBeginButton()
        {
            editing = true;
        }

        [ShowIf("editing"), PropertyOrder(-1)]
        [Button(ButtonSizes.Large, Name = "End Editing And Save"), GUIColor(1, 0.2f, 0)]
        private void EditingEndButton()
        {
            editing = false;
        }
        
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowInInspector, ShowIf("m_Editing"), EnableGUI, EnableIf("m_Editing")]
        public MPLGenerator Generator => generator;
        
        private ComputeBuffer m_ComputeBuffer;

        private void Update()
        {
            if (!editing)
            {
                return;
            }
            
            var rend = GetComponent<Renderer>();
            var mat = rend?.sharedMaterial;
            if (!generator || !rend || !mat)
            {
                return;
            }

            var templete = generator.templete;
            var itemGroups = generator.itemGroups;
            var validPixelCount = MPLUtils.GetValidPixelCount(templete);
            
            var buffer = new List<float4>();
            foreach (var itemGroup in itemGroups)
            {
                var itemDatas = itemGroup.itemDatas.value;
                for (var pixelIdx = 0; pixelIdx < templete.groupPixelCount; pixelIdx++)
                {
                    if (pixelIdx < validPixelCount)
                    {
                        var pixelData = float4.zero;
                        var pixelLayout = templete.pixelLayouts[pixelIdx];

                        if (pixelLayout.layoutType == MPLPixelLayoutType.Vector3AndScalar1)
                        {
                            pixelData.xyz = MPLUtils.GetPixelDataVector3(itemDatas, pixelLayout.channelRGB);
                            pixelData.w = MPLUtils.GetPixelDataScalar(itemDatas, pixelLayout.channelA);
                        }
                        else if (pixelLayout.layoutType == MPLPixelLayoutType.Scalar4)
                        {
                            pixelData.x = MPLUtils.GetPixelDataScalar(itemDatas, pixelLayout.channelR);
                            pixelData.y = MPLUtils.GetPixelDataScalar(itemDatas, pixelLayout.channelG);
                            pixelData.z = MPLUtils.GetPixelDataScalar(itemDatas, pixelLayout.channelB);
                            pixelData.w = MPLUtils.GetPixelDataScalar(itemDatas, pixelLayout.channelA);
                        }
                        else //(pixelLayout.layoutType == MPLPixelLayoutType.Vector4)
                        {
                            pixelData = MPLUtils.GetPixelDataVector4(itemDatas, pixelLayout.channelRGBA);
                        }

                        buffer.Add(pixelData);
                    }
                }
            }
            
            m_ComputeBuffer?.Release();
            m_ComputeBuffer = new ComputeBuffer(buffer.Count, UnsafeUtility.SizeOf<Vector4>());
            
            m_ComputeBuffer.SetData(buffer);
            
            var propertyBlock = new MaterialPropertyBlock();
            
            propertyBlock.SetBuffer(SwyoShaderPropertyId.PropertyLUTEditingData, m_ComputeBuffer);
            propertyBlock.SetFloat(SwyoShaderPropertyId.PropertyLUTEditingEnable, 1.0f);
            propertyBlock.SetInteger(SwyoShaderPropertyId.PropertyLUTFunctionCount, validPixelCount);
            
            rend.SetPropertyBlock(propertyBlock);
        }

        public void SetPropertyLUT()
        {
            var mat = MaterialMeshUtils.GetSharedMaterial(gameObject);
            if (!mat)
            {
                return;
            }
            mat.EnableKeyword(SwyoShaderKeywords.PropertyLUTOn);
            mat.SetFloat(SwyoShaderPropertyId.PropertyLUTOn, 1.0f);
            mat.SetTexture(SwyoShaderPropertyId.PropertyLUT, generator.Texture);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnDisable()
        {
            if (editing)
            {
                editing = false;
            }
        }
        
        private void Awake()
        {
            EditorSceneManager.sceneSaving += OnSceneSavingInEditMode;
        }

        private void OnDestroy()
        {

            EditorSceneManager.sceneSaving -= OnSceneSavingInEditMode;
        }
        
        private void OnSceneSavingInEditMode(Scene scene, string path)
        {
            if (editing)
            {
                editing = false;
            }
        }
#endif
    }
}