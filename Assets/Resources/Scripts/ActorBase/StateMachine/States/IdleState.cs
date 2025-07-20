using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private ActorBase actor;

    public IdleState(ActorBase actor) => this.actor = actor;

    private float timer = 0;
    private int detectTargetFrameCounter = 0;
    public string StateName => "IDLE";
    public void OnEnter()
    {
        timer = actor.AttributesConfig.GetIdleDuration();
        if (actor.AttributesConfig.ChooseLaziness()) timer *= (Random.Range(1f, 3f)) * actor.AttributesConfig.Laziness;

        actor.ActorAnimationController.PlayIdleAudio();
        actor.ActorAnimationController.PlayIdleAnimation();
    }

    public void OnUpdate()
    {
        if (detectTargetFrameCounter > 5 && actor.IsReady())
        {
            detectTargetFrameCounter = 0;
            if (actor.IsTargetInVisionRange() || actor.IsTargetInSurroundingSenseRange())
            {
                actor.ChangeState(StateType.Chase);
                return;
            }
        }
        else detectTargetFrameCounter++;

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else actor.ChangeState(StateType.Move);
    }

    public void OnExit()
    {

    }
}
