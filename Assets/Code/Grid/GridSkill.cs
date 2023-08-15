using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSkill : MonoBehaviour
{
    public virtual void Init()
    {
        
    }

    protected bool _IsActive;
    protected IEnumerator _Coroutine;
    protected float _PhaseChangeTime = 300f;
    protected Timer _PhaseChangeTimer = new Timer();

    protected IEnumerator DoMoveShotSlider(Slider slider)
    {
        slider.value = 0f;

        while (_IsActive)
        {
            while (slider.value < slider.maxValue)
            {
                slider.value += .1f;
                yield return new WaitForSeconds(0.1f);
            }

            while (slider.value > slider.minValue)
            {
                slider.value -= .1f;
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.01f);
        }

    }


    protected void StartPhaseChange()
    {
        _PhaseChangeTimer.Start(_PhaseChangeTime);
    }

}
