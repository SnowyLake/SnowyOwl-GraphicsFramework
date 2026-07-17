using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.Timeline;

namespace SnowyOwl.GraphicsFramework
{
    [TrackColor(1, 1, 1)]
    [TrackClipType(typeof(TODTimePeriodClip))]
    public class TODTimePeriodTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<TODTimePeriodMixer>.Create(graph, inputCount);
        }
    }
}

