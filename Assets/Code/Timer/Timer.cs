using System;
using System.Collections;
using System.Collections.Generic;

public class Timer
{
    public virtual event Action OnTimerComplete;
    public virtual event Action<float> OnTimerTick; //_TimeRemaning

    public virtual bool IsTicking { get; private set; }
    public virtual float RunTime { get; private set; }

    public virtual void Update()
    {
        if(!IsTicking)
        {
            return;
        }

        if(_Timer.ElapsedMilliseconds >= _TimerLength)
        {
            Stop();
            OnTimerComplete?.Invoke();
        }
    }

    public virtual void Start()
    {
        IsTicking = true;
        _Timer.Reset();
        _Timer.Start();
    }

    public virtual void Start(float lenghtOfTimeInMilli)
    {
        _TimerLength = lenghtOfTimeInMilli;
        IsTicking = true;
        _Timer.Reset();
        _Timer.Start();
    }

    public virtual void Stop()
    {
        IsTicking = false;
        _Timer.Stop();
        RunTime = _Timer.ElapsedMilliseconds;
    }

    protected System.Diagnostics.Stopwatch _Timer;
    protected float _TimerLength;

    public Timer(float lenghtOfTimeInMilli = 0)
    {
        _Timer = new System.Diagnostics.Stopwatch();
        _TimerLength = lenghtOfTimeInMilli;
    }

}
