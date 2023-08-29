using System;
using System.Collections.Generic;
using UnityEngine;
using eff = WTMK.Mechanics;
using DG.Tweening;

public class BattleSystem
{
    private StateActionMap<BattleState> _BattleState;
    public static int Difficulty = 14;

    public void Update()
    {
        if (_Player.AliveState == AliveState.Dead || _Player.AliveState == AliveState.Soul)
        {
            if (_BattleState.CurrentState != BattleState.BadEnd)
            {
                if (_ActionBar != null)
                {
                    _ActionBar.SetActive(false);
                }

                _BattleState.StateChange(BattleState.BadEnd);
            }

        }

        if(_ActiveUnit.Count <= 0)
        {
            if(_BattleState.CurrentState != BattleState.BadEnd2)
            {
                _BattleState.StateChange(BattleState.BadEnd2);
            }
        }

        if(!_RezTimer.IsTicking)
        {
            _RezTimer.Start(3000);
        }

        _RezTimer.Update();
        _BattleState.DoUpdate();
    }

    public void Start(UnitActionBar actionBar)
    {
        _ActionBar = actionBar;
        _ActionBar.OnAbilitySelected += AbilitySelected;
        _BattleState.StateChange(BattleState.Tutorial);
    }

    private Timer _RezTimer = new Timer();
    public BattleSystem(GridUnit pc, eff.Grid grid, UnitFactory factory, TutorialScreen tutorialScreen, StatusBar statusBar)
    {
        _Player = pc;
        _Grid = grid;
        _Factory = factory;
        _StatusBar = statusBar;

        _TutorialScreen = tutorialScreen;
        _TutorialScreen.Confirm.onClick.AddListener(TutorialCompleted);

        _BattleState = new StateActionMap<BattleState>();
        _BattleState.RegisterEnter(BattleState.Init, OnEnter_Init);
        _BattleState.RegisterEnter(BattleState.Tutorial, OnEnter_Tutorial);
        _BattleState.RegisterEnter(BattleState.Start, OnEnter_Start);
        _BattleState.RegisterEnter(BattleState.ExcuteAction, OnEnter_ExcuteAction);
        _BattleState.RegisterEnter(BattleState.Resolve, OnEnter_Resolve);
        _BattleState.RegisterEnter(BattleState.Default, OnEnter_Default);
        _BattleState.RegisterEnter(BattleState.BadEnd, OnEnter_BadEnd);
        _BattleState.RegisterEnter(BattleState.BadEnd2, OnEnter_BadEnd2);

        _BattleState.RegisterUpdate(BattleState.Updating, OnUpdate_Update);

        _BattleState.StateChange(BattleState.Init);

        _RezTimer.OnTimerComplete += OnComplete_ResTimer;
    }

    private void OnComplete_ResTimer()
    {
        for (int i = 0; i < _ActiveUnit.Count; i++)
        {
            if (_ActiveUnit[i].AliveState == AliveState.Dead)
            {
                _ActiveUnit[i].Rez(_Grid.GetOpenTile());
            }
        }
    }

    private void TutorialCompleted()
    {
        _TutorialScreen.SetActive(false);
        _BattleState.StateChange(BattleState.Start);
    }

    private void OnEnter_Init()
    {
        int count = 0;

        while(count < _Grid.Units.Count)
        {
            var tile = _Grid.GetRandomTilePos();
            if (!tile.IsOccupied)
            {
                var enemy = _Grid.Units[count];
                _Factory.CreateUnit(enemy);
                RegisterUnitEvents(enemy);
                _Grid.SetOnGrid(enemy, tile);
                var ai = new UnitAI(enemy, _Player, _Grid);
                ai.OnQueueMove += QueueMove;
                ai.OnQueueAbility += QueueAction;
                _ActiveUnit.Add(ai);
                count++;
            }
        }

        _Grid.TileSelected += OnTileSelected;
        _ActiveUnit[0].IsBoss = true;        
        RegisterUnitEvents(_Player);
        _BattleState.StateChange(BattleState.Default);
    }

