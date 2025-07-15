using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VExtension;

public class AttackState : IState
{
    private ActorBase actor;

    public AttackState(ActorBase actor) => this.actor = actor;
    public string StateName => "ATTACKING";

    private int frameCounter = 0;

    private float attackTimer = 0;

    public void OnEnter()
    {
        actor.ActorAnimationController.PlayAttackAudio();
        actor.ActorAnimationController.PlayAttackAnimation();
        attackTimer = actor.AttributesConfig.AttackRate;
        if (actor.AttributesConfig.ChooseBrutality()) attackTimer *= Random.Range(0.5f, 1f);

        actor.Attack(actor.Target);
    }

    public void OnUpdate()
    {
        if (frameCounter > 3)
        {
            if (!actor.IsTargetInAttackRange())
            {
                actor.ChangeState(StateType.Chase);
                return;
            }
            frameCounter = 0;
        }
        else frameCounter++;

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            RotateToTargetIgnoreHeight(actor.ActorTransform, actor.TargetTransform.position, actor.AttributesConfig.RotationSpeed);
        }
        else
        {
            attackTimer = actor.AttributesConfig.AttackRate;
            actor.ActorAnimationController.PlayAttackAudio();
            actor.ActorAnimationController.PlayAttackAnimation();
            actor.Attack(actor.Target);
        }
    }

    public void OnExit()
    {
    }
}
