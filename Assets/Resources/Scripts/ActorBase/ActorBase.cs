using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class ActorBase : MonoBehaviour, IActor
{
    #region PROPERTIES
    [Header("CONTROLLER(s)")]
    public StateMachine stateMachine;
    public ActorAnimationControl ActorAnimationController;
    public EnemyConfig AttributesConfig;
    public Transform ActorTransform;

    public IActor Target;
    public Transform TargetTransform;

    public bool Ready = false;

    #region events
    public UnityEvent OnActorDead = new UnityEvent();
    public UnityEvent OnActorStunned = new UnityEvent();
    public UnityEvent OnActorBeingAttackedByPlayer = new UnityEvent();
    #endregion

    [SerializeField] protected ActorType actorRole = ActorType.NPC;
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int attackPower = 10;
    public int CurrentHealth;
    public int Health { get => CurrentHealth; set => CurrentHealth = Mathf.Clamp(value, 0, MaxHealth); }

    public int MaxHealth => maxHealth;

    public int AttackPower => attackPower;

    public bool IsAlive => CurrentHealth > 0;

    public ActorType ActorRole => actorRole;
    public bool IsPlayer => actorRole == ActorType.Player;

    #endregion

    #region UNITY CORE
    protected virtual void Awake()
    {
        Heal(maxHealth + 99);
        ActorTransform = transform;
        stateMachine = new StateMachine();
        ActorAnimationController.Initialize(this);
        PlayerControlEventMananger.OnPlayerReady.AddListener(OnPlayerReady);
        Initialize();
    }

    protected virtual void Update()
    {
        stateMachine.Update();
    }

    private void OnDestroy()
    {
        PlayerControlEventMananger.OnPlayerReady.RemoveListener(OnPlayerReady);
    }
    #endregion

    #region MAIN

    #region _state
    public virtual void Spawn(EnemyConfig attributes, Vector3 spawnPoint)
    {
        SetAttributes(attributes);
        Heal(maxHealth + 99);
        ActorTransform.position = spawnPoint;
    }
    public virtual void ChangeState(StateType nextState) {}
    #endregion

    #region _actions
    public virtual void Attack(IActor target, float damageMultiplier = 1)
    {
        target.TakeDamage((int)(AttackPower * damageMultiplier), this);
    }

    public virtual void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    public virtual void TakeDamage(int amount, IActor source)
    {
        if (!IsAlive) return;
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            OnActorDie();
            OnActorDead?.Invoke();
        }
    }
    #endregion

    #region _movement
    public virtual void MoveTo(Vector3 movePoint)
    {
    }

    public virtual NavMeshAgent GetActorNavMeshAgent()
    {
        return GetComponent<NavMeshAgent>();
    }
    #endregion

    #region _check condition
    public virtual bool IsTargetInChaseRange(float rangeMultiplier = 1)
    {
        return false;
    }
    public virtual bool IsTargetInSurroundingSenseRange()
    {
        return false;
    }
    public virtual bool IsTargetInVisionRange()
    {
        return false;
    }
    public virtual bool IsTargetInAttackRange()
    {
        return false;
    }
    public virtual void Initialize()
    {
        if (PlayerBrain.Instance)
        {
            Target = PlayerBrain.Instance;
            TargetTransform = PlayerBrain.Instance.transform;
        }
    }
    #endregion

    #region _events
    public virtual void OnActorDie() {}
    private void OnPlayerReady()
    {
        Ready = true;
    }
    #endregion

    #endregion

    #region SUPPORTIVE
    public void SetAttributes(EnemyConfig attributes)
    {
        AttributesConfig = attributes;
        maxHealth = attributes.Health;
    }
    #endregion
}