    private void UpdateStatusBar()
    {
        var souls = 0;
        var necro = 0;
        var skelly = 0;

        for (int i = 0; i < _ActiveUnit.Count; i++)
        {
            var unit = _ActiveUnit[i];
            if (unit.IsBoss)
            {
                necro++;
            }else if(unit.AliveState == AliveState.Soul)
            {
                souls++;
            }else if(unit.AliveState == AliveState.Default)
            {
                skelly++;
            }
        }

        _StatusBar.Necro.ShowText("{rdir} X"  + necro);
        _StatusBar.Souls.ShowText("{rdir} X"  + souls);
        _StatusBar.Skelly.ShowText("{rdir} X" + skelly);
    }

    private void OnTileSelected(GridTile tile)
    {
        if(_Player.CombatModel.BattleState == UnitBattleState.Ready)
        {
            if(!tile.IsOccupied && tile.IsAdjacent(_Player.CurrentPosition))
            {
                _Player.CombatModel.BattleState = UnitBattleState.ActionQueued;
                QueueMove(_Player, tile);
            }
        }
    }

    private void KillSoul(GridTile tile)
    {
        var soul = GetSoulOrNull(tile);
        if (soul != null)
        {
            _ActiveUnit.Remove(soul);
            soul.Kill();
        }
    }

    private UnitAI GetSoulOrNull(GridTile tile)
    {
        UnitAI soul = null;

        for (int i = 0; i < _ActiveUnit.Count; i++)
        {
            if (_ActiveUnit[i].IsInGridPosition(tile))
            {
                if (_ActiveUnit[i].AliveState == AliveState.Soul)
                {
                    soul = _ActiveUnit[i];
                    break;
                }
            }
        }

        return soul;
    }

    public void QueueMove(GridUnit unit, GridTile tile)
    {
        var args = new UnitActionArgs();
        args.Actor = unit;
        args.SelectedTile = tile;

        var moveAction = new UnitAction();
        moveAction.Args = args;
        moveAction.Action = _Player.Move.Execute;

        _ActionQueue.Enqueue(moveAction);
    }

    private void RegisterUnitEvents(GridUnit unit)
    {
        unit.OnQueueAction += QueueAction;
        unit.OnKilled += UnitKilled;
        unit.OnSelected += UnitSelected;
    }

    private void UnitSelected(GridUnit unit)
    {
        if(_Player.CombatModel.BattleState == UnitBattleState.SelectingAction)
        {
            if (unit.CurrentPosition.IsInRange(_Player.CurrentPosition, _CurrentAbility.Range))
            {
                _Player.CombatModel.BattleState = UnitBattleState.ActionQueued;
                _Player.ActionBar.Unselect();
                _Grid.Unselect();

                if(_CurrentAbility is Cancel)
                {
                    return;
                }

                var args = new UnitActionArgs();
                args.Actor = _Player;
                args.Target = unit;
                args.SelectedTile = args.Target.CurrentPosition;

                var action = new UnitAction();
                action.Action = _CurrentAbility.Execute;
                action.Args = args;

                QueueAction(action);

                Debug.Log(unit.CurrentPosition);
            }
        }else if(_Player.CombatModel.BattleState == UnitBattleState.Ready)
        {
            if (unit.AliveState == AliveState.Soul && unit.CurrentPosition.IsAdjacent(_Player.CurrentPosition))
            {
                KillSoul(unit.CurrentPosition);
            }
        }
    }

    private void UnitKilled(GridUnit unit)
    {
        var roll = _RNG.GetRandomInt(10);

        if(!unit.IsBoss)
        {
            if (roll < 4)
            {
                unit.DropSoul();
            }
            else
            {
                unit.Kill();
            }
        }
        else
        {
            if(_ActiveUnit.Count == 1)
            {
                unit.Kill(false);
                _ActiveUnit.Clear();
            }
            else
            {
                unit.CombatModel.CurrentHp = unit.CombatModel.HP_MAX;
                unit.UpdateValues();
            }
        }
    }

    private void QueueAction(UnitAction action)
    {
        _ActionQueue.Enqueue(action);
    }

