using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Febucci.UI;

public class GridUnit : GridObject , IPointerClickHandler
{
    public static event Action<GridUnit> OnBallPickedUp;

    public event Action<GridUnit> OnSelected;
    public event Action<float> OnShotComplete;
    public event Action<List<GridCatchResult>> OnCatchComplete;
    public event Action<List<GridBlockResult>> OnBlockComplete;

    [SerializeField]
    private TextAnimatorPlayer _LifeValue, _ClockValue;
    [SerializeField]
    private GridShot _Shot;
    [SerializeField]
    private GridCatch _Catch;
    [SerializeField]
    private GridBlock _Block;

    public bool HasBall 
    { 
        get => _HasBall;
    }

    public int Clock
    {
        get => _Clock;
        set
        {
            _Clock = value;
            _ClockValue.ShowText($"{_Clock}");
        }
    }

    public int Life
    {
        get => _Life;
        set
        {
            _Life = value;
            _LifeValue.ShowText($"{_Life}");
        }
    }

    public void StartCatchIndicator()
    {
        _Catch.Init();
    }

    public void StartShotIndicator()
    {
        _Shot.gameObject.SetActive(true);
        _Shot.Init();        
    }

    public void StartBlockIndicator()
    {
        _Block.gameObject.SetActive(true);
        _Block.Init();
    }

    public void PickUpBall()
    {
        OnBallPickedUp?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        OnSelected?.Invoke(this);
    }

    private int _Clock, _Life;
    private bool _HasBall;

    private void Awake()
    {
        GridUnit.OnBallPickedUp += ResolveBallPickedUp;
        _Shot.OnShotComplet += ShotComplete;
        _Catch.OnGridCatchComplete += CatchComplete;
        _Block.OnBlockComplete += BlockComplete;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            StartCatchIndicator();
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            StartBlockIndicator();
        }
    }

    private void ShotComplete()
    {
        _Shot.gameObject.SetActive(false);
        OnShotComplete?.Invoke(_Shot.Score);
    }

    private void CatchComplete(List<GridCatchResult> results)
    {
        OnCatchComplete?.Invoke(results);
    }

    private void BlockComplete(List<GridBlockResult> resutls)
    {
        OnBlockComplete?.Invoke(resutls);
    }

    private void ResolveBallPickedUp(GridUnit unit)
    {
        if(unit == this)
        {
            _HasBall = true;
            Life--;
            Debug.Log("Ball was picked up");
        }
        else
        {
            _HasBall = false;
            Debug.Log("Not by me");
        }
    }
}
