using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridBlock : GridSkill
{
    public event Action<List<GridBlockResult>> OnBlockComplete;

    public override void Init()
    {
        _ShotSlider.gameObject.SetActive(true);
        _ShotSlider.value = 0;
        _Targets[0].color = new Color(1, 0, 0, 1);
        _Targets[1].color = new Color(1, 0, 0, 1);

        if (_Coroutine != null)
        {
            StopCoroutine(_Coroutine);
        }

        _CurrentPhase = GridBlockPhase.Phase0;
        _Results = new List<GridBlockResult>();

        _Coroutine = DoMoveShotSlider(_ShotSlider);

        _PhaseChangeTimer.Start(_PhaseChangeTime);
    }
    
    private void Awake()
    {
        _PhaseChangeTimer.OnTimerComplete += OnComplete_PhaseChangeTimer;
    }

    [SerializeField]
    private Transform _Skill;
    [SerializeField]
    private Slider _ShotSlider;
    [SerializeField]
    private List<Image> _Hits;
    [SerializeField]
    private List<Image> _Targets;
    private GridBlockPhase _CurrentPhase;
    private List<GridBlockResult> _Results = new List<GridBlockResult>();

    void Update()
    {
        _PhaseChangeTimer.Update();

        if (!_IsActive)
        {
            return;
        }

        if (Input.anyKey)
        {
            _IsActive = false;

            if(_PhaseChangeTimer.IsTicking)
            {
                return;
            }

            switch(_CurrentPhase)
            {
                case GridBlockPhase.Phase1:
                    HandelPhase1();
                    return;
                case GridBlockPhase.Phase2:
                    HandelPhase2();
                    return;
            }

            if (_Coroutine != null)
            {
                StopCoroutine(_Coroutine);
            }
        }
    }

    private void HandelPhase1()
    {
        Debug.Log(_ShotSlider.value);
        var result = new GridBlockResult();

        result.Phase = GridBlockPhase.Phase1;

        if (_ShotSlider.value > .15f && _ShotSlider.value < .28f)
        {
            result.Hit = GridBlockHit.Hit;
            _Targets[0].color = new Color(0, 1, 0, 1);
        }
        else
        {
            result.Hit = GridBlockHit.Miss;
        }

        _Results.Add(result);

        Debug.Log(result.Phase);
        Debug.Log(result.Hit);
        StartPhaseChange();
    }

    private void HandelPhase2()
    {
        Debug.Log(_ShotSlider.value);
        var result = new GridBlockResult();

        result.Phase = GridBlockPhase.Phase2;

        if (_ShotSlider.value > .67f && _ShotSlider.value < .9f) 
        { 
            result.Hit = GridBlockHit.Hit;
            _Targets[1].color = new Color(0, 1, 0, 1);
        }
        else
        {
            result.Hit = GridBlockHit.Miss;
        }

        _Results.Add(result);

        Debug.Log(result.Phase);
        Debug.Log(result.Hit);
        StartPhaseChange();
    }


    private void OnComplete_PhaseChangeTimer()
    {
        if (_CurrentPhase == GridBlockPhase.Phase1)
        {
            _ShotSlider.gameObject.SetActive(true);

            _CurrentPhase = GridBlockPhase.Phase2;
            _IsActive = true;
            StartCoroutine(_Coroutine);
        }
        else if (_CurrentPhase == GridBlockPhase.Phase0)
        {
            _ShotSlider.gameObject.SetActive(true);

            _CurrentPhase = GridBlockPhase.Phase1;
            _IsActive = true;
            StartCoroutine(_Coroutine);
        }
        else if (_CurrentPhase == GridBlockPhase.Phase2)
        {
            _ShotSlider.gameObject.SetActive(false);

            OnBlockComplete?.Invoke(_Results);
        }
    }

}

public enum GridBlockPhase
{
    Phase0,
    Phase1,
    Phase2
}

public enum GridBlockHit
{
    Miss,
    Hit
}

public struct GridBlockResult
{
    public GridBlockPhase Phase;
    public GridBlockHit Hit;
}
