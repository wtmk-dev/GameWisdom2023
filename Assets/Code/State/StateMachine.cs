using System.Collections;
using System.Collections.Generic;

public class StateMachine<T> : IStateDirector
{
    public IState<T> CurrentState { get { return _StateMap[_CurrentState]; } }
    public bool IsActive { get; set; }

    public void OnUpdate()
    {
        if (!IsActive)
        {
            return;
        }

        if (_StateMap[_CurrentState].OnUpdate())
        {
            _PreviousState = _CurrentState;
            _CurrentState = _StateMap[_CurrentState].NextState;

            _StateMap[_PreviousState].OnExit();
            _StateMap[_CurrentState].OnEnter();
        }
    }

    public void SetActive(T screen, bool isActive)
    {
        _StateMap[screen].View.SetActive(isActive);
    }

    public void StateChange(T state)
    {
        if (_CurrentState != null && _StateMap.ContainsKey(_CurrentState))
        {
            _PreviousState = _CurrentState;
            _StateMap[_PreviousState].OnExit();
        }

        _CurrentState = state;
        _StateMap[_CurrentState].OnEnter();
    }

    private Dood _Debug = Dood.Instance;
    private Dictionary<T, IState<T>> _StateMap = new Dictionary<T, IState<T>>();

    private IState<T>[] _States;
    private T _CurrentState;
    private T _PreviousState;

    public StateMachine(IState<T>[] states)
    {
        _States = states;

        for (int i = 0; i < _States.Length; i++)
        {
            _StateMap.Add(_States[i].Tag, _States[i]);
        }

        HideAllScreens();
    }

    public StateMachine(State<T>[] states)
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
            if (_States[i].View == null)
            {
                _Debug.Log($"{_States[i].Tag} View is null");
                return;
            }

            _States[i].View.SetActive(false);
        }
    }
}
