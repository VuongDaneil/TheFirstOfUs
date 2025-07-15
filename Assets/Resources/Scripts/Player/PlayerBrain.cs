using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PlayerBrain : MonoBehaviour, IActor
{
    public static PlayerBrain Instance { get; private set; }

    [SerializeField] protected ActorType actorRole = ActorType.Player;
    public ActorType ActorRole => actorRole;
    public bool IsInvincible = false;

    [Header("STAT - HP")]
    [SerializeField] protected int maxHealth = 100;
    [ReadOnly] public int CurrentHealth;
    public int MaxHealth => maxHealth;
    public int Health { get => CurrentHealth; set => CurrentHealth = Mathf.Clamp(value, 0, MaxHealth); }

    [Header("STAT - STAMINA")]
    public int MaxStamina = 100;
    public float StaminaConsumeRate = 1;
    [ReadOnly] public float CurrentStamina = 100;

    [Header("MOVEMENT")]
    [ReadOnly] public PlayerMovementStage CurrentMovementState = PlayerMovementStage.StandStill;
    [ReadOnly] public CharacterStanceStatus CurrentStanceStage = CharacterStanceStatus.Standing;

    [Header("ANIMATION")]
    public Animation DieAnimation;

    public bool IsPlayer => ActorRole == ActorType.Player;
    public bool IsAlive => CurrentHealth > 0;

    #region _unuse
    public int AttackPower => attackPower;
    protected int attackPower = 10;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Heal(99999);
            RestoreStamina();
            RegisterEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) UnRegisterEvents();
    }
    #endregion

    #region MAIN

    #region _events

    private void RegisterEvents()
    {
        PlayerControlEventMananger.OnPlayerChangeMovementState.AddListener(OnPlayerChangeMovementState);
        GameplayEventManager.OnPlayerIntialized?.Invoke();
    }

    private void UnRegisterEvents()
    {
        PlayerControlEventMananger.OnPlayerChangeMovementState.RemoveListener(OnPlayerChangeMovementState);
    }

    private void OnPlayerChangeMovementState(PlayerMovementStage newStage) => CurrentMovementState = newStage;
    private void OnPlayerDie()
    {

    }
    #endregion

    #region _actions
    public void TakeDamage(int amount, IActor source = null)
    {
        if (IsAlive && CurrentHealth - amount <= 0)
        {
            if (IsInvincible)
            {
                Heal(MaxHealth);
            }
            else
            {
                PlayerControlEventMananger.OnPlayerDie?.Invoke();
                DieAnimation.Play();
            }
        }
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        PlayerControlEventMananger.OnPlayerHealthChanged?.Invoke(CurrentHealth, MaxHealth, false);
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        PlayerControlEventMananger.OnPlayerHealthChanged?.Invoke(CurrentHealth, MaxHealth, true);
    }

    public void ConsumeStamina()
    {
        CurrentStamina -= StaminaConsumeRate;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
        if (CurrentStamina <= MaxStamina * 0.15f && IsInvincible) RestoreStamina();
        PlayerControlEventMananger.OnPlayerStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
    }

    public void RecoverStamina(float multiplierRate = 1f)
    {
        CurrentStamina += (StaminaConsumeRate / 2f) * multiplierRate;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
        PlayerControlEventMananger.OnPlayerStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
    }

    public void RestoreStamina()
    {
        CurrentStamina = MaxStamina;
        PlayerControlEventMananger.OnPlayerStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
    }

    public void Attack(IActor target, float damageMultiplier)
    {
        return;
    }
    #endregion

    #endregion

    #region SUPPORTIVE
    public bool CanSprint() => CurrentStamina >= (MaxStamina * 0.1f);
    #endregion
}
