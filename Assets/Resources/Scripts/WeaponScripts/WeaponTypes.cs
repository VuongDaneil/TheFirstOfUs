using UnityEngine;

namespace WeaponSystem
{
    public interface IWeapon
    {
        string WeaponName { get; }
        WeaponType Type { get; }
        WeaponSlot Slot { get; }
        EquipState currentEquipState { get; }

        // Core functions

        void OnEquip();
        void OnUnequip();

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
        void AttachAttachment(WeaponAttachment attachment);

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

    public enum EquipState
    {
        Equipped,
        Unequipped
    }

    //[System.Serializable]
    //public class WeaponStatModifier
    //{
    //    public float damageMultiplier = 1f;
    //    public float recoilMultiplier = 1f;
    //    public float accuracyMultiplier = 1f;
    //    public float adsSpeedMultiplier = 1f;

    //    public static WeaponStatModifier operator *(WeaponStatModifier a, WeaponStatModifier b)
    //    {
    //        return new WeaponStatModifier
    //        {
    //            damageMultiplier = a.damageMultiplier * b.damageMultiplier,
    //            recoilMultiplier = a.recoilMultiplier * b.recoilMultiplier,
    //            accuracyMultiplier = a.accuracyMultiplier * b.accuracyMultiplier,
    //            adsSpeedMultiplier = a.adsSpeedMultiplier * b.adsSpeedMultiplier
    //        };
    //    }
    //}
}