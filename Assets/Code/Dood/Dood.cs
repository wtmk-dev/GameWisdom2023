using System.Collections.Generic;
using UnityEngine;

public sealed class Dood
{
    private static readonly Dood _Instance = new Dood();
   
    public static Dood Instance
    {
        get
        {
            return _Instance;
        }
    }

    public static bool IsLogging { get; set; }

    public void Log(object text)
    {
        if (!IsLogging)
        {
            return;
        }

        Debug.Log(text);
    }

    public void Error(object text)
    {
        if (!IsLogging)
        {
            return;
        }

        Debug.LogError(text);
    }

    public void Warning(object text)
    {
        if(!IsLogging)
        {
            return;
        }

        Debug.LogWarning(text);
    }
}