using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace SnowyOwl.GraphicsFramework
{
    [TypeRegistryItem(Name = "Time Of Day")]
    public class TODComponent : WorldGraphicsComponent
    {
        [PropertyRange(0.0f, 1.0f)]
        public float normalizedTime;

        public PlayableDirector director;
        
        public override void Update(SwyoWorldGraphicsSettings owner)
        {
            if (!director)
            {
                return;
            }
            
            director.time = normalizedTime * 24;
            director.Evaluate();
        }
    }
}
