using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateView : MonoBehaviour, IStateView
{
    public virtual void SetOverlayText(string text)
    {

    }

    public virtual void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
