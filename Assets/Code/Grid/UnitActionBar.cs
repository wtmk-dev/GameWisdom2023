using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionBar : MonoBehaviour
{
    public event Action<Ability> OnAbilitySelected;

    [SerializeField]
    private GameObject Action_Prefab;

    public void SelectingAction()
    {
        SetActive(false);
        _Cancel.gameObject.SetActive(true);
    }

    public void Unselect()
    {
        SetActive(true);
        _Cancel.gameObject.SetActive(false);
    }

    public void Init(List<Ability> actions)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            var clone = Instantiate<GameObject>(Action_Prefab);
            var ua = clone.GetComponent<ActionBarButton>();
            ua.Init(actions[i]);
            _Actions.Add(ua);

            clone.transform.SetParent(transform);

            ua.OnActionSelected += ActionSelected;

            clone.gameObject.SetActive(false);
        }

        var clone1 = Instantiate<GameObject>(Action_Prefab);
        var ua1 = clone1.GetComponent<ActionBarButton>();
        ua1.Init(new Cancel("Cancel", 0, 0));
        ua1.OnActionSelected += ActionSelected;
        _Cancel = ua1;
        _Cancel.transform.SetParent(transform);
        _Cancel.gameObject.SetActive(false);
    }

    public void SetActive(bool isActive)
    {
        for (int i = 0; i < _Actions.Count; i++)
        {
            _Actions[i].gameObject.SetActive(isActive);
        }
    }

    public void ActionSelected(Ability ability)
    {
        OnAbilitySelected?.Invoke(ability);
    }

    private List<ActionBarButton> _Actions = new List<ActionBarButton>();
    private ActionBarButton _Cancel;
}
