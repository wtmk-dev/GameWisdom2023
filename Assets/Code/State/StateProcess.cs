using System.Collections.Generic;
public class StateProcess
{
    public delegate void Enter();
    public delegate void Update();
    public delegate void Exit();

    public Dictionary<string, Enter> StateEnter { get { return _StateEnter; } }
    public Dictionary<string, Update> StateUpdate { get { return _StateUpdate; } }
    public Dictionary<string, Exit> StateExit { get { return _StateExit; } }

    public void Clear()
    {
        _StateEnter.Clear();
        _StateUpdate.Clear();
    }

    public void RegisterEnter(string state, Enter onEnter)
    {
        _StateEnter.Add(state, onEnter);
    }

    public void RegisterUpdate(string state, Update onUpdate)
    {
        _StateUpdate.Add(state, onUpdate);
    }

    public void RegisterExit(string state, Exit onExit)
    {
        _StateExit.Add(state, onExit);
    }

    private Dictionary<string, Enter> _StateEnter = new Dictionary<string, Enter>();
    private Dictionary<string, Update> _StateUpdate = new Dictionary<string, Update>();
    private Dictionary<string, Exit> _StateExit = new Dictionary<string, Exit>();
}