using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Febucci.UI;

public class GridUnit : GridObject, IPointerClickHandler
{
    public event Action<GridUnit> OnSelected;

    [SerializeField]
    private TextAnimatorPlayer _LifeValue, _ClockValue;
    [SerializeField]
    public Image _ATB;
    
    public bool CanReady { get; set; }
    public CombatModel CombatModel => _ComatModel;
    public GridUnitStatus StatusBar => _StatusBar;

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

    public void Init(int hp, float speed, GridUnitStatus stats)
    {
        _ComatModel = new CombatModel(hp, speed);
        _ActiveTime = new ActiveTime(_ComatModel);
        _StatusBar = stats;
    }

    private int _Life;
    private ActiveTime _ActiveTime;
    private CombatModel _ComatModel;
    private GridUnitStatus _StatusBar;

    private void Update()
    {
        _ActiveTime.Update();
        _ATB.fillAmount = _ComatModel.WaitTime;
    }
}