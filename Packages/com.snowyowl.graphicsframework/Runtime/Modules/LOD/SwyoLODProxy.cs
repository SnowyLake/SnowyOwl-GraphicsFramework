using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnowyOwl.GraphicsFramework
{
    [Serializable]
    public enum LODTag
    {
        Small = 0,
        ImportantSmall,
        Medium,
        ImportantMedium,
        Large,
    }
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LODGroup))]
    [ExecuteAlways]
    public class SwyoLODProxy : MonoBehaviour
    {
        public LODTag lodTag;
        public bool lodTransitionOverride;
        
        public LODGroup lodGroup => GetComponent<LODGroup>();
        
        public float lod0Transition
        {
            get => LODTransitionGet(0);
            set => LODTransitionSet(0, value);
        }
        
        public float lod1Transition
        {
            get => LODTransitionGet(1);
            set => LODTransitionSet(1, value);
        }
                
        public float lod2Transition
        {
            get => LODTransitionGet(2);
            set => LODTransitionSet(2, value);
        }
        
        private float LODTransitionGet(int index)
        {
            if (lodGroup.lodCount < index + 1)
            {
                return -1f;
            }
            var lods = lodGroup.GetLODs();
            return lods[index].screenRelativeTransitionHeight;
        }

        private void LODTransitionSet(int index, float value)
        {
            if (lodGroup.lodCount < index + 1)
            {
                return;
            }
            var lods = lodGroup.GetLODs();
            lods[index].screenRelativeTransitionHeight = value;
            lodGroup.SetLODs(lods);
        }
    }
}