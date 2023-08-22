using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTime
{
    public void Update()
    {
        UpdateWaitTime();
    }

    public ActiveTime(CombatModel model)
    {
        _CombatModel = model;
        _CombatModel.ActiveTime = _CombatModel.MAX_WAIT - _CombatModel.SPEED_MAX;
    }

    private void UpdateWaitTime()
    {
        if (_CombatModel.BattleState == UnitBattleState.Waiting &&
           _CombatModel.BattleState != UnitBattleState.Ready)
        {
            _CombatModel.WaitTime += Time.deltaTime;
            if (_CombatModel.WaitTime >= _CombatModel.ActiveTime)
            {
                OnTimerComplete();
            }
        }
    }

    private void OnTimerComplete()
    {
        _CombatModel.WaitTime = 0;
        _CombatModel.CanReady = true;
    }

    private CombatModel _CombatModel;
}