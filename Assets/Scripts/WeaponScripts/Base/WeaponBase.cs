using UnityEngine;
using System.Collections;
using WeaponSystem;
using NaughtyAttributes;

namespace WeaponSystem
{
    public abstract class WeaponBase : MonoBehaviour, IWeapon
    {
        [Header("CONTROLLER")]
        [ReadOnly] public WeaponManager weaponManager;

        [Header("Weapon Configuration")]
        [SerializeField] protected WeaponData weaponData;
        [SerializeField] protected bool sway;
        [SerializeField][ShowIf("sway")] protected SwayEffect swayEffect;
        [SerializeField][ShowIf("sway")] protected float swayMultiplierADS;


        [Header("Weapon Components")]
        [SerializeField] protected Transform weaponModel;
        [SerializeField] protected Transform opticMount;
        [SerializeField] protected Transform muzzleMount;
        [SerializeField] protected ParticleSystem muzzleFlash;
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected Animator weaponAnimator;
        [SerializeField] protected Camera weaponCamera;

        // Runtime variables
        protected bool isReloading;
        protected bool isInspecting;
        protected bool isAttacking;
        protected bool isAiming;
        protected int currentAmmo;
        protected float lastFireTime;
        protected Camera mainCamera;
        protected WeaponStatModifier currentStats;
        protected float defaultFOV;
        protected float currentFOV;

        // Movement state
        protected bool isWalking;
        protected bool isRunning;
        protected float bobTime;
        protected Vector3 originalPosition;
        protected Vector3 originalRotation;

        [Header("DEBUG(s)")]
        [ReadOnly] public string CurrentWeaponAnimState = "";
        protected Coroutine attackCycleCoroutine;

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            mainCamera = Camera.main;
            currentStats = new WeaponStatModifier();
            
            if (weaponAnimator == null)
                weaponAnimator = GetComponent<Animator>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1f;
                audioSource.maxDistance = 20f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
            }

            defaultFOV = mainCamera != null ? mainCamera.fieldOfView : 60f;
            currentFOV = defaultFOV;
        }
        #endregion

        #region IWeapon Implementation
        public string WeaponName => weaponData?.weaponName;
        public WeaponType Type => weaponData.weaponType;
        public WeaponSlot Slot => weaponData.weaponSlot;
        public bool IsReloading => isReloading;
        public bool IsAiming => isAiming;

        public virtual void Initialize(WeaponManager manager)
        {
            if (!ValidateComponents()) return;

            weaponManager = manager;

            isAiming = false;
            isReloading = false;
            isAttacking = false;
            isInspecting = false;
            currentAmmo = weaponData.magazineSize;
            lastFireTime = 0f;

            if (weaponModel != null)
            {
                originalPosition = weaponData.hipFirePosition;
                originalRotation = weaponData.hipFireRotation;
                weaponModel.localPosition = originalPosition;
                weaponModel.localRotation = Quaternion.Euler(originalRotation);
            }

            PlayAnimation(weaponData.switchInState, weaponData.switchTransitionDuration);
            ClearAttachments();
        }

        public virtual void Fire()
        {
            if (!CanFire()) return;

            HandleFiring();
            lastFireTime = Time.time;
            currentAmmo--;

            if (attackCycleCoroutine != null) StopCoroutine(attackCycleCoroutine);
            attackCycleCoroutine = StartCoroutine(AttackingCycle());
        }

        public virtual void Reload()
        {
            if (!CanReload()) return;
            StartCoroutine(ReloadRoutine());
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

        public virtual bool CanFire()
        {
            if (weaponData == null || isInspecting || isReloading)
                return false;

            return currentAmmo > 0 && Time.time >= lastFireTime + (weaponData.fireRate / currentStats.adsSpeedMultiplier);
        }

        public virtual bool CanReload()
        {
            return !isReloading && currentAmmo < weaponData.magazineSize;
        }

        public virtual void OnReloadAnimationComplete()
        {
            isReloading = false;
            currentAmmo = weaponData.magazineSize;
            PlayAnimation(weaponData.idleState, weaponData.defaultTransitionDuration);
        }

        public virtual void OnInspectAnimationComplete()
        {
            isInspecting = false;
            PlayAnimation(weaponData.idleState, weaponData.defaultTransitionDuration);
        }

        public virtual AudioClip GetWeaponSound(WeaponSoundType type)
        {
            return null;
        }

        public virtual void AttachOptic(WeaponOptic optic)
        {
            if (opticMount == null)
            {
                Debug.LogError($"[{GetType().Name}] No optic mount point assigned!");
                return;
            }

            foreach (Transform child in opticMount)
                GameObject.Destroy(child.gameObject);
        }

        public virtual void AttachMuzzle(WeaponMuzzle muzzle)
        {
            if (muzzleMount == null)
            {
                Debug.LogError($"[{GetType().Name}] No muzzle mount point assigned!");
                return;
            }

            foreach (Transform child in muzzleMount)
                GameObject.Destroy(child.gameObject);
        }
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

        protected virtual void HandleFiring()
        {
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            
            float accuracy = weaponData.baseAccuracy * currentStats.accuracyMultiplier;
            if (isAiming) accuracy *= weaponData.adsAccuracyMultiplier;

            Vector3 spread = Random.insideUnitSphere * (1f - accuracy);
            ray.direction = Quaternion.Euler(spread.x, spread.y, 0) * ray.direction;

            if (Physics.Raycast(ray, out RaycastHit hit))
                HandleHit(hit);

            PlayEffects();
            PlayAnimation(weaponData.attackState, weaponData.attackTransitionDuration, rebind: true);
        }

        protected virtual void HandleHit(RaycastHit hit)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float finalDamage = weaponData.damage * currentStats.damageMultiplier;
                damageable.TakeDamage(finalDamage);
            }
        }

        protected virtual IEnumerator ReloadRoutine()
        {
            isReloading = true;
            PlayAnimation(weaponData.reloadState, weaponData.defaultTransitionDuration);
            yield return new WaitForSeconds(1f);
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

        protected virtual void PlayEffects()
        {
            if (muzzleFlash != null)
                muzzleFlash.Play();

            if (audioSource != null)
                audioSource.Play();
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
            if (opticMount != null)
                foreach (Transform child in opticMount)
                    GameObject.Destroy(child.gameObject);

            if (muzzleMount != null)
                foreach (Transform child in muzzleMount)
                    GameObject.Destroy(child.gameObject);
        }
        #endregion
    }
}