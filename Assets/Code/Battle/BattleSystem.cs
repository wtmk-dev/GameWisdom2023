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

        _BattleState.StateChange(BattleState.Init);
    }

    private void OnEnter_Init()
    {
        Debug.Log("int");

        while(_ActiveUnit.Count < 30)
        {
            var enemy = _Factory.CreateUnit();
            enemy.Skin();
            var tile = _Grid.GetRandomTilePos();
            if (!tile.IsOccupied)
            {
                _Grid.SetOnGrid(enemy, tile);
                _ActiveUnit.Add(enemy);
            }
        }
    }

    private List<GridUnit> _ActiveUnit = new List<GridUnit>();
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
    CompleteAction,
}

public enum UnitBattleState
{
    Default,
    NotReady,
    Ready,
    Waiting
}
