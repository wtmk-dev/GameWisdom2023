using System.Collections.Generic;

public interface IState<T> 
{
    T Tag { get; }
    T NextState { get; }
    IList<T> ValidTransitions { get; }
    IStateView View { get; }
    void OnEnter();
    void OnExit();
    bool OnUpdate();
}
