using System.Collections;
using System.Collections.Generic;

public class CombatModel
{
    public int HPMax;
    public int CurrentHP;
    public float ATB;
    public float Speed;
    public UnitBattleState BattleState { get; set; }
}
