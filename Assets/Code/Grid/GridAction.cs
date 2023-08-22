using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Febucci.UI;

public class GridAction : MonoBehaviour
{
    public event Action<GridAction> OnActionComplete;
    public Button Button;
    public TextAnimatorPlayer Label;

    public ActionType Type
    {
        get => _Type;
        set 
        {
            _Type = value;
            Label.ShowText($"{_Type}");
        }
    }

    private ActionType _Type;

    void Awake()
    {
        Button.onClick.AddListener(() => OnActionComplete?.Invoke(this));
    }
}

public enum ActionType
{
    Default,
    Move,
    Attack,
    Ability
}
