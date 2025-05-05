using UnityEngine;

public interface IWeapon
{
    // Basic weapon properties
    string WeaponName { get; }
    WeaponType Type { get; }
    WeaponSlot Slot { get; }

    // Core functions
    void Initialize();
    void Fire();
    void Reload();
    void AimDownSight(bool isAiming);
    void Inspect();
    
    // State checks
    bool CanFire();
    bool CanReload();
    bool IsReloading { get; }
    bool IsAiming { get; }

    // Attachment handling
    void AttachOptic(WeaponOptic optic);
    void AttachMuzzle(WeaponMuzzle muzzle);

    // Animation callbacks
    void OnReloadAnimationComplete();
    void OnInspectAnimationComplete();

    // Sound management
}