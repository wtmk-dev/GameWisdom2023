using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionBarButton : MonoBehaviour
{
    public event Action<Ability> OnActionSelected;

    [SerializeField]
    private Button _Button;
    [SerializeField]
    private TextMeshProUGUI _Label;
    
    public void Init(Ability ab)
    {
        _Label.text = ab.Name;
        _Ability = ab;

        _Button.onClick.AddListener(() =>
        {
            OnActionSelected?.Invoke(_Ability);
        });
    }

    private Ability _Ability;
}
