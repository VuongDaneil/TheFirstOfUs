using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PlayerBrain : MonoBehaviour, IActor
{
    public static PlayerBrain Instance { get; private set; }

    [SerializeField] protected ActorType actorRole = ActorType.Player;
    public ActorType ActorRole => actorRole;

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

    public bool IsPlayer => ActorRole == ActorType.Player;
    public bool IsAlive => throw new System.NotImplementedException();

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
        PlayerControlEventsMananger.OnPlayerChangeMovementState.AddListener(OnPlayerChangeMovementState);
        GameplayEventManager.OnPlayerIntialized?.Invoke();
    }

    private void UnRegisterEvents()
    {
        PlayerControlEventsMananger.OnPlayerChangeMovementState.RemoveListener(OnPlayerChangeMovementState);
    }

    private void OnPlayerChangeMovementState(PlayerMovementStage newStage) => CurrentMovementState = newStage;
    #endregion

    #region _actions
    public void TakeDamage(int amount, IActor source = null)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        PlayerControlEventsMananger.OnPlayerHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        PlayerControlEventsMananger.OnPlayerHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void ConsumeStamina()
    {
        CurrentStamina -= StaminaConsumeRate;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
        PlayerControlEventsMananger.OnPlayerStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
    }

    public void RecoverStamina(float multiplierRate = 1f)
    {
        CurrentStamina += (StaminaConsumeRate / 2f) * multiplierRate;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
        PlayerControlEventsMananger.OnPlayerStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
    }

    public void RestoreStamina()
    {
        CurrentStamina = MaxStamina;
        PlayerControlEventsMananger.OnPlayerStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
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