    private void OnEnter_Default()
    {
        
    }

    private void OnEnter_BadEnd2()
    {
        if (_ActionBar != null)
        {
            _ActionBar.gameObject.SetActive(false);
        }

        if (_StatusBar != null)
        {
            _StatusBar.gameObject.SetActive(false);
        }

        _TutorialScreen.StartBadEnd2Transition();
    }

    private void OnEnter_BadEnd()
    {
        if (_ActionBar != null)
        {
            _ActionBar.gameObject.SetActive(false);
        }

        if (_StatusBar != null)
        {
            _StatusBar.gameObject.SetActive(false);
        }

        _TutorialScreen.StartBadEndTransition();
    }

    private void OnEnter_Tutorial()
    {
        _Grid.SetActive(true);
        _TutorialScreen.SetActive(true);
    }

    private void OnEnter_Start()
    {
        _TutorialScreen._BattleAudio.Play();
        _TutorialScreen._BattleAudio.DOFade(1f, 0.3f);

        for (int i = 0; i < _ActiveUnit.Count; i++)
        {
            _ActiveUnit[i].StartBattle();
        }

        _Player.gameObject.SetActive(true);
        _Player.CombatModel.BattleState = UnitBattleState.Waiting;

        UpdateStatusBar();
        _BattleState.StateChange(BattleState.Updating);
    }

    private void OnEnter_ExcuteAction()
    {
        var ua = _ActionQueue.Dequeue();
        ua.Action(ua.Args);

        ua.Args.Actor.CombatModel.CanReady = true;
        ua.Args.Actor.UpdateValues();

        if(ua.Args.Target != null)
        {
            ua.Args.Target.UpdateValues();
        }

        _BattleState.StateChange(BattleState.Resolve);
    }

    private void OnEnter_Resolve()
    {
        UpdateStatusBar();
        _BattleState.StateChange(BattleState.Updating);
    }
    
    private void OnUpdate_Update()
    {
        if (_ActionQueue.Count > 0)
        {
            _BattleState.StateChange(BattleState.ExcuteAction);
        }
        else
        {
            for (int i = 0; i < _ActiveUnit.Count; i++)
            {
                _ActiveUnit[i].Update();
            }

            if(_Player != null && _Player.AliveState == AliveState.Default)
            {
                _Player.DoUpdate();
            }
        }
    }

    private Ability _CurrentAbility;
    private void AbilitySelected(Ability ability)
    {
        Debug.Log(ability.Name);

        if(_Player.CombatModel.BattleState == UnitBattleState.Ready)
        {
            _CurrentAbility = ability;

            bool occupide = ability is Move ? false : true;
            var selectionTiles = _Grid.GetSelectableTiles(occupide, _Player.CurrentPosition, ability.Range);

            for (int i = 0; i < selectionTiles.Count; i++)
            {
                var sTile = selectionTiles[i];
                sTile.Select();
            }

            _Player.CombatModel.BattleState = UnitBattleState.SelectingAction;
        }
        else if(_Player.CombatModel.BattleState == UnitBattleState.SelectingAction)
        {
            _CurrentAbility = null;
            _Grid.Unselect();
            _Player.ActionBar.Unselect();
            _Player.CombatModel.BattleState = UnitBattleState.Ready;
        }
    }

    private UnitActionBar _ActionBar;
    private Queue<UnitAction> _ActionQueue = new Queue<UnitAction>();
    private List<UnitAI> _ActiveUnit = new List<UnitAI>();
    private GridUnit _Player;
    private StatusBar _StatusBar;
    private eff.Grid _Grid;
    private UnitFactory _Factory;
    private RNG _RNG = new RNG();

    private TutorialScreen _TutorialScreen;
}

public enum BattleState
{
    Default,
    Init,
    Tutorial,
    Start,
    Updating,
    ExcuteAction,
    Resolve,
    BadEnd,
    BadEnd2

}

public enum UnitBattleState
{
    Default,
    NotReady,
    Ready,
    Waiting,
    ActionQueued,
    SelectingAction
}

public enum UnitType
{
    Default,
    Skelly
}