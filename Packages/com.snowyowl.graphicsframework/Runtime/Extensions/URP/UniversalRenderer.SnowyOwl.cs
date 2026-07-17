using UnityEngine.Rendering;

namespace UnityEngine.Rendering.Universal
{
    public sealed partial class UniversalRenderer
    {
        public RTHandle DepthTexture => m_DepthTexture;
        public CopyDepthMode CopyDepthMode => m_CopyDepthMode;
    }
}
