using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    public void CreateUnit(GridUnit clone)
    {
        var speed = _RNG.GetRandomInt(3);
        var hp = _RNG.GetRandomInt(12);
        var atk = _RNG.GetRandomInt(6);
        
        if(hp < 1)
        {
            hp = 1;
        }

        if(speed < 1)
        {
            speed = 1;
        }

        if(atk < 1)
        {
            atk = 1;
        }

        var combatModel = new CombatModel(hp, speed, atk, 0);
        clone.Skin(combatModel);
    }

    private RNG _RNG = new RNG();
}
