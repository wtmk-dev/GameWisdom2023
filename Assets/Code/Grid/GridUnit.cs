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

    public void Skin()
    {
        _ComatModel = new CombatModel(5, 2);
        _ActiveTime = new ActiveTime(_ComatModel);
    }

    //private UnitModel _Model;
    private int _Life;
    private ActiveTime _ActiveTime;
    private CombatModel _ComatModel;
    private GridUnitStatus _StatusBar;

    private void Update()
    {
        _ActiveTime.Update();

        if(_ComatModel.CanReady)
        {
            if(_ComatModel.BattleState == UnitBattleState.Waiting)
            {
                _ATB.color = Color.cyan;
                _ComatModel.CanReady = false;
                _ComatModel.BattleState = UnitBattleState.Ready;
            }
        }
        else
        {
            if (_ComatModel.BattleState == UnitBattleState.Waiting)
            {
                _ATB.fillAmount = _ComatModel.WaitTime;
            }
        }
    }
}