using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace SnowyOwl.GraphicsFramework
{
    public static class LayerMaskUtils
    {
        public static readonly LayerMask Everything = ~0;
        
        public static int GetLayerIndex(LayerMask layer)
        {
            return Mathf.RoundToInt(Mathf.Log(layer, 2));
        }
    }
    
    public static class CommonUtils
    {
        public static void DestroyObject(UnityEngine.Object obj)
        {
            if (obj)
            {
#if UNITY_EDITOR
                if (Application.isPlaying && !EditorApplication.isPaused)
                    UnityEngine.Object.Destroy(obj);
                else
                    UnityEngine.Object.DestroyImmediate(obj);
#else
                UnityObject.Destroy(obj);
#endif
            }
        }
        
        public static string GetGameObjectScenePath(GameObject obj)
        {
            var path = obj.name;
            var parent = obj.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }

        public static uint ToggleBit(uint target, uint value, bool toggle)
        {
            return toggle ? target |= value : target &= ~value;
        }
        
        public static uint ReplaceBit(uint target, uint value, uint mask)
        {
            return target = (target & ~mask) | (value & mask);
        }
        
        public static string AbsoluteToRelativePath(string absolutePath)
        {
            var assetsPath = Application.dataPath;
            if (absolutePath.StartsWith(assetsPath))
            {
                return "Assets" + absolutePath[assetsPath.Length..];
            }
            
            var packagesPath = assetsPath[..assetsPath.LastIndexOf("Assets", StringComparison.Ordinal)] + "Packages";
            if (absolutePath.StartsWith(packagesPath))
            {
                return "Packages" + absolutePath[packagesPath.Length..];
            }
            
            return null;
        }
    }

    public static class CommonExtensions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }
        
        public static IEnumerable<(int Index, T Item)> WithIndex<T>(this IEnumerable<T> source)
        {  
            var index = 0;
            foreach (var item in source)
            {
                yield return (index++, item);
            }
        }
        
        public static T TryAddComponent<T>(this GameObject obj) where T : Component
        {
            var component = obj?.GetComponent<T>();
            return component ? component : obj?.AddComponent<T>();
        }

        public static Component TryAddComponent(this GameObject obj, Type componentType)
        {
            var component = obj?.GetComponent(componentType);
            return component ? component : obj?.AddComponent(componentType);
        }

        public static void TryRemoveComponent<T>(this GameObject obj) where T : Component
        {
            var component = obj?.GetComponent<T>();
            CommonUtils.DestroyObject(component);
        }
        public static void TryRemoveComponent(this GameObject obj, Component component)
        {
            CommonUtils.DestroyObject(component);
        }
        
        public static VolumeProfile GetProfile(this Volume volume)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                return volume?.sharedProfile;
            }
            else
#endif
            {
                return volume?.profile;
            }
        }
    }
}
