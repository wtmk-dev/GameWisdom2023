using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionBar : MonoBehaviour
{
    public event Action<Ability> OnAbilitySelected;

    [SerializeField]
    private GameObject Action_Prefab;

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
    }

    public void SetActive(bool isActive)
    {
        for (int i = 0; i < _Actions.Count; i++)
        {
            _Actions[i].gameObject.SetActive(true);
        }
    }

    public void ActionSelected(Ability ability)
    {
        OnAbilitySelected?.Invoke(ability);
    }

    private List<ActionBarButton> _Actions = new List<ActionBarButton>();
}
