using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponUIHandler : MonoBehaviour
{
    #region PROPERTIES
    [Header("UI ELEMENT(s)")]
    public Image GunIcon;
    public TMP_Text CurrentAmmoAmountTxt;
    public TMP_Text MaxAmmoAmountTxt;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        RegisterAllEvents();
    }

    private void OnDestroy()
    {
        UnRegisterAllEvents();
    }
    #endregion

    #region MAIN

    #region _events
    private void RegisterAllEvents()
    {
        PlayerControlEventsMananger.OnSwitchingWeapon.AddListener(OnSwitchWeapon);
        PlayerControlEventsMananger.OnWeaponAmmoChange.AddListener(OnCurrentWeaponAmmoChanged);
    }

    private void UnRegisterAllEvents()
    {
        PlayerControlEventsMananger.OnSwitchingWeapon.RemoveListener(OnSwitchWeapon);
        PlayerControlEventsMananger.OnWeaponAmmoChange.RemoveListener(OnCurrentWeaponAmmoChanged);
    }

    private void OnSwitchWeapon(int weaponID, int currentAmmo, int maxAmmo)
    {
        CurrentAmmoAmountTxt.text = currentAmmo.ToString();
        MaxAmmoAmountTxt.text = "/" + maxAmmo.ToString();
    }

    private void OnCurrentWeaponAmmoChanged(int currentAmmo, int ammoLeft)
    {
        CurrentAmmoAmountTxt.text = currentAmmo.ToString();
        MaxAmmoAmountTxt.text = "/" + ammoLeft.ToString();
    }

    #endregion

    #endregion
}
