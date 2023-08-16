using System;
using System.Collections;
using System.Collections.Generic;

public class ActiveTime
{
    public void Update()
    {
        if(_Unit.BattleState == BattleState.NotReady)
        {
            _Timer.OnTimerComplete += OnTimerComplete;
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
}
