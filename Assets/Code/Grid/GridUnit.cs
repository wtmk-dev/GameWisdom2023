using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Febucci.UI;

public class GridUnit : GridObject, IPointerClickHandler
{
    public event Action<GridUnit> OnSelected;

    [SerializeField]
    private TextAnimatorPlayer _LifeValue, _ClockValue;
    private GridUnitStatus _StatusBar;

    public bool CanReady { get; set; }
    public CombatModel CombatModel {get; set;}
        
    public int Life
    {
        get => _Life;
        set
        {
            _Life = value;
            _LifeValue.ShowText($"{_Life}");
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        OnSelected?.Invoke(this);
    }

    private int _Life;
    private ActiveTime _ActiveTime;
    private CombatModel _ComatModel; 

    private void Awake()
    {
        _ActiveTime = new ActiveTime(this);
    }

    private void Update()
    {
        _ActiveTime.Update();             
    }
}
