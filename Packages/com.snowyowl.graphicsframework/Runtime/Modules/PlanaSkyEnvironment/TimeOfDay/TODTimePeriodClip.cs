using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SnowyOwl.GraphicsFramework
{
    [System.Serializable]
    public class TODTimePeriodClip : PlayableAsset
    {
        private TimelineClip m_Clip;
        
        // Factory method that generates a playable based on this asset
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TODTimePeriodBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.x = x;
            return playable;
        }
        
        public float x;

        [Button]
        public void SetToThisTimePeriod()
        {
            // var controller = TODController.Instance;
            // // controller.normalizedTime;
            // var director = controller.GetTimelineDirector();
            // if (director.playableAsset is TimelineAsset timeline)
            // {
            //     foreach (var track in timeline.GetOutputTracks())
            //     {
            //         foreach (var clip in track.GetClips())
            //         {
            //             if (clip.asset == this)
            //                 return clip;
            //         }
            //     }
            // }
            

        }
    }
}
