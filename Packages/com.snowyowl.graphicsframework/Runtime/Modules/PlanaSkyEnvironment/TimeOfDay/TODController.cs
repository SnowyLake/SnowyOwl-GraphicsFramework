using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace SnowyOwl.GraphicsFramework
{
    [RequireComponent(typeof(PlayableDirector))]
    [ExecuteAlways]
    public class TODController : MonoBehaviour
    {
        public static TODController Instance { get; private set; }
        
        [PropertyRange(0.0f, 1.0f)]
        public float normalizedTime;
        
        private void OnEnable()
        {
            Instance = this;
        }
        
        private void OnDisable()
        {
            Instance = null;
        }
        
        void Update()
        {
            OnUpdate(Instance);
        }

        private static void OnUpdate(TODController controller)
        {
            var director = controller.GetTimelineDirector();
            director.time = controller.normalizedTime * 24;
            director.Evaluate();
        }

        public PlayableDirector GetTimelineDirector()
        {
            return GetComponent<PlayableDirector>();
        }
    }
}
