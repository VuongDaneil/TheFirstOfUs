using UnityEngine;

namespace WeaponSystem
{
    public interface IWeapon
    {
        int WeaponID { get; }
        string WeaponName { get; }
        WeaponType Type { get; }
        EquipState CurrentEquipState { get; }

        // Core functions

        void OnEquip();
        void OnUnequip();

        void Initialize(WeaponManager manager);
        void Fire();
        void Reload();
        void AimDownSight(bool isAiming, bool secondaryAim);
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

}