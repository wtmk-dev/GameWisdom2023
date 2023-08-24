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
    public event Action<UnitAction> OnQueueAction;

    [SerializeField]
    private TextAnimatorPlayer _LifeValue, _ClockValue;
    [SerializeField]
    public Image _ATB;
    [SerializeField]
    private List<Sprite> _HeroOptions;
    [SerializeField]
    private List<GameObject> _WepOptions;
    [SerializeField]
    private GameObject _Wep;
    [SerializeField]
    private Image _Sprite;

    public CombatModel CombatModel => _ComatModel;
    public GridUnitStatus StatusBar => _StatusBar;

    public List<ActivateWhenReady> ActivateWhenReady => _ActivateWhenReady;
    public List<Ability> Abilities => _Abilities;

    public int Life
    {
        get => _Life;
        set
        {
            _Life = value;
            _LifeValue.ShowText($"{_Life}");
        }
    }

    public void SetActionBar(UnitActionBar actionBar)
    {
        _ActionBar = actionBar;
    }

    public void SetActivateWhenReady(ActivateWhenReady awr)
    {
        _ActivateWhenReady.Add(awr);
    }

    public void SetAbility(Ability ability)
    {
        _Abilities.Add(ability);
    }

    public void Heal(int value)
    {
        _ComatModel.CurrentHp += value;

        if(_ComatModel.CurrentHp > _ComatModel.HP_MAX)
        {
            _ComatModel.CurrentHp = _ComatModel.HP_MAX;
        }
    }

    public void Default()
    {
        _Wep.gameObject.SetActive(true);
        _Sprite.material = null;
    }

    public bool IsInRange(Vector2 gridPosition, int range)
    {
        return true;
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

    public void Skin(CombatModel combatModel)
    {
        _ComatModel = combatModel;
        _ActiveTime = new ActiveTime(_ComatModel);
    }

    public void QueueAction(UnitAction action)
    {
        OnQueueAction?.Invoke(action);
    }

    //private UnitModel _Model;
    
    private int _Life;
    private ActiveTime _ActiveTime;
    private CombatModel _ComatModel;
    private GridUnitStatus _StatusBar;
    private List<ActivateWhenReady> _ActivateWhenReady = new List<ActivateWhenReady>();
    private List<Ability> _Abilities = new List<Ability>();
    private UnitActionBar _ActionBar;

    public void DoUpdate()
    {
        if(_ComatModel.CanReady)
        {
            if(_ComatModel.BattleState == UnitBattleState.Waiting)
            {
                _ATB.color = Color.cyan;
                _ComatModel.CanReady = false;
                _ComatModel.BattleState = UnitBattleState.Ready;

                for (int i = 0; i < _ActivateWhenReady.Count; i++)
                {
                    _ActivateWhenReady[i].Apply(this);
                }

                Debug.Log(CombatModel.Attack);

            }else if (_ComatModel.BattleState == UnitBattleState.ActionQueued)
            {
                _ComatModel.CanReady = false;
                _ComatModel.BattleState = UnitBattleState.Waiting;
            }
        }
        else
        {
            if (_ComatModel.BattleState == UnitBattleState.Waiting)
            {
                _ActiveTime.Update();
                _ATB.fillAmount = _ComatModel.WaitTime;
            }
            else if (_ComatModel.BattleState == UnitBattleState.Ready)
            {
                if(_ActionBar != null)
                {
                    _ActionBar.SetActive(true);
                }
            }
        }
    }
}