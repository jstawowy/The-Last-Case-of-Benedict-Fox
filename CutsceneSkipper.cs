using UnityEngine;
using UnityEngine.Playables;
using GameInput;
using PlotTwist.Nucleus;
using PlotTwist.BenFox.UI;
using System;
using Cysharp.Threading.Tasks;
using PlayFab;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using BenFox.GameData;

public class CutsceneSkipper : MonoBehaviour, IInputCutscene
{
	public event Action CutsceneStopped;
	public event Action CutsceneSkipped;

	private PlayableDirector _currentDirector;
	private bool _inputRejected;
	private bool _inputReceived;
	private bool _skipAllowed;
	private double _skipBlockOffset;
	private double BtnHoldTime;
	private float progress;
	private bool _skipCurtain;
	private bool _customSkip;
	private float _customSkipTime;
	private bool customSkipExecuted;
	private TimelineAsset someTimelineAsset;
	private bool mutetrack;
    private IEnumerable<TrackAsset> tracklist;
    private IEnumerable<TimelineClip> cliplist;
	private List<TrackAsset> toMuteList = new List<TrackAsset>();
	private List<string> ClipsToMuteFromSignal = new List<string>();
	private int _numberOfTracksToMute;
	private MiscGameData _miscGameData;
	private bool _muteExecuted;

    private void Update()
	{
		if (_skipAllowed & _inputReceived)
		{
			if(UIManager.Instance != null)
            {
				if (_currentDirector.time >= _skipBlockOffset)
				{


					RejectInput();
					UIManager.Instance.SetCutsceneSkipProgress(0);
					UIManager.Instance.HideCutscenesSkipInfo();
					_skipAllowed = false;

				}


				else if (!UIManager.Instance.PanelCutscenesSkip.IsActive() & _inputReceived & _skipAllowed)
				{

					foreach (var c in Rewired.ReInput.controllers.Controllers)
					{
						foreach (var e in c.PollForAllElements())
							ButtonPressedOrAxisActive();
					}

				}
			}
			
		}
	}
	public void OnDestroy()
	{
		if (UIManager.Instance != null)
		{
			if (UIManager.Instance.PanelCutscenesSkip != null)
			{
				
					UIManager.Instance.HideCutscenesSkipInfo();
				
			}

			if (UIManager.Instance.PanelSubtitles != null)
			{
				if (UIManager.Instance.PanelSubtitles.isActive)
				{
					UIManager.Instance.HideSubtitlesForce();
				}
			}
		}
		if (toMuteList != null & toMuteList.Count > 0)
		{
			foreach (TrackAsset track in toMuteList)
			{
				if (track.muted) track.muted = false;
			}
		}

	}
	public void OnUIDeactivated()
	{
	}
	public void OnUIActivated()
	{
		
	}
	public void GetDirector(PlayableDirector director)
	{

			if (!_muteExecuted)
			{
				_skipAllowed = true;
				_currentDirector = director;
				_skipBlockOffset = _currentDirector.duration - 7;
				BtnHoldTime = 1d;
				someTimelineAsset = (TimelineAsset)_currentDirector.playableAsset;

				if (_miscGameData == null)
					_miscGameData = Singleton<GameData>.Instance.Misc;
            if (!_miscGameData.CutscenesWithSkipperAttached.ContainsKey(_currentDirector.name))
            {
				_miscGameData.CutscenesWithSkipperAttached.Add(_currentDirector.name, false);
			}

				UIManager.Instance.PanelCutscenesSkip.OnDeactivated -= OnUIDeactivated;
				UIManager.Instance.PanelCutscenesSkip.OnActivated -= OnUIActivated;
				UIManager.Instance.PanelCutscenesSkip.OnDeactivated += OnUIDeactivated;
				UIManager.Instance.PanelCutscenesSkip.OnActivated += OnUIActivated;
				RequestInput();
				_currentDirector.stopped -= _currentDirector_stopped;
				_currentDirector.stopped += _currentDirector_stopped;
			}
		

		
		
	}
	public void CustomSkipReceiver(float skipTime)
    {
		_customSkipTime = (float)_currentDirector.initialTime + skipTime;
		_customSkip = true;
	}
	public void OnDisable()
	{
		if (UIManager.Instance != null)
		{
			if (UIManager.Instance.PanelCutscenesSkip != null)
			{
				
					UIManager.Instance.HideCutscenesSkipInfo();
				
			}

			if (UIManager.Instance.PanelSubtitles != null)
			{
				if (UIManager.Instance.PanelSubtitles.isActive)
				{
					UIManager.Instance.HideSubtitlesForce();
				}
			}
		}
		if (toMuteList != null & toMuteList.Count > 0)
		{
			foreach (TrackAsset track in toMuteList)
			{
				if (track.muted) track.muted = false;
			}
		}


		if (!_inputRejected)
		{
			RejectInput();
		}
	}

	private void _currentDirector_stopped(PlayableDirector obj)
	{
		if (!_inputRejected)
		{
			RejectInput();
		}
		if (UIManager.Instance != null)
		{
			if (UIManager.Instance.PanelCutscenesSkip != null)
			{
				
					UIManager.Instance.HideCutscenesSkipInfo();
				
			}
		}
		if (toMuteList != null & toMuteList.Count > 0)
        {
			foreach (TrackAsset track in toMuteList)
			{
				if(track.muted) track.muted = false;
			}
		}
		
		CutsceneStopped?.Invoke();
		_currentDirector.stopped -= _currentDirector_stopped;
		
	}
	public void OnInputRejected()
	{
		_inputRejected = true;
		UIManager.Instance.PanelCutscenesSkip.OnDeactivated -= OnUIDeactivated;
		UIManager.Instance.PanelCutscenesSkip.OnActivated -= OnUIActivated;
	}

