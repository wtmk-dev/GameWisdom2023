using System.Collections.Generic;
public class StateActionMap<T> 
{
    public T CurrentState { get => _CurrentState; }

    public delegate void Enter();
    public delegate void Update();
    public delegate void Exit();

    public Dictionary<T, Enter> StateEnter { get { return _StateEnter; } }
    public Dictionary<T, Update> StateUpdate { get { return _StateUpdate; } }
    public Dictionary<T, Exit> StateExit { get { return _StateExit; } }

    public bool Contains(T type)
    {
        if(_StateEnter.ContainsKey(type))
        {
            return true;
        }

        if (_StateUpdate.ContainsKey(type))
        {
            return true;
        }

        if (_StateExit.ContainsKey(type))
        {
            return true;
        }

        return false;
    }

    public void DoUpdate()
    {
        if(_StateUpdate.ContainsKey(_CurrentState))
        {
            _StateUpdate[_CurrentState]();
        }
    }

    public void Clear()
    {
        _StateEnter.Clear();
        _StateUpdate.Clear();
    }

    public void RegisterEnter(T state, Enter onEnter)
    {
        _StateEnter.Add(state, onEnter);
    }

    public void RegisterUpdate(T state, Update onUpdate)
    {
        _StateUpdate.Add(state, onUpdate);
    }

    public void RegisterExit(T state, Exit onExit)
    {
        _StateExit.Add(state, onExit);
    }

    public void StateChange(T state)
    {
        if(_StateExit.ContainsKey(state))
        {
            _StateExit[_CurrentState]();
        }
        
        _CurrentState = state;

        if(_StateEnter.ContainsKey(state))
        {
            _StateEnter[_CurrentState]();
        }
    }

    private Dictionary<T, Enter> _StateEnter = new Dictionary<T, Enter>();
    private Dictionary<T, Update> _StateUpdate = new Dictionary<T, Update>();
    private Dictionary<T, Exit> _StateExit = new Dictionary<T, Exit>();
    private T _CurrentState;

    public StateActionMap() { }
    public StateActionMap(T defaultState)
    {
        _CurrentState = defaultState;
    }
}