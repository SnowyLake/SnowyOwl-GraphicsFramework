using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SnowyOwl.GraphicsFramework
{
    [Serializable, VolumeComponentMenuForRenderPipeline("SnowyOwl GraphicsFramework/Rendering", typeof(UniversalRenderPipeline))]
    public class SwyoCharacterLighting : VolumeComponent
    {
        public ClampedFloatParameter directIntensity = new(1f, 0f, 2f);
        public ClampedFloatParameter indirectIntensity = new(1f, 0f, 2f);

        [Header("Custom MainLight")]
        public BoolParameter enableCustomMainLight = new(false);
        public ColorParameter customMainLightColor = new(Color.white);
        public FloatParameter customMainLightIntensity = new(1f);
    }
}
