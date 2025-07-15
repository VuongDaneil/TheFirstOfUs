using UnityEngine;
using System.Collections;
using WeaponSystem;
using NaughtyAttributes;
using UnityEngine.Events;

namespace WeaponSystem
{
    public abstract class WeaponBase : MonoBehaviour, IWeapon
    {
        #region PROPERTIES
        [ReadOnly]public EquipState CurrentEquipState;
        public EquipState currentEquipState => CurrentEquipState;
        public bool IsEquipped => CurrentEquipState == EquipState.Equipped;

        [Header("CONTROLLER")]
        [ReadOnly] public WeaponManager weaponManager;

        [Header("Weapon Configuration")]
        [SerializeField] protected WeaponData weaponData;
        [SerializeField] protected bool sway;
        [SerializeField][ShowIf("sway")] protected SwayEffect swayEffect;
        [SerializeField][ShowIf("sway")] protected float swayMultiplierADS;

        [Header("Weapon Components")]
        public Transform BulletOutPuter;
        [SerializeField] protected Transform weaponModel;
        [SerializeField] protected AudioSource gunAudioSource;
        [SerializeField] protected Animator weaponAnimator;
        [SerializeField] protected Camera weaponCamera;

        // Runtime variables
        protected bool isReloading;
        protected bool isInspecting;
        protected bool isAttacking;
        protected bool isAiming;
        protected int currentAmmo;
        protected int currentAmmoCapacity;
        protected float lastFireTime;
        protected Camera mainCamera;
        protected float defaultFOV;
        protected float currentFOV;
        //protected WeaponStatModifier currentStats;

        // Movement state
        protected bool isWalking;
        protected bool isRunning;
        protected float bobTime;
        protected Vector3 originalPosition;
        protected Vector3 originalRotation;

        public string WeaponName => weaponData?.weaponName;
        public WeaponType Type => weaponData.weaponType;
        public WeaponSlot Slot => weaponData.weaponSlot;
        public bool IsReloading => isReloading;
        public bool IsAiming => isAiming;

        [Header("EVENT(s)")]
        public UnityEvent OnWeaponFire;

        [Header("DEBUG(s)")]
        [ReadOnly] public string CurrentWeaponAnimState = "";
        [ReadOnly] public float CurrentSpreadFactor = 0;
        [ReadOnly] public float SpreadReducePercentage = 0;
        [ReadOnly] public float DamageBuffPercentage = 0;
        protected Coroutine attackCycleCoroutine;
        public int WeaponID => weaponData != null ? weaponData.WeaponID : -1;
        public int CurrentAmmo => currentAmmo;
        public int CurrentAmmoCapacity => currentAmmoCapacity;

        #endregion

        #region UNITY CORE
        protected virtual void Awake()
        {
            mainCamera = Camera.main;
            //currentStats = new WeaponStatModifier();

            if (weaponAnimator == null)
                weaponAnimator = GetComponent<Animator>();

            if (gunAudioSource == null)
            {
                gunAudioSource = gameObject.AddComponent<AudioSource>();
                gunAudioSource.spatialBlend = 1f;
                gunAudioSource.maxDistance = 20f;
                gunAudioSource.rolloffMode = AudioRolloffMode.Linear;
            }

            defaultFOV = mainCamera != null ? mainCamera.fieldOfView : 60f;
            currentFOV = defaultFOV;

            RegisterAllEvents();
        }

        protected virtual void Update()
        {
            if (CurrentSpreadFactor > 0)
            {
                CurrentSpreadFactor -= weaponData.SpreadFactorIncreasingRate * Time.deltaTime;
                CurrentSpreadFactor = Mathf.Max(CurrentSpreadFactor, 0);
            }
        }

        private void OnDestroy()
        {
            UnregisterAllEvents();
        }
        #endregion

        #region MAIN

        #region _events
        private void RegisterAllEvents()
        {
            PlayerControlEventsMananger.OnWeaponReloadDone.AddListener(OnReloadAnimationComplete);
        }

        private void UnregisterAllEvents()
        {
            PlayerControlEventsMananger.OnWeaponReloadDone.RemoveListener(OnReloadAnimationComplete);
        }

        public virtual void OnReloadAnimationComplete()
        {
            if (!IsEquipped) return;
            isReloading = false;

            int ammoAvailableToReload = Mathf.Min(weaponData.magazineSize, currentAmmoCapacity);

            currentAmmo = ammoAvailableToReload;
            currentAmmoCapacity -= currentAmmo;

            PlayerControlEventsMananger.OnWeaponAmmoChange?.Invoke(currentAmmo, currentAmmoCapacity);
            PlayAnimation(weaponData.idleState, weaponData.defaultTransitionDuration);
        }

        public virtual void OnInspectAnimationComplete()
        {
            if (!IsEquipped) return;
            isInspecting = false;
            PlayAnimation(weaponData.idleState, weaponData.defaultTransitionDuration);
        }
        #endregion

        #region _action
        public void OnEquip()
        {
            SwitchInAnimation();
            CurrentEquipState = EquipState.Equipped;
        }

        public void OnUnequip()
        {
            SwitchOutAnimation();
            CurrentEquipState = EquipState.Unequipped;
        }