	public void OnInputReceived()
	{
		_inputReceived = true;
	}

	public void RequestInput() { Singleton<GameInputManager>.Instance.RegisterInput(this as IInput); }

	public void RejectInput() { Singleton<GameInputManager>.Instance.UnregisterInput(this as IInput); }

	public async void Skip_JustPressed()
	{
	}

	public async void Skip_JustLongPressed()
	{
	}

	public async void Skip_Press(double pressTime)
	{
		if (_currentDirector == null)
			return;

		

		if(_customSkip & !customSkipExecuted & _currentDirector.time <= _customSkipTime - 2)
        {
			if (UIManager.Instance.PanelCutscenesSkip.IsActive() & _skipAllowed)
			{
				progress = (float)(pressTime / BtnHoldTime);
				UIManager.Instance.SetCutsceneSkipProgress(progress);
				if (progress >= 1)
				{
					progress = 0;
					customSkipExecuted = true;
					if(!_miscGameData.CutsceneSkipExecuted) _miscGameData.CutsceneSkipExecuted = true;
                    if (_miscGameData.CutscenesWithSkipperAttached.ContainsKey(_currentDirector.name))
                    {
						_miscGameData.CutscenesWithSkipperAttached[_currentDirector.name] = true;
                    }
					if (mutetrack)
					{
						MuteTrack();
					}
					RejectInput();
					
					UIManager.Instance.HideSubtitlesForce();
					UIManager.Instance.HideCutscenesSkipInfo();
					await UIManager.Instance.ShowCurtainAsync(1);
					_currentDirector.time = _customSkipTime;
					UIManager.Instance.HideCurtain(1);
					UIManager.Instance.SetCutsceneSkipProgress(0);
					RequestInput();
					

				}
			}
		}
		else
		{
			if (UIManager.Instance.PanelCutscenesSkip.IsActive() & _skipAllowed)
			{
				progress = (float)(pressTime / BtnHoldTime);
				UIManager.Instance.SetCutsceneSkipProgress(progress);
				if (progress >= 1)
				{
					progress = 0;
					if (!_miscGameData.CutsceneSkipExecuted) _miscGameData.CutsceneSkipExecuted = true;
					if (_miscGameData.CutscenesWithSkipperAttached.ContainsKey(_currentDirector.name))
					{
						_miscGameData.CutscenesWithSkipperAttached[_currentDirector.name] = true;
					}
					PlayFabService.WriteTelemetryEvent("cutscene_skip", new
					{
						name = _currentDirector.playableAsset.name,
						time = _currentDirector.time,
						duration = _currentDirector.duration
					});

					RejectInput();
					
					UIManager.Instance.HideSubtitlesForce();
					await UIManager.Instance.ShowCurtainAsync(1);


					_skipAllowed = false;
					_currentDirector.time = _currentDirector.duration - 1;
					if (UIManager.Instance.PanelSubtitles.isActive)
					{
						UIManager.Instance.HideSubtitlesForce();
					}

					UIManager.Instance.HideCutscenesSkipInfo();
					if (!_skipCurtain)
					{
						await UniTask.Delay(3000);
					}

					UIManager.Instance.HideCurtain(1);

					CutsceneSkipped?.Invoke();
				}

			}
		}
	}
	public void SkipCurtainAwait(bool skip)
	{
		_skipCurtain = skip;
	}
	public void Skip_Released()
	{

		if (UIManager.Instance.PanelCutscenesSkip.IsActive())
		{
			progress = 0;
			UIManager.Instance.SetCutsceneSkipProgress(0);
		}
	}

	public void GetMuteTrackSignal(string trackName)
    {
        if (!_muteExecuted)
        {
			mutetrack = true;
			ClipsToMuteFromSignal.Add(trackName);
			if (ClipsToMuteFromSignal.Count == _numberOfTracksToMute) FindTrack(ClipsToMuteFromSignal);
		}
		

	}
	public void GetMuteLenght(int NumberOfTracks)
    {
		_numberOfTracksToMute = NumberOfTracks;
    }
	public void MuteTrack()
	{
		if (toMuteList.Count > 0 & toMuteList != null)
        {
			foreach (TrackAsset track in toMuteList)
			{
				track.muted = true;
			}
			_muteExecuted = true;
			double t = _currentDirector.time; // Store elapsed time
			_currentDirector.RebuildGraph(); // Rebuild graph
			_currentDirector.time = t; // Restore elapsed time
			
		}
		
		//if (someTimelineTrackAsset != null)
		//  {
		//	someTimelineTrackAsset.muted = true;

		//	double t = _currentDirector.time; // Store elapsed time
		//	_currentDirector.RebuildGraph(); // Rebuild graph
		//	_currentDirector.time = t; // Restore elapsed time
		//}
		
	}

	public void FindTrack(List<string> ClipsToMute)
    {
		bool tracksFound = false;
		tracklist = someTimelineAsset.GetOutputTracks();
		//Debug.Log(tracklist);
        foreach (var track in tracklist)
        {
			if(track.name.Contains("FMOD Event Track"))
            {
				cliplist = track.GetClips();
				foreach (var clip in cliplist)
				{
					foreach (string str in ClipsToMute)
					{
						if (clip.displayName == str)
						{
							toMuteList.Add(track);
							if (toMuteList.Count == ClipsToMute.Count)
							{
								tracksFound = true;
								break;
							}
						}
						
					}
					if (tracksFound) break;
				}
			}
        }

	}
	private void ButtonPressedOrAxisActive()
	{
		if (UIManager.Instance.PanelCutscenesSkip.IsActive()) return;
		UIManager.Instance.ShowCutscenesSkipInfo();
	}
}
