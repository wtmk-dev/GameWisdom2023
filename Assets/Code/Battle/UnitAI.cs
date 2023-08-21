using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = WTMK.Mechanics;

public class UnitAI 
{
    public void Update()
    {
        if(_Unit.CombatModel.BattleState == UnitBattleState.Ready)
        {
            if(_PC.CurrentPosition.IsInRange())
            {

            }
        }
    }

    private GridUnit _Unit;
    private GridUnit _PC;
    private M.Grid _Grid;
    public UnitAI(GridUnit unit, GridUnit pc, M.Grid grid)
    {
        _Unit = unit;
        _Grid = grid;
        _PC = pc;
    }
}
