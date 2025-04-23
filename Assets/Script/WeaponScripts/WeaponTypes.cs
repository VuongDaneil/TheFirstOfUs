using UnityEngine;

namespace WeaponSystem
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }

    public interface IWeapon
    {
        string WeaponName { get; }
        WeaponType Type { get; }
        WeaponSlot Slot { get; }

        // Core functions
        void Initialize(WeaponManager manager);
        void Fire();
        void Reload();
        void AimDownSight(bool isAiming);
        void Inspect();
        
        // State checks
        bool CanFire();
        bool CanReload();
        bool IsReloading { get; }
        bool IsAiming { get; }

        // Movement state
        void UpdateMovementState(bool walking, bool running);

        // Attachment handling
        void AttachOptic(WeaponOptic optic);
        void AttachMuzzle(WeaponMuzzle muzzle);

        // Animation callbacks
        void OnReloadAnimationComplete();
        void OnInspectAnimationComplete();

        // Sound management
        AudioClip GetWeaponSound(WeaponSoundType type);
    }

    public enum WeaponType
    {
        MachineGun,
        Rifle,
        Shotgun
    }

    public enum WeaponSlot
    {
        Primary,
        Secondary,
        Melee,
        Throwable
    }

    public enum WeaponOptic
    {
        IronSight,
        RedDot,
        Holographic,
        TelescopicScope,
        ThermalScope
    }

    public enum WeaponMuzzle
    {
        None,
        Suppressor,
        FlashHider,
        Compensator,
        MuzzleBreak
    }

    public enum FireMode
    {
        Single,
        Burst,
        Auto
    }

    public enum WeaponSoundType
    {
        MagazineOut,
        MagazineIn,
        BoltPull,
        BoltRelease,
        Fire,
        Empty
    }

    [System.Serializable]
    public class WeaponStatModifier
    {
        public float damageMultiplier = 1f;
        public float recoilMultiplier = 1f;
        public float accuracyMultiplier = 1f;
        public float adsSpeedMultiplier = 1f;

        public static WeaponStatModifier operator *(WeaponStatModifier a, WeaponStatModifier b)
        {
            return new WeaponStatModifier
            {
                damageMultiplier = a.damageMultiplier * b.damageMultiplier,
                recoilMultiplier = a.recoilMultiplier * b.recoilMultiplier,
                accuracyMultiplier = a.accuracyMultiplier * b.accuracyMultiplier,
                adsSpeedMultiplier = a.adsSpeedMultiplier * b.adsSpeedMultiplier
            };
        }
    }
}