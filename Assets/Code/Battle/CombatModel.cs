using System.Collections;
using System.Collections.Generic;

public class CombatModel
{
    public readonly float MAX_WAIT = 6f;

    public bool CanReady { get; set; }

    public int Defense => _Defense;
    public int CurrentHp { get; set; }
    public int Attack => _Attack;

    public float WaitTime { get; set; }
    public float ActiveTime { get; set; }
    public float Speed  { get; set; }
    public float ATB { get; set; }
    public float SPEED_MAX => _SPEED;
    
    public UnitBattleState BattleState { get; set; }
    public UnitType UnitType { get; set; }

    public int HP_MAX => _HP_MAX;

    public void IncreaseAttack(int value)
    {
        _Attack += value;
    }

    public CombatModel(int hp, float speed, int attack, int defense)
    {
        _HP_MAX = hp;
        _SPEED = speed;
        _Attack = attack;
        _Defense = defense;

        CurrentHp = _HP_MAX;
    }

    private int _HP_MAX, _Attack, _Defense;
    private float _SPEED;

}