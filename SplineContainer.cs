using Benfox.Graphen.Character;
using Cysharp.Threading.Tasks;
using PlotTwist.BenFox.Utilities;
using PlotTwist.Nucleus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Splines;

public class SplineContainer : MonoBehaviour
{
    private List<Transform> _splineList = new List<Transform>();
    private Transform _activeSpline;
    private int _indexHolder;
    private Vector3 _startingPosition;
    private void Start()
    {
        Prepare();
    }
    private async void Prepare()
    {
        _startingPosition = Singleton<BenedictCharacterController>.Instance.transform.position;
        Singleton<DebugCharacterController>.Instance.SetDebugMovementActive(true);
        await UniTask.Delay(200);
        Singleton<DebugCharacterController>.Instance.SetDebugMovementActive(false);
        await UniTask.Delay(200);
        Singleton<DebugCharacterController>.Instance.SetDebugMovementActive(true);
        await UniTask.Delay(200);
        //UNITY STUFF^
        var children = GetComponentInChildren<Transform>();
        foreach (Transform child in children)
        {
            _splineList.Add(child);
            child.gameObject.SetActive(false);
        }
        _activeSpline = _splineList[0];
        _indexHolder = 0;
        _activeSpline.gameObject.SetActive(true);
    }
    private async void Update()
    {
        if(_activeSpline != null)
        {
            Singleton<BenedictCharacterController>.Instance.transform.position = _activeSpline.GetComponentInChildren<SplineAnimate>().gameObject.transform.position;
            if (!_activeSpline.GetComponentInChildren<SplineAnimate>().IsPlaying)
            {
                _activeSpline.gameObject.SetActive(false);
                if (_indexHolder != _splineList.Count)
                {
                    _indexHolder += 1;

                    _activeSpline = _splineList[_indexHolder];
                    _activeSpline.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("This is done");
                    Singleton<BenedictCharacterController>.Instance.transform.position = _startingPosition;
                    Singleton<DebugCharacterController>.Instance.SetDebugMovementActive(false);
                    await UniTask.Delay(1000);
                    Addressables.ReleaseInstance(gameObject);
                   
                }
                
            }
        }
    }
}
