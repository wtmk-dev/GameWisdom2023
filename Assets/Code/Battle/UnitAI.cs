using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = WTMK.Mechanics;

public class UnitAI 
{
    public void Update()
    {
        _Unit.DoUpdate();
        if(_Unit.CombatModel.BattleState == UnitBattleState.Ready)
        {
            if(_PC.IsInRange(_Unit.CurrentPosition.GridPosition, 1))
            {
                if(_RNG.GetRandomInt(9) < 2)
                {
                    _CurrentAction.Action = _Actions.DefaultAction;
                    _CurrentActionArgs.Type = ActionType.Attack;

                    _Unit.QueueAction(_CurrentAction);
                    _Unit.CombatModel.BattleState = UnitBattleState.ActionQueued;
                }
            }
        }
    }

    public void StartBattle()
    {
        _Unit.CombatModel.BattleState = UnitBattleState.Waiting;
    }

    private UnitAction _CurrentAction = new UnitAction();
    private UnitActionArgs _CurrentActionArgs = new UnitActionArgs();

    private GridUnit _Unit;
    private GridUnit _PC;
    private M.Grid _Grid;
    private RNG _RNG;
    private cAction _Actions = cAction.Instance;
    public UnitAI(GridUnit unit, GridUnit pc, M.Grid grid)
    {
        _Unit = unit;
        _Grid = grid;
        _PC = pc;

        _RNG = new RNG();

        _CurrentAction.Args = _CurrentActionArgs;
        _CurrentActionArgs.Actor = _Unit;
        _CurrentActionArgs.Target = _PC;
    }
}
