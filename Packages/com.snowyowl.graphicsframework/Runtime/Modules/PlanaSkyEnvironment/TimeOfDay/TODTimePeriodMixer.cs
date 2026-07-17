using System.Collections;
using System.Collections.Generic;
using SnowyOwl.GraphicsFramework;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class TODTimePeriodMixer : PlayableBehaviour
{

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        int inputCount = playable.GetInputCount();
        float mix = 0;
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            var inputPlayable = (ScriptPlayable<TODTimePeriodBehaviour>)playable.GetInput(i);
            var behaviour = inputPlayable.GetBehaviour();
            mix += behaviour.x * inputWeight;
        }
        // Debug.LogWarning(mix);
    }
}
