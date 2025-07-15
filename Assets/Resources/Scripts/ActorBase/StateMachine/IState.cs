using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    string StateName { get; }
    void OnEnter();
    void OnUpdate();
    void OnExit();
}

public enum StateType
{
    Idle,
    Move,
    Attack,
    Chase,
    Dead,
    Stun,
    AggressiveChase,
    RunAway
}
