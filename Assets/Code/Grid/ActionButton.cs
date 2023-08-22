using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ActionButton : MonoBehaviour
{
    [SerializeField]
    private Button _Action;
    [SerializeField]
    private TextMeshProUGUI _Text;

    public void Set(string name)
    {
        _Text.SetText(name);
    }
}