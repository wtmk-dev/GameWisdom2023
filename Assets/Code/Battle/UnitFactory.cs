using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    public GridUnit CreateUnit()
    {
        var clone = Instantiate<GridUnit>(_UnitPrefab);
        clone.Skin();
        return clone;
    }

    [SerializeField]
    GridUnit _UnitPrefab;

    public UnitFactory(GridUnit unitPrefab)
    {
        _UnitPrefab = unitPrefab;
    }
}
