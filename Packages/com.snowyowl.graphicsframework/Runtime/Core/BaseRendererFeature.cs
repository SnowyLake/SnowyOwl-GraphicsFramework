using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SnowyOwl.GraphicsFramework
{
    public abstract class BaseRendererFeature : ScriptableRendererFeature
    {
        public bool enable = true;
        
        protected void BeforeCreate()
        {
            Dispose(true);
        }
        
        public new void SetActive(bool active)
        {
            base.SetActive(active);
            if (active)
            {
                Create();
            }
            else
            {
                Dispose(true);
            }
        }
        
        public void SetEnable(bool inEnable)
        {
            enable = inEnable;
            if (enable)
            {
                Create();
            }
            else
            {
                Dispose(true);
            }
        }
    }

    public abstract class BaseRenderPass : ScriptableRenderPass
    {
        public abstract void Dispose(bool disposing);
    }
}