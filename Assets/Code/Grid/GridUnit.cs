using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Febucci.UI;
using DG.Tweening;

public class GridUnit : GridObject, IPointerClickHandler
{
    public event Action<GridUnit> OnSelected;
    public event Action<UnitAction> OnQueueAction;
    public event Action<GridUnit> OnKilled;

    [SerializeField]
    private TextAnimatorPlayer _LifeValue, _AttackValue;
    [SerializeField]
    public Image _ATB;
    [SerializeField]
    private List<Sprite> _HeroOptions;
    [SerializeField]
    private List<GameObject> _WepOptions;
    [SerializeField]
    private GameObject _Wep, _goLife, _goAtk;
    [SerializeField]
    private Image _Sprite;
    [SerializeField]
    private Sprite _Necro, _Soul, _DeadSkelly, _AliveSkelly;

    public CombatModel CombatModel => _ComatModel;
    public GridUnitStatus StatusBar => _StatusBar;

    public List<ActivateWhenReady> ActivateWhenReady => _ActivateWhenReady;
    public List<Ability> Abilities => _Abilities;
    public Move Move { get; set; }
    public Image Body => _Sprite;
    public int Option;

    public UnitActionBar ActionBar => _ActionBar;

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

    public void SetPassive(Passive passive)
    {
        _Passive.Add(passive);
    }

    public void SetCounter(Counter counter)
    {
        _Coutner.Add(counter);
    }

    public AliveState AliveState { get; set; }

    public void Rez()
    {
        _ComatModel.CurrentHp = _ComatModel.HP_MAX;
        AliveState = AliveState.Default;
        _Sprite.sprite = _AliveSkelly;
        ShowAliveItems();
    }

    public void DropSoul()
    {
        AliveState = AliveState.Soul;
        _Sprite.sprite = _Soul;
        HideAliveItems();
    }

    public void Kill(bool cleanBody = true)
    {
        AliveState = AliveState.Dead;
        _Sprite.sprite = _DeadSkelly;
        HideAliveItems();

        if(cleanBody)
        {
            StartCoroutine(CleanBody());
        }
    }

    private IEnumerator CleanBody()
    {
        yield return new WaitForSeconds(1.2f);
        CurrentPosition.IsOccupied = false;
    }

    private void HideAliveItems()
    {
        _goLife.gameObject.SetActive(false);
        _ATB.gameObject.SetActive(false);
        _goAtk.gameObject.SetActive(false);
        _Wep.gameObject.SetActive(false);
    }

    private void ShowAliveItems()
    {
        _goLife.gameObject.SetActive(true);
        _ATB.gameObject.SetActive(true);
        _goAtk.gameObject.SetActive(true);
        _Wep.gameObject.SetActive(true);
    }
    
    public bool IsBoss { get; set; }
    public void SetBoss(bool isBoss)
    {
        IsBoss = isBoss;
        StartCoroutine(DoSetBoss(isBoss));
    }

    IEnumerator DoSetBoss(bool isBoss)
    {
        yield return new WaitForEndOfFrame();
        if (isBoss)
        {
            _Sprite.sprite = _Necro;
            _WepOptions[2].SetActive(true);
        }
        else
        {
            _WepOptions[0].SetActive(true);
        }

        UpdateValues();
    }

    public void Damage(int value)
    {
        _ComatModel.CurrentHp -= value;

        if (_ComatModel.CurrentHp > _ComatModel.HP_MAX)
        {
            _ComatModel.CurrentHp = _ComatModel.HP_MAX;
        }
    }

    public void UpdateValues()
    {
        if(CombatModel == null)
        {
            return;
        }

        _LifeValue.ShowText(CombatModel.CurrentHp + "");
        _AttackValue.ShowText(CombatModel.Attack + "");
    }

    public void Heal(int value)
    {
        _ComatModel.CurrentHp += value;

        if(_ComatModel.CurrentHp > _ComatModel.HP_MAX)
        {
            _ComatModel.CurrentHp = _ComatModel.HP_MAX;
        }
    }

    public void Default(int option)
    {
        Option = option;

        _Wep.gameObject.SetActive(true);

        for (int i = 0; i < _WepOptions.Count; i++)
        {
            _WepOptions[i].SetActive(false);
        }

        _WepOptions[option].gameObject.SetActive(true);
        _Sprite.material = null;

        UpdateValues();
        gameObject.SetActive(false);
    }
    
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        OnSelected?.Invoke(this);
    }

    public void Init(int hp, float speed, GridUnitStatus stats)
    {
        _ComatModel = new CombatModel(hp, speed, 0,0);
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

    private ActiveTime _ActiveTime;
    private CombatModel _ComatModel;
    private GridUnitStatus _StatusBar;
    private List<ActivateWhenReady> _ActivateWhenReady = new List<ActivateWhenReady>();
    private List<Ability> _Abilities = new List<Ability>();
    private List<Passive> _Passive = new List<Passive>();
    private List<Counter> _Coutner = new List<Counter>();

    private RNG _RNG = new RNG();
    private UnitActionBar _ActionBar;

    public void DoAttack(GridUnit unit, int value)
    {
        unit.Damage(value);
        transform.DOLocalMove(unit.transform.localPosition, 0.3f).OnComplete(()=> 
        {
            transform.DOLocalMove(CurrentPosition.transform.localPosition, 0.3f);
            unit.transform.DOPunchRotation(Vector2.left, 0.3f);
        });
    }

    public void DoHeal(GridUnit unit, int value)
    {
        unit.Heal(value);
        unit.transform.DOShakePosition(0.6f);
    }

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
                    Debug.LogWarning("TO DO ACTIVATE WHEN READY");
                }

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
        }

        if (CombatModel.CurrentHp <= 0)
        {
            if (AliveState == AliveState.Default)
            {
                AliveState = AliveState.PendingDeath;
                OnKilled?.Invoke(this);
            }else if(AliveState == AliveState.PendingDeath && IsBoss)
            {
                OnKilled?.Invoke(this);
            }
        }

        if (_ActionBar == null)
        {
            return;
        }

        if (_ComatModel.BattleState == UnitBattleState.SelectingAction || _ComatModel.BattleState == UnitBattleState.Ready)
        {
            if(_ComatModel.BattleState == UnitBattleState.SelectingAction)
            {
                _ActionBar.SelectingAction();
            }
            else
            {
                _ActionBar.Unselect();
            }
        }
        else
        {
            _ActionBar.SetActive(false);
            
        }
    }
}

public enum AliveState
{
    Default,
    PendingDeath,
    Dead,
    Soul
}