using System.Collections;
using System.Collections.Generic;

public class StateDirector : IStateDirector
{
    public State<string> CurrentState { get { return (State<string>)_StateMap[_CurrentState]; } }
    public bool IsActive { get; set; }

    public void OnUpdate()
    {
        if(!IsActive)
        {
            return;
        }

        if(_StateMap[_CurrentState].OnUpdate())
        {
            _PreviousState = _CurrentState;
            _CurrentState = _StateMap[_CurrentState].NextState;

            _StateMap[_PreviousState].OnExit();
            _StateMap[_CurrentState].OnEnter();
        }
    }

    public void SetActive(string screen, bool isActive)
    {
        _StateMap[screen].View.SetActive(isActive);
    }

    public void SetCurrentState(string state)
    {
        _PreviousState = _CurrentState;

        if (_CurrentState != null && _StateMap.ContainsKey(_PreviousState))
        {
            _StateMap[_PreviousState].OnExit();
        }

        _CurrentState = state;

        if (_CurrentState != null && _StateMap.ContainsKey(_CurrentState))
        {
            _StateMap[_CurrentState].OnEnter();
        }
    }

    private Dood _Debug = Dood.Instance;
    private Dictionary<string, IState<string>> _StateMap = new Dictionary<string, IState<string>>();

    private IState<string>[] _States;
    private string _CurrentState;
    private string _PreviousState;

    public StateDirector(IState<string>[] states)
    {
        _States = states;

        for (int i = 0; i < _States.Length; i++)
        {
            _StateMap.Add(_States[i].Tag, _States[i]);
        }

        HideAllScreens();
    }

    public StateDirector(State<string>[] states)
    {
        _States = states;

        for (int i = 0; i < _States.Length; i++)
        {
            _StateMap.Add(_States[i].Tag, _States[i]);
        }

        HideAllScreens();
    }

    private void HideAllScreens()
    {
        for (int i = 0; i < _States.Length; i++)
        {
            if(_States[i].View == null)
            {
                _Debug.Log($"{_States[i].Tag} View is null");
                return;
            }

            _States[i].View.SetActive(false);
        }
    }
}
