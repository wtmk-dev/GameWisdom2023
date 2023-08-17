using System.Collections;
using System.Collections.Generic;

public class CombatModel
{
    public readonly float MAX_WAIT = 6f;

    public bool CanReady { get; set; }
    public float WaitTime { get; set; }
    public float ActiveTime { get; set; }
    public float Speed  { get; set; }
    public float ATB { get; set; }
    public float SPEED_MAX => _SPEED;

    public int HP_MAX => _HP;
    
    public UnitBattleState BattleState { get; set; }

    public CombatModel(int hp, float speed)
    {
        _HP = hp;
        _SPEED = speed;
    }

    private int _HP;
    private float _SPEED;
}
