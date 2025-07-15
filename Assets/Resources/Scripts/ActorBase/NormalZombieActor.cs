using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

public class NormalZombieActor : ActorBase
{
    #region PROPERTIES
    private IState idleState;
    private IState moveState;
    private IState attackState;
    private IState chaseState;
    private IState aggressiveChaseState;
    private IState stunState;
    private IState deadState;
    private IState runAwayState;

    [Header("CONTROLLER(s)")]
    public NavMeshAgent ActorNavAgent;

    [Header("DEBUG")]
    [ReadOnly] public string CurrentStateName = "IDLE";
    #endregion
    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState(this);
        moveState = new MoveState(this);
        deadState = new DeadState(this);
        chaseState = new ChaseState(this);
        aggressiveChaseState = new AggressiveChaseState(this);
        attackState = new AttackState(this);
        stunState = new StunState(this);
        runAwayState = new RunAwayState(this);
    }
    protected override void Update()
    {
        base.Update();
#if UNITY_EDITOR
        DebugBotVision();
#endif
    }

    private void OnValidate()
    {
        if (ActorNavAgent == null) ActorNavAgent = GetComponent<NavMeshAgent > ();
        if (ActorAnimationController == null) ActorAnimationController = GetComponent< ActorAnimationControl>();
    }

    public override void ChangeState(StateType _state)
    {
        IState nextState = null;
        switch (_state)
        {
            case StateType.Idle:            nextState = idleState; break;
            case StateType.Move:            nextState = moveState; break;
            case StateType.Attack:          nextState = attackState; break;
            case StateType.Chase:           nextState = chaseState; break;
            case StateType.Stun:            nextState = stunState; break;
            case StateType.Dead:            nextState = deadState; break;
            case StateType.AggressiveChase: nextState = aggressiveChaseState; break;
            case StateType.RunAway:         nextState = runAwayState; break;
        }
        if (nextState != null)
        {
            CurrentStateName = nextState.StateName;

            stateMachine.PreviousStateType = stateMachine.CurrentStateType;
            stateMachine.CurrentStateType = _state;
            stateMachine.ChangeState(nextState);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterAllEvent();
    }

    #region MAIN

    public override void Spawn(EnemyConfig attributes, Vector3 spawnPoint)
    {
        base.Initialize();
        ActorNavAgent.enabled = false;
        gameObject.SetActive(true);
        SetAttributes(attributes);
        Heal(maxHealth + 99);
        ActorTransform.position = spawnPoint;
        ActorNavAgent.enabled = true;

        ChangeState(StateType.Idle);
    }

    #region _events
    private void RegisterAllEvent()
    {
        OnActorStunned.AddListener(() =>
        {
            if (IsAlive && !stateMachine.IsInState(StateType.Stun)) ChangeState(StateType.Stun);
        });

        OnActorBeingAttackedByPlayer.AddListener(() =>
        {
            if (IsAlive && (stateMachine.IsInState(StateType.Idle) || stateMachine.IsInState(StateType.Move)))
            {
                ChangeState(StateType.AggressiveChase);
            }
        });
        GameplayEventManager.OnPlayerMakeNoise.AddListener(OnPlayerMakeNoise);
    }
    private void OnPlayerMakeNoise(float noiseRange)
    {
        if (IsAlive & (stateMachine.IsInState(StateType.Idle) || stateMachine.IsInState(StateType.Move)))
        {
            bool hearable = AttributesConfig.IsTargetInHearingRange(TargetTransform.position, ActorTransform, noiseRange);
            if (hearable) ChangeState(StateType.AggressiveChase);
        }
    }
    public override void OnActorDie()
    {
        ChangeState(StateType.Dead);
        GameplayEventManager.OnAnEnemyDead?.Invoke(this);
    }
    #endregion

    public override void Attack(IActor target, float damageMultiplier = 1)
    {
        if (!AttributesConfig.CanDealDamage(TargetTransform.position, ActorTransform)) return;
        if (AttributesConfig.ChooseBrutality()) damageMultiplier = Random.Range(1, AttributesConfig.Brutality);
        base.Attack(target, damageMultiplier);
    }
    public override void TakeDamage(int amount, IActor source)
    {
        base.TakeDamage(amount, source);

        if (source != null && source.IsPlayer)
        {
            OnActorBeingAttackedByPlayer?.Invoke();
            GameplayEventManager.OnAnEnemyAttackedByPlayer?.Invoke(ActorTransform.position);
        }

        bool canbeStunned = amount >= maxHealth * AttributesConfig.HealthPercentageCauseStun / 100f;
        if (!canbeStunned) canbeStunned = Random.Range(0f, 1f) <= 0.35f;

        if (IsAlive && canbeStunned)
        {
            OnActorStunned?.Invoke();
        }
    }
    #endregion

    #region SUPPORTIVE

    public override NavMeshAgent GetActorNavMeshAgent()
    {
        return ActorNavAgent;
    }

    public override bool IsTargetInChaseRange(float rangeMultiplier = 1)
    {
        return AttributesConfig.IsTargetInChaseRange(TargetTransform.position, ActorTransform, rangeMultiplier);
    }

    public override bool IsTargetInSurroundingSenseRange()
    {
        return AttributesConfig.IsTargetInSurroundingSenseRange(TargetTransform.position, ActorTransform);
    }

    public override bool IsTargetInVisionRange()
    {
        return AttributesConfig.IsTargetInVisionRange(TargetTransform.position, ActorTransform);
    }

    public override bool IsTargetInAttackRange()
    {
        return AttributesConfig.IsTargetInAttackRange(TargetTransform.position, ActorTransform);
    }
    #endregion

    #region DEBUG(s)
    private void DebugBotVision()
    {
        if (AttributesConfig == null || TargetTransform == null) return;
        Vector3 actorPosition = ActorTransform.position;
        Debug.DrawRay(actorPosition, ActorTransform.forward * AttributesConfig.VisionRange, Color.green);
        //draw angle
        Debug.DrawRay(actorPosition, Quaternion.Euler(0, AttributesConfig.VisionAngle / 2, 0) * ActorTransform.forward * AttributesConfig.VisionRange, Color.red);
        Debug.DrawRay(actorPosition, Quaternion.Euler(0, -AttributesConfig.VisionAngle / 2, 0) * ActorTransform.forward * AttributesConfig.VisionRange, Color.red);
        //Debug.DrawRay(actorPosition, Quaternion.Euler(0, -AttributesConfig.VisionAngle / 2, 0) * (targetPosition - actorPosition).normalized * AttributesConfig.VisionRange, Color.red);
    }
    #endregion
}