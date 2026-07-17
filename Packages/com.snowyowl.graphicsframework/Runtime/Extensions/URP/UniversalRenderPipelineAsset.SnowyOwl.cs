using System;

namespace UnityEngine.Rendering.Universal
{
    public partial class UniversalRenderPipelineAsset
    {
        public ReadOnlySpan<ScriptableRendererData> RendererDatas => m_RendererDataList;
    }
}
