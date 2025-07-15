using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static VExtension;

public class AggressiveChaseState : IState
{
    private ActorBase actor;

    public AggressiveChaseState(ActorBase actor) => this.actor = actor;
    public string StateName => "AGGRESSIVE CHASING";

    private NavMeshAgent navMeshAgent;
    private bool reachedDestination = false;
    private int frameCounter = 0;

    private Vector3 currentDestination;
    private Vector3 temporaryDestination;
    private float currentRotateSpeed;
    private bool lookingAtTemporaryDestination = false;

    private float checkDistance = 1.5f;

    public void OnEnter()
    {
        currentRotateSpeed = 0;
        reachedDestination = false;

        checkDistance = actor.AttributesConfig.AttackRange;

        navMeshAgent = actor.GetActorNavMeshAgent();
        actor.ActorAnimationController.PlayChaseAudio();
        actor.ActorAnimationController.PlayFastChaseAnimation();
    }

    public void OnUpdate()
    {
        if (navMeshAgent == null) return;
        if (!reachedDestination)
        {
            currentDestination = actor.TargetTransform.position;

            if (frameCounter == 0 || frameCounter.DivisibleFor(5))
            {
                navMeshAgent.SetDestination(currentDestination);


                if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    if (navMeshAgent.path.corners.Length > 1)
                    {
                        temporaryDestination = navMeshAgent.path.corners[1];
                    }
                }

                lookingAtTemporaryDestination = IsLookAtIgnoreY(actor.ActorTransform, temporaryDestination, 0.9f);

                if (Vector3.SqrMagnitude(actor.ActorTransform.position - currentDestination) <= checkDistance * checkDistance)
                {
                    reachedDestination = true;
                }
            }
        }
        else
        {
            actor.ChangeState(StateType.Attack);
        }

#if UNITY_EDITOR
        Vector3 myposition = actor.ActorTransform.position;
        Debug.DrawLine(currentDestination, currentDestination + Vector3.up * 5, Color.green);
        Debug.DrawLine(myposition, temporaryDestination, Color.blue);
        Debug.DrawRay(myposition, actor.ActorTransform.forward * 1.5f, Color.red);
        if (navMeshAgent.path.corners.Length > 1)
        {
            for (int i = 1; i < navMeshAgent.path.corners.Length; i++)
            {
                Debug.DrawLine(navMeshAgent.path.corners[i - 1], navMeshAgent.path.corners[i], Color.yellow);
            }
        }
#endif
        if (!reachedDestination) UpdateRotateSpped(!lookingAtTemporaryDestination);
        RotateToTargetIgnoreHeight(actor.ActorTransform, temporaryDestination, currentRotateSpeed);

        frameCounter++;
    }

    public void OnExit()
    {

    }

    #region SUPPORTIVE
    private void UpdateRotateSpped(bool speedUp = true)
    {
        if (speedUp)
        {
            currentRotateSpeed += actor.AttributesConfig.RotateSpeedAcceleration * 1.5f;
            currentRotateSpeed = Mathf.Min(currentRotateSpeed, actor.AttributesConfig.RotationSpeed * 2);
        }
        else
        {
            currentRotateSpeed -= actor.AttributesConfig.RotateSpeedAcceleration * 1.5f;
            currentRotateSpeed = Mathf.Max(currentRotateSpeed, actor.AttributesConfig.RotationSpeed * 2);
        }
    }
    #endregion
}
