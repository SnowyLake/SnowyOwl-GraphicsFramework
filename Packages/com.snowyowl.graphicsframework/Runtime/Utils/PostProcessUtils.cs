using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SnowyOwl.GraphicsFramework
{
    public static class PostProcessUtils
    {
        public static Volume GetGlobalVolume()
        {
            var globalSettings = CoreUtils.GetGlobalSettings();
            var volumes = VolumeManager.instance.GetVolumes(-1);
            return volumes.First(v => v.isGlobal);
        }
        
        public static void SetGlobalVolumeComponentActive<T>(bool active) where T : VolumeComponent
        {
            var volume = GetGlobalVolume();
            if (volume.GetProfile().TryGet(out T component))
            {
                component.active = active;
            }
            else
            {
                Debug.LogWarning($"PostProcessUtils: Current global post-process volume don't have {typeof(T).Name} component!");
            }
        }
        
        public static void SetGlobalVolumeBloomActive(bool active)
        {
            SetGlobalVolumeComponentActive<Bloom>(active);
        }
        
        public static void SetGlobalVolumeVignetteActive(bool active)
        {
            SetGlobalVolumeComponentActive<Vignette>(active);
        }
    }
}
