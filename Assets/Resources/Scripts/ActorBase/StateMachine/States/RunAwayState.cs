using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static VExtension;

public class RunAwayState : IState
{
    private ActorBase actor;

    public RunAwayState(ActorBase actor) => this.actor = actor;
    public string StateName => "RUN AWAY";

    private NavMeshAgent navMeshAgent;
    private bool reachedDestination = false;
    private int frameCounter = 0;
    private int detectTargetFrameCounter = 0;

    private Vector3 currentDestination;
    private Vector3 temporaryDestination;
    private float currentRotateSpeed;
    private bool lookingAtTemporaryDestination = false;

    private float checkDistance = 1.5f;

    public void OnEnter()
    {
        currentRotateSpeed = 0;
        reachedDestination = false;
        actor.AttributesConfig.GetRandomPointOnNavMesh(actor.ActorTransform.position, out currentDestination);

        checkDistance = actor.AttributesConfig.DistanceCheckDestinationStop;

        navMeshAgent = actor.GetActorNavMeshAgent();
        actor.ActorAnimationController.PlayMoveAudio();
        actor.ActorAnimationController.PlayFastChaseAnimation();
    }

    public void OnUpdate()
    {
        if (navMeshAgent == null) return;
        if (!reachedDestination)
        {
            if (frameCounter == 0 || frameCounter.DivisibleFor(10))
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
            actor.ChangeState(StateType.Idle);
        }

#if UNITY_EDITOR
        Vector3 myposition = actor.ActorTransform.position;
        Debug.DrawLine(currentDestination, currentDestination + Vector3.up * 5, Color.green);
        Debug.DrawLine(myposition, temporaryDestination, Color.blue);
        Debug.DrawRay(myposition, actor.ActorTransform.forward * 1.5f, Color.red);
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
            currentRotateSpeed += actor.AttributesConfig.RotateSpeedAcceleration;
            currentRotateSpeed = Mathf.Min(currentRotateSpeed, actor.AttributesConfig.RotationSpeed);
        }
        else
        {
            currentRotateSpeed -= actor.AttributesConfig.RotateSpeedAcceleration;
            currentRotateSpeed = Mathf.Max(currentRotateSpeed, actor.AttributesConfig.RotationSpeed);
        }
    }
    #endregion
}
