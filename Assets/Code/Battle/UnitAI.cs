using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using M = WTMK.Mechanics;

public class UnitAI 
{
    public event Action<GridUnit, GridTile> OnQueueMove;
    public event Action<UnitAction> OnQueueAbility;

    public bool IsInGridPosition(GridTile tile)
    {
        return _Unit.CurrentPosition.GridPosition.x == tile.GridPosition.x &&
           _Unit.CurrentPosition.GridPosition.y == tile.GridPosition.y;
    }

    public void Update()
    {
        if(!_Unit.gameObject.activeInHierarchy)
        {
            return;
        }

        //Debug.Log(_Unit.AliveState);
        _Unit.DoUpdate();

        if (_Unit.AliveState != AliveState.Default)
        {
            return;
        }

        if (!IsBoss)
        {
            if (_PC.AliveState != AliveState.Default)
            {
                return;
            }

            if (_Unit.CombatModel.BattleState == UnitBattleState.Ready)
            {
                if (_PC.CurrentPosition.IsAdjacent(_Unit.CurrentPosition))
                {
                    QueueAttack();
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

                        if(!willMove)
                        {
                            QueuePowerUp();
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

    public void Kill()
    {
        _Unit.gameObject.SetActive(false);
        _Unit.CurrentPosition.IsOccupied = false;
    }

    public void Rez(GridTile pos)
    {
        pos.IsOccupied = true;
        _Unit.DoMove(pos, _Unit.Rez);    
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
    private Attack _Attack;
    private PowerUp _PowerUp;

    public bool IsBoss = false;

    public AliveState AliveState => _Unit.AliveState;

    public void SetAliveState(AliveState alive)
    {
        _Unit.AliveState = alive;
    }

    public UnitAI(GridUnit unit, GridUnit pc, M.Grid grid)
    {
        _Unit = unit;
        _Grid = grid;
        _PC = pc;

        _RNG = new RNG();

        _CurrentAction.Args = _CurrentActionArgs;
        _CurrentActionArgs.Actor = _Unit;
        _CurrentActionArgs.Target = _PC;

        _Attack = new Attack("Attack", 1, 1);
        _PowerUp = new PowerUp("PoweUp", 0, 1);
    }

    private void QueueAttack()
    {
        _Unit.CombatModel.BattleState = UnitBattleState.ActionQueued;
        var args = new UnitActionArgs();
        args.Actor = _Unit;
        args.Target = _PC;
        args.SelectedTile = _PC.CurrentPosition;

        var attack = new UnitAction();
        attack.Args = args;
        _Attack.SetDmg(_Unit.CombatModel.Attack);
        attack.Action = _Attack.Execute;
        OnQueueAbility?.Invoke(attack);
    }

    private void QueuePowerUp()
    {
        _Unit.CombatModel.BattleState = UnitBattleState.ActionQueued;
        var args = new UnitActionArgs();
        args.Actor = _Unit;
        args.SelectedTile = _Unit.CurrentPosition;

        var attack = new UnitAction();
        attack.Args = args;
        attack.Action = _PowerUp.Execute;
        OnQueueAbility?.Invoke(attack);
    }
}
