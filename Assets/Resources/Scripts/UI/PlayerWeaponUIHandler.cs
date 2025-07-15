using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponUIHandler : MonoBehaviour
{
    #region PROPERTIES
    [Header("UI ELEMENT(s)")]
    public Image WeaponIconImg;
    public TMP_Text CurrentAmmoAmountTxt;
    public TMP_Text MaxAmmoAmountTxt;
    public WeaponIconSet[] WeaponIcons;
    public Color OutOfAmmoColor = Color.red;
    public Color NormalAmmoColor = Color.red;

    #region _custom attributes
    [System.Serializable]
    public struct WeaponIconSet
    {
        public int WeaponID;
        public Sprite WeaponIcon;
    }
    #endregion
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
        PlayerControlEventMananger.OnSwitchingWeapon.AddListener(OnSwitchWeapon);
        PlayerControlEventMananger.OnWeaponAmmoChange.AddListener(OnCurrentWeaponAmmoChanged);
    }

    private void UnRegisterAllEvents()
    {
        PlayerControlEventMananger.OnSwitchingWeapon.RemoveListener(OnSwitchWeapon);
        PlayerControlEventMananger.OnWeaponAmmoChange.RemoveListener(OnCurrentWeaponAmmoChanged);
    }

    private void OnSwitchWeapon(int weaponID, int currentAmmo, int maxAmmo)
    {
        var wpIcon = GetWeaponIcon(weaponID);
        if (wpIcon != null) WeaponIconImg.sprite = wpIcon;
        CurrentAmmoAmountTxt.text = currentAmmo.ToString();
        MaxAmmoAmountTxt.text = "/" + maxAmmo.ToString();

        CurrentAmmoAmountTxt.color = currentAmmo <= 0 ? OutOfAmmoColor : NormalAmmoColor;
    }

    private void OnCurrentWeaponAmmoChanged(int currentAmmo, int ammoLeft)
    {
        CurrentAmmoAmountTxt.text = currentAmmo.ToString();
        MaxAmmoAmountTxt.text = "/" + ammoLeft.ToString();

        CurrentAmmoAmountTxt.color = currentAmmo <= 0 ? OutOfAmmoColor : NormalAmmoColor;
    }

    #endregion

    #endregion

    #region SUPPORTIVE
    private Sprite GetWeaponIcon(int weaponID)
    {
        if (WeaponIcons.Any(x => x.WeaponID == weaponID)) 
        { 
            return WeaponIcons.FirstOrDefault(x => x.WeaponID == weaponID).WeaponIcon;
        }
        return null;
    }
    #endregion
}
