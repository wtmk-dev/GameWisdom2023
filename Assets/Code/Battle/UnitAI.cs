using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using M = WTMK.Mechanics;

public class UnitAI 
{
    public event Action<GridUnit, GridTile> OnQueueMove;
    public void Update()
    {
        _Unit.DoUpdate();

        if(!IsBoss)
        {
            if (_Unit.CombatModel.BattleState == UnitBattleState.Ready)
            {
                if (_PC.CurrentPosition.IsAdjacent(_Unit.CurrentPosition))
                {
                    /*
                    if (_RNG.GetRandomInt(9) < 2)
                    {
                        _CurrentAction.Action = _Actions.DefaultAction;
                        _CurrentActionArgs.Type = ActionType.Attack;

                        _Unit.QueueAction(_CurrentAction);
                        _Unit.CombatModel.BattleState = UnitBattleState.ActionQueued;
                    }
                    */

                    //attack
                }
                else
                {
                    if(_PC.CurrentPosition.CalculateDistance(_Unit.CurrentPosition) > 1)
                    {
                        var tiles = _Grid.GetAdjacentTiles(_Unit.CurrentPosition);
                        bool willMove = false;

                        for (int i = 0; i < tiles.Count; i++)
                        {
                            if(!tiles[i].IsOccupied && tiles[i].IsCloser(_PC.CurrentPosition, tiles[i], _Unit.CurrentPosition))
                            {
                                _Unit.CombatModel.BattleState = UnitBattleState.ActionQueued;
                                OnQueueMove?.Invoke(_Unit, tiles[i]);
                                willMove = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            //
        }
        
    }

    public void StartBattle()
    {
        _Unit.gameObject.SetActive(true);

        _Unit.SetBoss(IsBoss);

        _Unit.CombatModel.BattleState = UnitBattleState.Waiting;
    }

    private UnitAction _CurrentAction = new UnitAction();
    private UnitActionArgs _CurrentActionArgs = new UnitActionArgs();

    private GridUnit _Unit;
    private GridUnit _PC;
    private M.Grid _Grid;
    private RNG _RNG;
    private cAction _Actions = cAction.Instance;

    public bool IsBoss = false;

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
