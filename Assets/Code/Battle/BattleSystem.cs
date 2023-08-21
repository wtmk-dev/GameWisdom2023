using System;
using System.Collections.Generic;
using UnityEngine;
using eff = WTMK.Mechanics;

public class BattleSystem
{
    private StateActionMap<BattleState> _BattleState;

    public void Update()
    {

    }

    public BattleSystem(GridUnit pc, eff.Grid grid, UnitFactory factory)
    {
        _Player = pc;
        _Grid = grid;
        _Factory = factory;
        _BattleState = new StateActionMap<BattleState>();
        _BattleState.RegisterEnter(BattleState.Init, OnEnter_Init);
        _BattleState.RegisterEnter(BattleState.Default, OnEnter_Default);

        _BattleState.StateChange(BattleState.Init);
    }

    private void OnEnter_Init()
    {
        while(_ActiveUnit.Count < 30)
        {
            var enemy = _Factory.CreateUnit();
            enemy.Skin();
            var tile = _Grid.GetRandomTilePos();
            if (!tile.IsOccupied)
            {
                _Grid.SetOnGrid(enemy, tile);
                var ai = new UnitAI(enemy, _Player, _Grid);
                _ActiveUnit.Add(ai);

                enemy.CombatModel.BattleState = UnitBattleState.Waiting;
            }
        }

        _BattleState.StateChange(BattleState.Default);
    }

    private void OnEnter_Default()
    {
        Debug.Log("Default");
    }
    private void OnEnter_Start()
    {

    }

    private void OnUpdate_Update()
    {
        if(_ActionQueue.Count > 0)
        {
            _BattleState.StateChange(BattleState.ExcuteAction);
        }
    }

    private void OnEnter_ExcuteAction()
    {
        var gridUnit = _ActionQueue.Dequeue();

    }

    private Queue<GridUnit> _ActionQueue = new Queue<GridUnit>();
    private List<UnitAI> _ActiveUnit = new List<UnitAI>();
    private GridUnit _Player;
    private eff.Grid _Grid;
    private GridUnit _Unit;
    private UnitFactory _Factory;

}

public enum BattleState
{
    Default,
    Init,
    Updating,
    ActionSelection,
    ActionQueued,
    ExcuteAction,
    CompleteAction
}

public enum UnitBattleState
{
    Default,
    NotReady,
    Ready,
    Waiting
}
