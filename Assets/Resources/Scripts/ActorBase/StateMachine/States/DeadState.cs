using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    private ActorBase actor;

    public DeadState(ActorBase actor) => this.actor = actor;
    public string StateName => "DEAD";

    private float timer;

    public void OnEnter()
    {
        timer = 4f;
        actor.ActorAnimationController.PlayDeadAudio();
        actor.ActorAnimationController.PlayDeadAnimation();
    }

    public void OnUpdate()
    {
        if (timer > 0) timer -= Time.deltaTime;
        else
        {
            actor.gameObject.SetActive(false);
        }
    }

    public void OnExit()
    {

    }
}
