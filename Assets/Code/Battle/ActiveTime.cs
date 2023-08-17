using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTime
{
    public void Update()
    {
        if(_CombatModel.BattleState == UnitBattleState.Waiting && 
           _CombatModel.BattleState != UnitBattleState.Ready)
        {
            _CombatModel.WaitTime += Time.deltaTime;
        }

        if (_CombatModel.WaitTime >= _CombatModel.ActiveTime)
        {
            OnTimerComplete();
        }
    }

    public ActiveTime(CombatModel model)
    {
        _CombatModel = model;
        _CombatModel.ActiveTime = _CombatModel.MAX_WAIT - _CombatModel.SPEED_MAX;
    }

    private void OnTimerComplete()
    {
        Debug.Log("Done");
        _CombatModel.CanReady = true;
        _CombatModel.WaitTime = 0;
    }

    private CombatModel _CombatModel;
}