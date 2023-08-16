using System;
using System.Collections;
using System.Collections.Generic;

public class ActiveTime
{
    public void Update()
    {
        if(_Unit.CombatModel.BattleState == UnitBattleState.NotReady)
        {
            _Timer.OnTimerComplete += OnTimerComplete;
        }

        if(_Unit.CombatModel.BattleState == UnitBattleState.Waiting)
        {
            if(!_Timer.IsTicking)
            {
                var activateTime = MAX_WAIT - _Unit.CombatModel.Speed;
                _Timer.Start(activateTime);
            }
        }
    }

    public ActiveTime(GridUnit unit)
    {
        _Unit = unit;
    }

    private void OnTimerComplete()
    {
        _Unit.CanReady = true;
    }

    private GridUnit _Unit;
    private Timer _Timer = new Timer();
    private float MAX_WAIT = 3000;
}