        public virtual void Fire()
        {
            if (!CanFire()) return;

            HandleFiring(out Vector3 bulletDirrection);
            lastFireTime = Time.time;
            currentAmmo--;
            PlayerControlEventsMananger.OnWeaponAmmoChange?.Invoke(currentAmmo, currentAmmoCapacity);

            if (attackCycleCoroutine != null) StopCoroutine(attackCycleCoroutine);
            attackCycleCoroutine = StartCoroutine(AttackingCycle());

            OnWeaponFire?.Invoke();
        }

        public virtual void Reload()
        {
            if (!CanReload()) return;
            isReloading = true;
            PlayAnimation(weaponData.reloadState, weaponData.defaultTransitionDuration);
        }

        public virtual void AimDownSight(bool aiming)
        {
            if (isAiming == aiming || isReloading || isInspecting) return;

            isAiming = aiming;

            if (isAiming) swayEffect.ResetSwayPositionAndRotation();

            if (weaponModel == null) return;

            Vector3 targetPosition = aiming ? weaponData.adsPosition : originalPosition;
            Vector3 targetRotation = aiming ? weaponData.adsRotation : originalRotation;

            LeanTween.cancel(weaponModel.gameObject);
            LeanTween.moveLocal(weaponModel.gameObject, targetPosition, weaponData.movementTransitionSpeed)
                    .setEaseInOutQuad();
            LeanTween.rotateLocal(weaponModel.gameObject, targetRotation, weaponData.movementTransitionSpeed)
                    .setEaseInOutQuad();

            float targetFOV = aiming ? weaponData.aimingFOV : weaponData.defaultFOV;
            StartCoroutine(TransitionFOV(targetFOV));

            string targetState = aiming ? weaponData.idleState : weaponData.idleState;
            PlayAnimation(targetState, weaponData.defaultTransitionDuration);
        }

        public virtual void Inspect()
        {
            if (isReloading || isInspecting) return;

            isInspecting = true;
            PlayAnimation(weaponData.inspectState, weaponData.inspectTransitionDuration);
        }
        #endregion

        #region _implementation

        public virtual void Initialize(WeaponManager manager)
        {
            if (!ValidateComponents()) return;

            weaponManager = manager;

            isAiming = false;
            isReloading = false;
            isAttacking = false;
            isInspecting = false;
            currentAmmo = weaponData.magazineSize;
            currentAmmoCapacity = weaponData.maxAmmoCapacity;
            lastFireTime = 0f;

            PlayerControlEventsMananger.OnWeaponAmmoChange?.Invoke(currentAmmo, currentAmmoCapacity);

            if (weaponModel != null)
            {
                originalPosition = weaponData.hipFirePosition;
                originalRotation = weaponData.hipFireRotation;
                weaponModel.localPosition = originalPosition;
                weaponModel.localRotation = Quaternion.Euler(originalRotation);
            }

            OnEquip();
            ClearAttachments();
        }

        public virtual void OnSwitchIn()
        {
            isAiming = false;
            isReloading = false;
            isAttacking = false;
            isInspecting = false;
            lastFireTime = 0f;
            if (weaponModel != null)
            {
                originalPosition = weaponData.hipFirePosition;
                originalRotation = weaponData.hipFireRotation;
                weaponModel.localPosition = originalPosition;
                weaponModel.localRotation = Quaternion.Euler(originalRotation);
            }
            OnEquip();
        }

        #endregion

        #endregion

        #region Protected Methods
        public virtual void UpdateMovementState(bool walking, bool running)
        {
            if (sway) swayEffect.SwayAction(noHovering: isAiming, amountMultiplier: isAiming ? swayMultiplierADS : 1);

            isWalking = walking;
            isRunning = running;

            if (!isAiming && !isReloading && !isInspecting)
            {
                //UpdateWeaponBobbing();
                UpdateWeaponPosition();
                UpdateMovementAnimation();
            }
        }

        private void UpdateMovementAnimation()
        {
            if (isAttacking) return;

            string targetState = weaponData.idleState;
            
            if (isRunning)
                targetState = weaponData.runState;
            else if (isWalking)
                targetState = weaponData.walkState;
            else
            {
                PlayAnimation(targetState, weaponData.movementTransitionDuration);
                return;
            }

            PlayAnimation(targetState, weaponData.movementTransitionDuration, true);
        }

        protected virtual void HandleFiring(out Vector3 bulletDirrection)
        {
            CurrentSpreadFactor += weaponData.SpreadFactorIncreasingRate;
            CurrentSpreadFactor -= CurrentSpreadFactor * (SpreadReducePercentage / 100f);
            CurrentSpreadFactor = Mathf.Min(CurrentSpreadFactor, weaponData.MaximumSpreadFactor);

            Ray ray = new Ray
            {
                origin = BulletOutPuter.position,
                direction = BulletOutPuter.forward
            };
            if (isAiming) CurrentSpreadFactor *= weaponData.adsSpreadReduceFactor;

            Vector3 spread = Random.insideUnitSphere * CurrentSpreadFactor;
            bulletDirrection = Quaternion.Euler(spread.x, spread.y, 0) * BulletOutPuter.forward;
            ray.direction = bulletDirrection;
            PlayerControlEventsMananger.OnWeaponShootDirection?.Invoke(ray.direction);

            if (Physics.Raycast(ray, out RaycastHit hit))
                HandleHit(hit);

            PlayShootSoundEffects();
            GameplayEventManager.OnPlayerMakeNoise?.Invoke(weaponData.NoiseCausedRange);
            PlayAnimation(weaponData.attackState, weaponData.attackTransitionDuration, rebind: true);
        }

