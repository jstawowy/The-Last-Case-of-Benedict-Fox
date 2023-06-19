using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using TMPro;
using GameInput;

[TrackBindingType(typeof(IVibrationProvider))]
[TrackClipType(typeof(VibrationTimelineClip))]
public class VibrationTimelineTrack : TrackAsset
{
}
