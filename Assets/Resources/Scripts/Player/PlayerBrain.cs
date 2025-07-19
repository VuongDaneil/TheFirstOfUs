using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using WeaponSystem;

public class PlayerBrain : MonoBehaviour, IActor, IDataPersistence
{
    #region PROPERTIES  
    public static PlayerBrain Instance { get; private set; }

    [SerializeField] protected ActorType actorRole = ActorType.Player;
    public ActorType ActorRole => actorRole;
    public PlayerStatsConfig PlayerStats;
    public bool IsInvincible = false;

    [Header("CONTROLLER(s)")]
    public PlayerLookController LookController;
    public PlayerMovementController MovementController;
    public WeaponManager WeaponManager;

    [Header("STAT - HP")]
    [SerializeField] protected int maxHealth = 100;
    [ReadOnly] public int CurrentHealth;
    public int MaxHealth => maxHealth;
    public int Health { get => CurrentHealth; set => CurrentHealth = Mathf.Clamp(value, 0, MaxHealth); }

    [Header("STAT - STAMINA")]
    public int MaxStamina = 100;
    public float StaminaConsumeRate = 1;
    [ReadOnly] public float CurrentStamina = 100;
    public float SelfHealingRate = 5f;
    private float hurtedTimer = 0;

    [Header("MOVEMENT")]
    [ReadOnly] public PlayerMovementStage CurrentMovementState = PlayerMovementStage.StandStill;
    [ReadOnly] public CharacterStanceStatus CurrentStanceStage = CharacterStanceStatus.Standing;

    [Header("ANIMATION")]
    public Animator PlayerAnimator;
    public string IntroAnimState;
    public string DieAnimState;

    public bool IsReady = false;
    public bool IsPlayer => ActorRole == ActorType.Player;
    public bool IsAlive => CurrentHealth > 0;

    #region _unuse
    public int AttackPower => attackPower;
    protected int attackPower = 10;
    #endregion

    #endregion

    #region UNITY CORE
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            SetUpPLayerStats();

            Heal(99999);
            RestoreStamina();
            RegisterEvents();

            if (DataPersistenceManager.Instance == null)
            {
                PlayerReady();
            }
            else
            {
                if (DataPersistenceManager.Instance.IsNewGameProgress)
                {
                    PlayerAnimator.Play(IntroAnimState);
                }
                else
                {
                    PlayerReady();
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!IsAlive || !IsReady) return;
        if (hurtedTimer > 0)
        {
            hurtedTimer -= Time.deltaTime;
        }
        else
        {
            Heal(50);
            hurtedTimer = SelfHealingRate;
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
        GameplayEventManager.OnGameEnd.AddListener(OnGameEnd);
        PlayerControlEventMananger.OnPlayerChangeMovementState.AddListener(OnPlayerChangeMovementState);
        GameplayEventManager.OnPlayerIntialized?.Invoke();
    }

    private void UnRegisterEvents()
    {
        GameplayEventManager.OnGameEnd.RemoveListener(OnGameEnd);
        PlayerControlEventMananger.OnPlayerChangeMovementState.RemoveListener(OnPlayerChangeMovementState);
    }

    private void OnPlayerChangeMovementState(PlayerMovementStage newStage) => CurrentMovementState = newStage;
    private void OnGameEnd()
    {
        WeaponManager.enabled = false;
        LookController.enabled = false;
        MovementController.enabled = false;
        IsInvincible = true;
    }
    /// <summary>
    /// used in animation event
    /// </summary>
    private void OnPLayerDoneIntro()
    {
        PlayerControlEventMananger.OnPlayerDoneIntro?.Invoke();
        PlayerReady();
    }

    #endregion

    #region _actions
    public void TakeDamage(int amount, IActor source = null)
    {
        hurtedTimer = SelfHealingRate;
        if (IsAlive && CurrentHealth - amount <= 0)
        {
            if (IsInvincible)
            {
                Heal(MaxHealth);
            }
            else
            {
                PlayerControlEventMananger.OnPlayerDie?.Invoke();
                PlayerAnimator.enabled = true;
                PlayerAnimator.Play(DieAnimState);
            }
        }
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        PlayerControlEventMananger.OnPlayerGetHurt?.Invoke();
        PlayerControlEventMananger.OnPlayerHealthChanged?.Invoke(CurrentHealth, MaxHealth, false);
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        PlayerControlEventMananger.OnPlayerHealthChanged?.Invoke(CurrentHealth, MaxHealth, true);
    }

    public void SetHealth(int health)
    {
        CurrentHealth = Mathf.Clamp(health, 0, MaxHealth);
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

    public void SetStamina(float stamina)
    {
        CurrentStamina = Mathf.Clamp(stamina, 0, MaxStamina);
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
    private void SetUpPLayerStats()
    {
        hurtedTimer = 0;
        IsReady = false;
        if (LookController == null) LookController = GetComponent<PlayerLookController>();
        if (MovementController == null) MovementController = GetComponent<PlayerMovementController>();
        if (PlayerStats != null)
        {
            maxHealth = PlayerStats.MaxHealth;
            MaxStamina = PlayerStats.MaxStamina;
            StaminaConsumeRate = PlayerStats.StaminaConsumeRate;
            if (MovementController)
            {
                MovementController.walkSpeed = PlayerStats.WalkSpeed;
                MovementController.runSpeed = PlayerStats.RunSpeed;
                MovementController.jumpHeight = PlayerStats.JumpHeight;
            }
        }
    }
    private void PlayerReady()
    {
        IsReady = true;
        PlayerAnimator.enabled = false;
        WeaponManager.InitializeWeapons();
        PlayerControlEventMananger.OnPlayerReady?.Invoke();
    }
    #endregion

    #region SAVE GAME DATA
    public void LoadData(GameData data)
    {
        if (data == null || DataPersistenceManager.Instance.IsNewGameProgress) return;
        SetHealth(data.PlayerSavedData.PlayerHealth);
        SetStamina(data.PlayerSavedData.PlayerStamina);
        transform.position = data.PlayerSavedData.PlayerPosition;
        transform.eulerAngles = data.PlayerSavedData.PlayerEulerAngle;
        WeaponManager.InitializeWeaponsFromSavedFile(data.PlayerSavedData.MainWeaponCurrentMagazine, data.PlayerSavedData.MainWeaponCurrentAmmoCapacity);
    }

    public void SaveData(ref GameData data)
    {
        if (data == null) return;
        data.PlayerSavedData.PlayerHealth = CurrentHealth;
        data.PlayerSavedData.PlayerStamina = CurrentStamina;
        data.PlayerSavedData.PlayerPosition = transform.position;
        data.PlayerSavedData.PlayerEulerAngle = transform.eulerAngles;
        data.PlayerSavedData.MainWeaponID = WeaponManager.MainWeapon?.WeaponID ?? 0;
        data.PlayerSavedData.SubWeaponID = WeaponManager.SubWeapon?.WeaponID ?? 0;
        data.PlayerSavedData.MainWeaponCurrentMagazine = WeaponManager.MainWeapon?.CurrentAmmo ?? 0;
        data.PlayerSavedData.MainWeaponCurrentAmmoCapacity = WeaponManager.MainWeapon?.CurrentAmmoCapacity ?? 0;
    }
    #endregion
}
