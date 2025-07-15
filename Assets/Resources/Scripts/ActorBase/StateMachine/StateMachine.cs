using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    #region PROPERTIES
    public IState CurrentState;
    public StateType CurrentStateType;
    public StateType PreviousStateType;
    #endregion

    #region MAIN

    public void ChangeState(IState newState)
    {
        if (CurrentState == newState) return;

        CurrentState?.OnExit();
        CurrentState = newState;
        CurrentState?.OnEnter();
    }

    public void Update()
    {
        CurrentState?.OnUpdate();
    }
    #endregion

    #region SUPPORTIVE
    public bool IsInState(StateType stateType) => CurrentStateType == stateType;
    #endregion
}
