using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Serialization;

public class VibrationTimelineClip : PlayableAsset
{
    public AnimationCurve Curve;


    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VibrationTimelineBehaviour>.Create(graph);
        VibrationTimelineBehaviour vibrationTimelineBehaviour = playable.GetBehaviour();
        vibrationTimelineBehaviour.curve = Curve;
        return playable;
    }

       
}
