using System;
using System.Collections.Generic;
using UnityEngine;
using eff = WTMK.Mechanics;

public class BattleSystem
{
    private StateActionMap<BattleState> _BattleState;

    public void Update()
    {
        _BattleState.DoUpdate();
    }

    public void Start(UnitActionBar actionBar)
    {
        _ActionBar = actionBar;
        _ActionBar.OnAbilitySelected += AbilitySelected;
        _BattleState.StateChange(BattleState.Tutorial);
    }

    public BattleSystem(GridUnit pc, eff.Grid grid, UnitFactory factory, TutorialScreen tutorialScreen)
    {
        _Player = pc;
        _Grid = grid;
        _Factory = factory;

        _TutorialScreen = tutorialScreen;
        _TutorialScreen.Confirm.onClick.AddListener(TutorialCompleted);

        _BattleState = new StateActionMap<BattleState>();
        _BattleState.RegisterEnter(BattleState.Init, OnEnter_Init);
        _BattleState.RegisterEnter(BattleState.Tutorial, OnEnter_Tutorial);
        _BattleState.RegisterEnter(BattleState.Start, OnEnter_Start);
        _BattleState.RegisterEnter(BattleState.ExcuteAction, OnEnter_ExcuteAction);
        _BattleState.RegisterEnter(BattleState.Resolve, OnEnter_Resolve);
        _BattleState.RegisterEnter(BattleState.Default, OnEnter_Default);

        _BattleState.RegisterUpdate(BattleState.Updating, OnUpdate_Update);

        _BattleState.StateChange(BattleState.Init);    
    }

    private void TutorialCompleted()
    {
        _TutorialScreen.gameObject.SetActive(false);
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
                _ActiveUnit.Add(ai);
                count++;
            }
        }

        _Grid.TileSelected += OnTileSelected;
        _ActiveUnit[0].IsBoss = true;
        RegisterUnitEvents(_Player);
        _BattleState.StateChange(BattleState.Default);
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
    }

    private void QueueAction(UnitAction action)
    {
        _ActionQueue.Enqueue(action);
    }

    private void OnEnter_Default()
    {
        _Grid.SetActive(false);
    }

    private void OnEnter_Tutorial()
    {
        _Grid.SetActive(true);
        _TutorialScreen.gameObject.SetActive(true);
    }

    private void OnEnter_Start()
    {
        //_TutorialScreen.gameObject.SetActive(false);

        for (int i = 0; i < _ActiveUnit.Count; i++)
        {
            _ActiveUnit[i].StartBattle();
        }

        _Player.gameObject.SetActive(true);
        _Player.CombatModel.BattleState = UnitBattleState.Waiting;

        _BattleState.StateChange(BattleState.Updating);
    }

    private void OnEnter_ExcuteAction()
    {
        Debug.Log("OnEnter_ExcuteAction");

        var ua = _ActionQueue.Dequeue();
        ua.Action(ua.Args);

        ua.Args.Actor.CombatModel.CanReady = true;

        _BattleState.StateChange(BattleState.Resolve);
    }

    private void OnEnter_Resolve()
    {
        Debug.Log("OnEnter_Resolve");

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

            if(_Player != null)
            {
                _Player.DoUpdate();
            }
        }
    }

    private void AbilitySelected(Ability ability)
    {
        Debug.Log(ability.Name);
    }

    private UnitActionBar _ActionBar;
    private Queue<UnitAction> _ActionQueue = new Queue<UnitAction>();
    private List<UnitAI> _ActiveUnit = new List<UnitAI>();
    private GridUnit _Player;
    private eff.Grid _Grid;
    private UnitFactory _Factory;

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
    Resolve
}

public enum UnitBattleState
{
    Default,
    NotReady,
    Ready,
    Waiting,
    ActionQueued
}


public enum UnitType
{
    Default,
    Skelly
}