using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using WeaponSystem;

namespace WeaponSystem
{
    public class RifleWeapon : WeaponBase
    {
        #region PROPERTIES
        [Header("Rifle State")]
        private FireMode currentFireMode;
        private int burstRemaining;
        private bool isFiring;
        #endregion

        #region Weapon Setup
        protected override void Awake()
        {
            base.Awake();
            ValidateWeaponType();
            burstRemaining = weaponData.burstCount;
        }

        public override void Initialize(WeaponManager manager)
        {
            if (!ValidateWeaponType()) return;
            base.Initialize(manager);

            currentFireMode = weaponData.defaultFireMode;
            burstRemaining = weaponData.burstCount;
            isFiring = false;
        }

        private bool ValidateWeaponType()
        {
            if (weaponData == null) return false;
            
            if (!weaponData.IsRifle)
            {
                Debug.LogError($"[{GetType().Name}] Invalid weapon type. Expected Rifle, got {weaponData.weaponType}");
                return false;
            }
            return true;
        }
        #endregion

        #region Firing System
        public override void Fire()
        {
            if (!CanFire()) return;

            switch (currentFireMode)
            {
                case FireMode.Single:
                    base.Fire();
                    break;

                case FireMode.Burst:
                    if (weaponData.CanBurst && burstRemaining == weaponData.burstCount)
                    {
                        base.Fire();
                        burstRemaining--;
                        StartCoroutine(BurstFireRoutine());
                    }
                    break;

                case FireMode.Auto:
                    if (weaponData.CanAutoFire)
                    {
                        base.Fire();
                    }
                    break;
            }
        }

        private IEnumerator BurstFireRoutine()
        {
            while (burstRemaining > 0 && CanFire())
            {
                yield return new WaitForSeconds(weaponData.fireRate);
                base.Fire();
                burstRemaining--;
            }
            burstRemaining = weaponData.burstCount;
        }

        protected override void HandleFiring()
        {
            base.HandleFiring();
            ApplyRecoil();
        }

        private void ApplyRecoil()
        {
            if (weaponData == null || weaponManager.CameraRootForEffect == null) return;

            float recoilMultiplier = currentStats.recoilMultiplier;
            recoilMultiplier *= isAiming ? weaponData.adsRecoilMultiplier : 1f;
            float currentRecoil = weaponData.recoilForce * recoilMultiplier;
            
            float horizontalRecoil = Random.Range(-weaponData.recoilPattern.x, weaponData.recoilPattern.x) * currentRecoil;
            float verticalRecoil = weaponData.recoilPattern.y * currentRecoil;

            Transform cameraParent = weaponManager.CameraRootForEffect;
            if (cameraParent != null)
            {
                Vector3 currentRotation = cameraParent.localEulerAngles;
                currentRotation.x = ClampRotation(currentRotation.x - verticalRecoil);
                currentRotation.y += horizontalRecoil;

                LeanTween.cancel(cameraParent.gameObject);
                
                LeanTween.rotateLocal(cameraParent.gameObject, currentRotation, 0.1f)
                        .setEaseOutQuad()
                        .setOnComplete(() =>
                        {
                            float recoveryModifier = isAiming ? 0.8f : 0.6f;
                            Vector3 recoveryRotation = new Vector3(
                                currentRotation.x + (verticalRecoil * recoveryModifier),
                                currentRotation.y - (horizontalRecoil * recoveryModifier),
                                currentRotation.z
                            );

                            LeanTween.rotateLocal(cameraParent.gameObject, recoveryRotation, 0.2f)
                                    .setEaseOutQuad();
                        });
            }
        }

        private float ClampRotation(float rotation)
        {
            if (rotation > 180) rotation -= 360;
            return Mathf.Clamp(rotation, -75f, 75f);
        }
        #endregion

        #region Fire Mode Control
        public void SetFireMode(FireMode newMode)
        {
            switch (newMode)
            {
                case FireMode.Burst when !weaponData.CanBurst:
                case FireMode.Auto when !weaponData.CanAutoFire:
                    Debug.LogWarning($"[{GetType().Name}] Attempted to set unsupported fire mode: {newMode}");
                    return;
            }

            currentFireMode = newMode;
            burstRemaining = weaponData.burstCount;
        }
        #endregion

        #region Attachment System
        public override void AttachOptic(WeaponOptic optic)
        {
            base.AttachOptic(optic);
            
            var opticModifier = new WeaponStatModifier();
            
            switch (optic)
            {
                case WeaponOptic.TelescopicScope:
                case WeaponOptic.ThermalScope:
                    opticModifier.accuracyMultiplier = 1.2f;
                    opticModifier.adsSpeedMultiplier = 0.8f;
                    break;
                    
                case WeaponOptic.RedDot:
                case WeaponOptic.Holographic:
                    opticModifier.accuracyMultiplier = 1.1f;
                    opticModifier.adsSpeedMultiplier = 1.1f;
                    break;
            }

            currentStats *= opticModifier;
        }

        public override void AttachMuzzle(WeaponMuzzle muzzle)
        {
            base.AttachMuzzle(muzzle);

            var muzzleModifier = new WeaponStatModifier();
            
            switch (muzzle)
            {
                case WeaponMuzzle.Suppressor:
                    muzzleModifier.recoilMultiplier = 0.8f;
                    muzzleModifier.damageMultiplier = 0.9f;
                    break;
                    
                case WeaponMuzzle.Compensator:
                    muzzleModifier.recoilMultiplier = 0.7f;
                    break;
                    
                case WeaponMuzzle.MuzzleBreak:
                    muzzleModifier.recoilMultiplier = 0.85f;
                    muzzleModifier.damageMultiplier = 1.1f;
                    break;
            }

            currentStats *= muzzleModifier;
        }
        #endregion
    }
}