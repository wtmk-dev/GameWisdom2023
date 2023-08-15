using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCatch : GridSkill
{
    public event Action<List<GridCatchResult>> OnGridCatchComplete;
    public override void Init()
    {
        _Skill.SetActive(true);

        _CurrentPhase = GridCatchPhase.Phase0;
        _Results = new List<GridCatchResult>();

        _OuterHit.color = Color.red;
        _InnterHit.color = Color.red;

        StopCorutine();

        _Coroutine = DoCatch();

        _PhaseChangeTimer.Start(_PhaseChangeTime);
    }

    private void Awake()
    {
        _PhaseChangeTimer.OnTimerComplete += OnComplete_PhaseChangeTimer;
    }

    [SerializeField]
    private GameObject _Skill;
    [SerializeField]
    private Image _OuterHit, _InnterHit;
    [SerializeField]
    private RectTransform _HitBox;

    private int _Phase = 0;
    private List<GridCatchResult> _Results = new List<GridCatchResult>();
    private GridCatchPhase _CurrentPhase;

    private Vector3 ResetHitBox()
    {
        var size = new Vector3(1f, 1f, 1f);
        _HitBox.localScale = size;
        return size;
    }

    private IEnumerator DoCatch()
    {
        var size = new Vector3(1f, 1f, 1f);
        _HitBox.localScale = size;
        yield return new WaitForSeconds(0.2f);

        while(_IsActive)
        {
            while (size.x > 0f)
            {
                size.x -= .1f;
                size.y -= .1f;
                _HitBox.localScale = size;
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.1f);

            size.x = 1;
            size.y = 1;
            _HitBox.localScale = size;
            yield return new WaitForSeconds(0.1f);
        }
    }

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
                case GridCatchPhase.Phase1:
                    HandelPhase1();
                    return;
                case GridCatchPhase.Phase2:
                    HandelPhase2();
                    return;
            }
        }
    }

    private void AddResult()
    {
        var result = new GridCatchResult();
        result.Phase = _CurrentPhase;
        result.Location = GetLocationFromHitBox();
        _Results.Add(result);

        Debug.Log($"Catch Result Added {result.Phase} :: {result.Location}");
    }

    private void StopCorutine()
    {
        if (_Coroutine != null)
        {
            StopCoroutine(_Coroutine);
        }
    }

    private void HandelPhase2()
    {
        _HitBox.gameObject.SetActive(false);
        StopCorutine();

        AddResult();
        StartPhaseChange();   
    }
    
    

    private void OnComplete_PhaseChangeTimer()
    {
        if(_CurrentPhase == GridCatchPhase.Phase1)
        {
            _HitBox.gameObject.SetActive(true);
            _CurrentPhase = GridCatchPhase.Phase2;
            _IsActive = true;

            StartCoroutine(_Coroutine);
        }
        else if (_CurrentPhase == GridCatchPhase.Phase0)
        {
            _HitBox.gameObject.SetActive(true);
            _CurrentPhase = GridCatchPhase.Phase1;
            _IsActive = true;

            StartCoroutine(_Coroutine);
        }else if(_CurrentPhase == GridCatchPhase.Phase2)
        {
            _Skill.SetActive(false);
            OnGridCatchComplete?.Invoke(_Results);
        }
    }

    private void HandelPhase1()
    {
        _HitBox.gameObject.SetActive(false);

        StopCorutine();

        AddResult();
        StartPhaseChange();
    }

    public GridCatchHitBoxLocation GetLocationFromHitBox()
    {
        var hitbox = _HitBox.transform.localScale.x;

        if(hitbox > .7)
        {
            return GridCatchHitBoxLocation.Miss;
        }else if(hitbox <= .7 && hitbox > .5f)
        {

            _OuterHit.color = Color.green;
            return GridCatchHitBoxLocation.Outter;
        }else if(hitbox <= .33f && hitbox > .13f)
        {
            _InnterHit.color = Color.green;
            return GridCatchHitBoxLocation.Inner;
        }

        return GridCatchHitBoxLocation.Miss;
    }
}

public enum GridCatchPhase
{
    Phase0,
    Phase1,
    Phase2
}

public enum GridCatchHitBoxLocation
{
    Miss,
    Inner,
    Outter
}

public struct GridCatchResult
{
    public GridCatchPhase Phase;
    public GridCatchHitBoxLocation Location;
}
