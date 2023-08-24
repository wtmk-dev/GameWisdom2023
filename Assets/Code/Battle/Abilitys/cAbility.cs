using System;

public enum AbilityType
{
    BasicAttack,
    Sword,
    Dagger,
    Staff,
    Potion,
    FireBall,
    Teleport,
    TrickAttack,
    Block,
    WhirlWind,
    Hex,
    Shadows,
    Soulshatter,
    RuneOfPower,
    RuneOfCourage,
    RuneOfWisdom
}

public enum PassiveType
{
    Noble,
    Orphan,
    Disciplined,
}

public enum OnHitType
{
    Dodge,
}

public class AbilityFactory
{
    public Ability GetAbility(AbilityType type)
    {
        switch(type)
        {
            case AbilityType.BasicAttack:
                return new Ability("Attack", 1, 1);

            case AbilityType.Sword:
                return new Ability("Sword", 1, 5);

            case AbilityType.Dagger:
                return new Ability("Dagger", 1, 5);

            case AbilityType.Staff:
                return new Ability("Staff", 2, 5);

            case AbilityType.Potion:
                return new Ability("Red Potion", 1, -5);

            case AbilityType.FireBall:
                return new Ability("FireBall", 0, 0);

            case AbilityType.Teleport:
                return new Ability("Teleport", 0, 0);

            case AbilityType.TrickAttack:
                return new Ability("Trick Attack", 0, 0);

            case AbilityType.Block:
                return new Ability("Block", 0, 0);

            case AbilityType.WhirlWind:
                return new Ability("Whirl Wind", 0, 0);

            case AbilityType.Hex:
                return new Ability("Tomb of Hex", 0, 0);

            case AbilityType.Shadows:
                return new Ability("Tomb of Shadows", 0, 0);

            case AbilityType.Soulshatter:
                return new Ability("Tomb of Soulshatter", 0, 0);

            case AbilityType.RuneOfPower:
                return new Ability("Rune Of Power", 0, 0);

            case AbilityType.RuneOfCourage:
                return new Ability("Rune Of Courage", 0, 0);

            case AbilityType.RuneOfWisdom:
                return new Ability("Rune Of Wisdom", 0, 0);

            default:
                return new Ability("Attack", 1, 1); 
        }
    }

    public Passive GetPassive(PassiveType type)
    {
        switch (type)
        {
            case PassiveType.Disciplined:
                var p = new CombatModel(0, 0, 1, 1);
                return new Passive(p);
            case PassiveType.Noble:
                var n = new CombatModel(0, 0, 1, 0);
                return new Passive(n);
            case PassiveType.Orphan:
                var o = new CombatModel(1, 1, 0, 0);
                return new Passive(o);
            default:
                return new Passive(null);
        }
    }

    public OnHit GetOnHot(OnHitType type)
    {
        switch(type)
        {
            case OnHitType.Dodge:
                return new OnHit();
            default:
                return new OnHit();
        }
    }
}

public class OnHit
{

}

public class Passive
{
    public CombatModel Model => _Model;
    public Passive(CombatModel model)
    {
        _Model = model;
    }
    private CombatModel _Model;
}

public class Ability
{
    public string Name => _Name;
    public int Attack => _Damage;
    public int Range => _Range;

    public virtual void Execute(GridUnit caster, GridUnit target)
    {

    }

    public Ability(string name, int range, int damage)
    {
        _Name = name;
        _Range = range;
        _Damage = damage;
    }

    protected int _Range;
    protected int _Damage;
    protected string _Name;
}


public class KillSkellyRestorHp : Ability
{
    public KillSkellyRestorHp(string name, int range, int attack) : base(name,range,attack)
    {

    }

    public override void Execute(GridUnit caster, GridUnit target)
    {
        if (target.CombatModel.UnitType == UnitType.Skelly)
        {
            var heal = target.CombatModel.CurrentHp;
            caster.Heal(heal);

            target.CombatModel.CurrentHp = 0;
        }
    }
}

public class VengeanceResilence : ActivateWhenReady
{

    public void Apply(GridUnit unit)
    {
        //unit.CombatModel.Attack += 1;
        int heal = (int)Math.Floor(unit.CombatModel.HP_MAX * .2);
        unit.Heal(heal);
    }
}




public interface ActivateWhenReady
{
    public void Apply(GridUnit unit);
}

