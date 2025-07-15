using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class PlayerControlEventMananger
{
    #region _player status
    public static UnityEvent OnPlayerDie = new UnityEvent();
    /// <summary>
    /// On player's HP change
    /// 1st float - current HP
    /// 2nd float - max HP
    /// bool - Healing or not
    /// </summary>
    public static UnityEvent<float, float, bool> OnPlayerHealthChanged = new UnityEvent<float, float, bool>();
    /// <summary>
    /// Same as OnPlayerHealthChanged
    /// </summary>
    public static UnityEvent<float, float> OnPlayerStaminaChanged = new UnityEvent<float, float>();
    #endregion

    #region _player movement events
    public static UnityEvent<PlayerMovementStage> OnPlayerChangeMovementState = new UnityEvent<PlayerMovementStage>();
    public static UnityEvent<float> OnPlayerSteering = new UnityEvent<float>();
    #endregion

    #region _weapon controller events
    /// <summary>
    /// bullet direction
    /// </summary>
    public static UnityEvent<Vector3> OnWeaponShootDirection = new UnityEvent<Vector3>();
    public static UnityEvent<int, int> OnWeaponAmmoChange = new UnityEvent<int, int>();

    /// <summary>
    /// recoil value
    /// </summary>
    public static UnityEvent<float> OnRecoilAfterShoot = new UnityEvent<float>();
    public static UnityEvent OnWeaponReloadDone = new UnityEvent();

    /// <summary>
    /// On player switch weapon
    /// 1st int - Weapon Id
    /// 2nd int - Current Ammo
    /// 3rd int - Max Ammo
    /// </summary>
    public static UnityEvent<int, int, int> OnSwitchingWeapon = new UnityEvent<int, int, int>();
    public static UnityEvent OnWeaponSwitchInDone = new UnityEvent();
    public static UnityEvent OnWeaponSwitchOutDone = new UnityEvent();
    #endregion
}
