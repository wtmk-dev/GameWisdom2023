using UnityEngine;
using eff = WTMK.Mechanics;

public class BattleSystem
{
    private StateActionMap<BattleState> _BattleState;

    public void Update()
    {

    }

    public BattleSystem(eff.Grid grid, GridUnit unit, UnitFactory factory)
    {
        _BattleState = new StateActionMap<BattleState>();
        _BattleState.RegisterEnter(BattleState.Init,OnEnter_Init);
    }

    private void OnEnter_Init()
    {
        Debug.Log("int");        
    }

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
