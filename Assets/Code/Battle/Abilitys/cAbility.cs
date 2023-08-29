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

public enum CounterType
{
    Dodge,
}

public enum ActivateWhenReadyType
{
    Vengance
}

public class AbilityFactory
{
    public Ability GetAbility(AbilityType type)
    {
        switch (type)
        {
            case AbilityType.BasicAttack:
                return new Attack("Attack", 1, 1);

            case AbilityType.Sword:
                return new Attack("Sword", 1, 5);

            case AbilityType.Dagger:
                return new Attack("Daggers", 1, 3);

            case AbilityType.Staff:
                return new Attack("Staff", 2, 2);

            case AbilityType.Potion:
                var potion = new Heal("Red Potion", 0, 25);
                return potion;

            case AbilityType.FireBall:
                return new Attack("FireBall", 6, 25);

            case AbilityType.Teleport:
                return new Heal("Cure", 0, 10);

            case AbilityType.TrickAttack:
                return new Ability("Trick Attack", 5, 5);

            case AbilityType.Block:
                return new Heal("Block", 0, 5);

            case AbilityType.WhirlWind:
                return new Attack("Whirl Wind", 8, 5);

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

    public Counter GetCounter(CounterType type)
    {
        switch (type)
        {
            case CounterType.Dodge:
                return new Counter();
            default:
                return new Counter();
        }
    }


    public ActivateWhenReady GetActivateWhenReady(ActivateWhenReadyType type)
    {
        switch (type)
        {
            case ActivateWhenReadyType.Vengance:
                return new ActivateWhenReady();
            default:
                return new ActivateWhenReady();
        }
    }
}

public class Counter
{

}

public class ActivateWhenReady
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

    public virtual void Execute(UnitActionArgs args)
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

public class Attack : Ability
{
    public void SetDmg(int value)
    {
        _Damage = value;
    }

    public Attack(string name, int range, int dmg) : base(name,range,dmg)
    {

    }

    public override void Execute(UnitActionArgs args)
    {
        if(args.SelectedTile.GridPosition.x == args.Target.CurrentPosition.GridPosition.x  &&
           args.SelectedTile.GridPosition.y == args.Target.CurrentPosition.GridPosition.y)
        {
            args.Actor.DoAttack(args.Target, _Damage + args.Actor.CombatModel.Attack);
        }
    }
}

public class Heal : Ability
{
    public void SetDmg(int value)
    {
        _Damage = value;
    }

    public Heal(string name, int range, int dmg) : base(name, range, dmg)
    {

    }

    public override void Execute(UnitActionArgs args)
    {
        if (args.SelectedTile.GridPosition.x == args.Target.CurrentPosition.GridPosition.x &&
           args.SelectedTile.GridPosition.y == args.Target.CurrentPosition.GridPosition.y)
        {
            args.Actor.DoHeal(args.Target, _Damage);
        }
    }
}

public class Teleport : Ability
{
    public Teleport(string name, int range, int dmg) : base(name, range, dmg)
    {

    }

    public override void Execute(UnitActionArgs args)
    {
        if (!args.SelectedTile.IsOccupied)
        {
            args.Actor.DoMove(args.SelectedTile);
        }
    }
}

public class Move : Ability
{
    public Move(string name, int range, int dmg) : base(name, range, dmg)
    {

    }

    public override void Execute(UnitActionArgs args)
    {
        if(!args.SelectedTile.IsOccupied)
        {
            args.Actor.DoMove(args.SelectedTile);
        }
    }
}

public class PowerUp : Ability
{
    public PowerUp(string name, int range, int dmg) : base(name, range, dmg)
    {

    }

    public override void Execute(UnitActionArgs args)
    {
        args.Actor.CombatModel.IncreaseAttack(1);
    }
}

public class Cancel : Ability 
{
    public Cancel(string name, int range, int dmg) : base(name, range, dmg)
    {

    }

}

