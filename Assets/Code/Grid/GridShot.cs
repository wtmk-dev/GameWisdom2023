using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridShot : GridSkill
{
    public event Action OnShotComplet;
    public float Score { get; private set; }

    public override void Init()
    {
        if (_Coroutine != null)
        {
            StopCoroutine(_Coroutine);
        }

        _Coroutine = DoMoveShotSlider(_ShotSlider);

        _IsActive = true;
        StartCoroutine(_Coroutine);
    }

    [SerializeField]
    private Slider _ShotSlider;

    void Update()
    {
        if(!_IsActive)
        {
            return;
        }

        if(Input.anyKey)
        {
            _IsActive = false;
            if (_Coroutine != null)
            {
                StopCoroutine(_Coroutine);
            }

            Score = _ShotSlider.value;

            OnShotComplet.Invoke();
        }
    }

    
}