        protected virtual void HandleHit(RaycastHit hit)
        {
            IHitableObject damageable = hit.collider.GetComponent<IHitableObject>();
            if (damageable != null)
            {
                damageable.OnHit(hit);
                float finalDmg = weaponData.damage + weaponData.damage * (DamageBuffPercentage / 100f);
                IActor actorHit = hit.collider.GetComponentInParent<IActor>();
                if (actorHit != null)
                {
                    actorHit.TakeDamage((int)finalDmg, PlayerBrain.Instance);
                }
            }
        }

        protected virtual IEnumerator AttackingCycle()
        {
            isAttacking = true;
            yield return new WaitForSeconds(weaponData.attackAnimationDuration);
            isAttacking = false;
        }

        protected virtual void PlayAnimation(string stateName, float transitionDuration, bool checkOverlap = false, bool rebind = false)
        {
            if (checkOverlap)
            {
                bool sameAsCurrentAnimation = CurrentWeaponAnimState.Equals(stateName, System.StringComparison.OrdinalIgnoreCase);
                if (sameAsCurrentAnimation) return;
            }
            CurrentWeaponAnimState = stateName;
            if (rebind) weaponAnimator.Rebind();
            weaponAnimator.CrossFade(stateName, transitionDuration);
        }

        protected virtual void UpdateWeaponPosition()
        {
            Vector3 targetPosition = originalPosition;
            Vector3 targetRotation = originalRotation;

            weaponModel.localPosition = Vector3.Lerp(
                weaponModel.localPosition,
                targetPosition,
                Time.deltaTime * weaponData.movementTransitionSpeed
            );

            weaponModel.localRotation = Quaternion.Lerp(
                weaponModel.localRotation,
                Quaternion.Euler(targetRotation),
                Time.deltaTime * weaponData.movementTransitionSpeed
            );
        }

        protected virtual IEnumerator TransitionFOV(float targetFOV)
        {
            float startFOV = currentFOV;
            float elapsedTime = 0f;

            while (elapsedTime < weaponData.fovTransitionSpeed)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / weaponData.fovTransitionSpeed;
                
                currentFOV = Mathf.Lerp(startFOV, targetFOV, t);
                if (mainCamera != null)
                    mainCamera.fieldOfView = currentFOV;
                
                yield return null;
            }

            currentFOV = targetFOV;
            if (mainCamera != null)
                mainCamera.fieldOfView = targetFOV;
        }

        protected virtual void PlayShootSoundEffects()
        {
            if (gunAudioSource != null)
            {
                gunAudioSource.PlayOneShot(weaponData.AttackSoundEffects.GetRandom());
            }
        }

        protected bool ValidateComponents()
        {
            if (weaponData == null)
            {
                Debug.LogError($"[{GetType().Name}] WeaponData is not assigned!");
                return false;
            }

            if (weaponModel == null)
            {
                Debug.LogError($"[{GetType().Name}] WeaponHolder is not assigned!");
                return false;
            }

            if (weaponAnimator == null)
                Debug.LogWarning($"[{GetType().Name}] Animator is not assigned - animations will be disabled.");

            return true;
        }

        protected void ClearAttachments()
        {

        }

        public void AttachAttachment(WeaponAttachment attachment)
        {
            
        }
        #endregion

        #region SUPPORTIVE
        public void SwitchInAnimation()
        {
            PlayAnimation(weaponData.switchInState, weaponData.switchTransitionDuration);
        }
        public void SwitchOutAnimation()
        {
            PlayAnimation(weaponData.switchOutState, weaponData.switchTransitionDuration);
        }

        public void ApplyDefaultPosition()
        {
            weaponModel.localPosition = originalPosition;
            weaponModel.localRotation = Quaternion.Euler(originalRotation);
        }
        public void ApplyADSPosition()
        {
            weaponModel.localPosition = weaponData.adsPosition;
            weaponModel.localRotation = Quaternion.Euler(weaponData.adsRotation);
        }
        public virtual bool CanFire()
        {
            if (!IsEquipped) return false;
            if (weaponData == null || isInspecting || isReloading || PlayerBrain.Instance.CurrentMovementState == PlayerMovementStage.Sprinting)
                return false;

            return currentAmmo > 0 && Time.time >= lastFireTime + weaponData.fireRate;
        }
        public virtual AudioClip GetWeaponSound(WeaponSoundType type)
        {
            return null;
        }

        public virtual bool CanReload()
        {
            return !isReloading && currentAmmo < weaponData.magazineSize && currentAmmoCapacity > 0 && IsEquipped;
        }
        #endregion
    }
}