using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    public void CreateUnit(GridUnit clone)
    {
        var speed = _RNG.GetRandomInt(5);
        var hp = _RNG.GetRandomInt(10);
        var combatModel = new CombatModel(hp, speed, 0, 0);
        clone.Skin(combatModel);
    }

    private RNG _RNG = new RNG();
}
