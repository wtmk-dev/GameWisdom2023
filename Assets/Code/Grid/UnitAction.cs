using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UnitAction
{
    public UnitActionArgs Args;
    public Action<UnitActionArgs> Action; 
}

public class UnitActionArgs
{
    public ActionType Type;
    public GridUnit Target;
    public GridUnit Actor;
    public GridTile SelectedTile;
}

