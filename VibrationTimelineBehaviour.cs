using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using GameInput;
using PlotTwist.Nucleus;

public class VibrationTimelineBehaviour : PlayableBehaviour
{
    #region Variables
    public float leftPower;
    public float rightPower;
    public AnimationCurve curve;
    public float curvetime;

    public Vector2 offset;
    public float curveratio;
    private float _timer = 0f;
    public Rewired.ControllerType controllerType;
    #endregion

    #region Behaviour
    public virtual void OnGraphStart(Playable playable)
    {
        curvetime = (float)playable.GetDuration();
        offset.x = curvetime;
        offset.y = 1f;

    }

    public virtual void OnBehaviourPause(Playable playable, FrameData info)
    {
        Singleton<GameInput.GameInputManager>.Instance.StopPlayerGamepadVibration();
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        controllerType = InputReceiver.Input.ActiveControllerType;
        if(controllerType == Rewired.ControllerType.Keyboard) { return; }
        _timer += Time.deltaTime;
        if (_timer > curvetime)
        {
            _timer = curvetime;
        }
        curveratio = _timer / curvetime;
        if (PlotTwist.BenFox.Options.GameOptions.Vibrations)
        {
            Vector2 positionOffset = curve.Evaluate(curveratio) * offset;
            rightPower = positionOffset.y;
            leftPower = positionOffset.y;

            if (curveratio == 1)
            {
                Singleton<GameInput.GameInputManager>.Instance.StopPlayerGamepadVibration();
            }
            else
            {
                Singleton<GameInput.GameInputManager>.Instance.StartPlayerGamepadVibration(leftPower, rightPower);
            }
        }
        

    }
    #endregion
}
