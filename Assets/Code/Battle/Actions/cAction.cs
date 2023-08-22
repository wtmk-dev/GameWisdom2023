using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public sealed class cAction
{
    private static readonly cAction _instance = new cAction();
    public static cAction Instance
    {
        get
        {
            return _instance;
        }
    }
    private cAction()
    {

    }

    public void DefaultAction(UnitActionArgs args)
    {
        Debug.Log(args.Type);
    }
}
