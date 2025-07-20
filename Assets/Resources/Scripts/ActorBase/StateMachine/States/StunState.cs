using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VExtension;

public class StunState : IState
{
    private ActorBase actor;

    public StunState(ActorBase actor) => this.actor = actor;
    public string StateName => "STUNNED";

    private float timer = 0;

    public void OnEnter()
    {
        actor.ActorAnimationController.PlayHurtAudio();
        actor.ActorAnimationController.PlayStunAnimation();
        timer = actor.AttributesConfig.StunDuration;
        if (actor.stateMachine.PreviousStateType == StateType.Chase) timer /= 1.5f;
    }

    public void OnUpdate()
    {
        if (!actor.IsAlive) return;
        if (timer > 0) timer -= Time.deltaTime;
        else
        {
            if (actor.AttributesConfig.ChooseCourage())
            {
                if (actor.stateMachine.PreviousStateType == StateType.Stun) actor.ChangeState(StateType.Idle);
                else actor.ChangeState(actor.stateMachine.PreviousStateType);
            }
            else
            {
                actor.ChangeState(StateType.RunAway);
            }
        }
    }

    public void OnExit()
    {
    }
}
