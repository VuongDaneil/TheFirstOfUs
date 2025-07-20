using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static VExtension;

public class MoveState : IState
{
    private ActorBase actor;

    public MoveState(ActorBase actor) => this.actor = actor;
    public string StateName => "MOVING";

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

        float moveDistanceMultiplier = 1;
        if (actor.AttributesConfig.ChooseLaziness()) moveDistanceMultiplier = Random.Range(0.2f, 1f);
        actor.AttributesConfig.GetRandomPointOnNavMesh(actor.ActorTransform.position, out currentDestination, moveDistanceMultiplier);

        checkDistance = actor.AttributesConfig.DistanceCheckDestinationStop;

        navMeshAgent = actor.GetActorNavMeshAgent();
        actor.ActorAnimationController.PlayMoveAudio();
        actor.ActorAnimationController.PlayMoveAnimation();
    }

    public void OnUpdate()
    {

        if (detectTargetFrameCounter > 5 && actor.Ready)
        {
            detectTargetFrameCounter = 0;
            if (actor.IsTargetInVisionRange() || actor.IsTargetInSurroundingSenseRange())
            {
                actor.ChangeState(StateType.Chase);
                return;
            }
        }
        else detectTargetFrameCounter++;

        if (navMeshAgent == null) return;
        if (!reachedDestination)
        {
            if (frameCounter > 10 && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.SetDestination(currentDestination);


                if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    if (navMeshAgent.path.corners.Length > 1)
                    {
                        temporaryDestination = navMeshAgent.path.corners[1];
                    }
                }
                else temporaryDestination = currentDestination;

                lookingAtTemporaryDestination = IsLookAtIgnoreY(actor.ActorTransform, temporaryDestination, 0.9f);

                if (Vector3.SqrMagnitude(actor.ActorTransform.position - currentDestination) <= checkDistance * checkDistance)
                {
                    reachedDestination = true;
                }

                frameCounter = 0;
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
            currentRotateSpeed -= actor.AttributesConfig.RotateSpeedAcceleration ;
            currentRotateSpeed = Mathf.Max(currentRotateSpeed, actor.AttributesConfig.RotationSpeed);
        }
    }
    #endregion
}
