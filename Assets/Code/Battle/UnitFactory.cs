using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{

    [SerializeField]
    GridUnit _UnitPrefab;

    public GridUnit CreateUnit()
    {
        var clone = Instantiate<GridUnit>(_UnitPrefab);
        var speed = _RNG.GetRandomInt(5);
        var hp = _RNG.GetRandomInt(10);
        var combatModel = new CombatModel(hp, speed);
        clone.Skin(combatModel);
        return clone;
    }


    private RNG _RNG = new RNG();
}
