using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using WeaponSystem;

namespace WeaponSystem
{
    public class RifleWeapon : WeaponBase
    {
        #region PROPERTIES
        [Header("RIFLE STATE")]
        private FireMode currentFireMode;
        private int burstRemaining;
        private bool isFiring;

        [Header("EFFECT(s)")]
        public List<ParticleSystem> BulletStrailEffects = new List<ParticleSystem>();
        public List<ParticleSystem> OnFireParticalEffectsLoop = new List<ParticleSystem>();
        public List<ParticleSystem> OnFireParticalEffectsNoLoop = new List<ParticleSystem>();
        public Transform BulletStrailRoot;
        #endregion

        #region Weapon Setup
        protected override void Awake()
        {
            base.Awake();
            ValidateWeaponType();
            burstRemaining = weaponData.burstCount;
        }

        protected override void Update()
        {
            base.Update();
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
            PlayFireEffects();
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

        protected override void HandleFiring(out Vector3 bulletDirection)
        {
            base.HandleFiring(out bulletDirection);
            BulletStrailRoot.rotation = Quaternion.LookRotation(bulletDirection);
            ApplyRecoil();
        }

        private void ApplyRecoil()
        {
            if (weaponData == null) return;

            float recoilMultiplier = weaponData.baseRecoil;
            recoilMultiplier *= isAiming ? weaponData.adsRecoilMultiplier : 1f;
            float currentRecoil = weaponData.recoilForce * recoilMultiplier;
            
            float horizontalRecoil = Random.Range(-weaponData.recoilPattern.x, weaponData.recoilPattern.x) * currentRecoil;
            float verticalRecoil = weaponData.recoilPattern.y * currentRecoil;

            PlayerControlEventsMananger.OnRecoilAfterShoot?.Invoke(verticalRecoil);
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

        #endregion

        #region SUPPORTIVE
        public void PlayFireEffects()
        {
            foreach (var effect in BulletStrailEffects)
            {
                effect.Stop();
                effect.Play();
            }
            foreach (var effect in OnFireParticalEffectsLoop)
            {
                effect.Stop();
                effect.Play();
            }
            foreach (var effect in OnFireParticalEffectsNoLoop)
            {
                effect.Play();
            }
        }

        [Button("PREVIEW HIP POS")]
        public void ApplyHipPosition()
        {
            weaponModel.localPosition = weaponData.hipFirePosition;
            weaponModel.localRotation = Quaternion.Euler(weaponData.hipFirePosition);
        }

        [Button("PREVIEW ADS POS")]
        public void ApplyAimingPosition()
        {
            weaponModel.localPosition = weaponData.adsPosition;
            weaponModel.localRotation = Quaternion.Euler(weaponData.adsRotation);
        }
        #endregion
    }
}