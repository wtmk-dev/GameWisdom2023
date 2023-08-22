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

    public void Start()
    {
        _BattleState.StateChange(BattleState.Start);
    }

    public BattleSystem(GridUnit pc, eff.Grid grid, UnitFactory factory)
    {
        _Player = pc;
        _Grid = grid;
        _Factory = factory;
        _BattleState = new StateActionMap<BattleState>();
        _BattleState.RegisterEnter(BattleState.Init, OnEnter_Init);
        _BattleState.RegisterEnter(BattleState.Start, OnEnter_Start);
        _BattleState.RegisterEnter(BattleState.ExcuteAction, OnEnter_ExcuteAction);
        _BattleState.RegisterEnter(BattleState.Resolve, OnEnter_Resolve);

        _BattleState.RegisterEnter(BattleState.Default, OnEnter_Default);

        _BattleState.RegisterUpdate(BattleState.Updating, OnUpdate_Update); //lol

        _BattleState.StateChange(BattleState.Init);
    }

    private void OnEnter_Init()
    {
        while(_ActiveUnit.Count < 30)
        {
            var enemy = _Factory.CreateUnit();
            RegisterUnitEvents(enemy);
            var tile = _Grid.GetRandomTilePos();
            if (!tile.IsOccupied)
            {
                _Grid.SetOnGrid(enemy, tile);
                var ai = new UnitAI(enemy, _Player, _Grid);
                _ActiveUnit.Add(ai);
            }
        }

        RegisterUnitEvents(_Player);
        _BattleState.StateChange(BattleState.Default);
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
        Debug.Log("Default");
    }
    private void OnEnter_Start()
    {
        Debug.Log("Start");

        for (int i = 0; i < _ActiveUnit.Count; i++)
        {
            _ActiveUnit[i].StartBattle();
        }

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
        }
    }

    private Queue<UnitAction> _ActionQueue = new Queue<UnitAction>();
    private List<UnitAI> _ActiveUnit = new List<UnitAI>();
    private GridUnit _Player;
    private eff.Grid _Grid;
    private UnitFactory _Factory;
}

public enum BattleState
{
    Default,
    Init,
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
