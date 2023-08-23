using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;
using DG.Tweening;

public class SpawnableWord : MonoBehaviour
{
    public List<string> PossibleWords;

    public void Init()
    {
        gameObject.SetActive(true);

        if (_TextAnimator == null)
        {
            _TextAnimator = GetComponent<TextAnimatorPlayer>();
            _Timer.OnTimerComplete += OnTimerComplete;
        }

        _TextAnimator.ShowText("");
    }    

    void Update()
    {
        if(!_Timer.IsTicking)
        {
            var willActive = _RNG.GetRandomInt(9);

            if (willActive < 3)
            {
                StartTimer();
            }
        }

        _Timer.Update();
    }

    private void OnTimerComplete()
    {
        var willActive = _RNG.GetRandomInt(9);
        if(willActive < 3)
        {
            SetEffect();
        }
        else
        {
            _TextAnimator.ShowText("");
        }

        var delay = _RNG.GetRandomInt(3600);
        _Timer.Start(delay);
    }

    private void SetEffect()
    {
        var willActive = _RNG.GetRandomInt(9);

        if(willActive < 3)
        {
            
            var roll = _RNG.GetRandomInt(PossibleWords.Count);
            var str = PossibleWords[roll];

            _TextAnimator.ShowText("{rdir}<shake>" + str + "</shake>{/rdir}");
        }
        else
        {
            _TextAnimator.ShowText("");
        }
    }

    private void StartTimer()
    {
        var delay = _RNG.GetRandomInt(3600);
        _Timer.Start(delay);
    }

    private Timer _Timer = new Timer();
    private RNG _RNG = new RNG();
    private TextAnimatorPlayer _TextAnimator;
}
