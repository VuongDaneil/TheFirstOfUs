using UnityEngine;

// These types need to be accessible without namespace to avoid breaking existing code
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