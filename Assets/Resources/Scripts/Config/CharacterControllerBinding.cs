using static GameConstant;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterControllerBinding", menuName = "Config/Character Controller Binding")]
public class CharacterControllerBinding : ScriptableObject
{
    [Header("Movement Controls")]
    public KeyCode MoveForward = KeyCode.W;
    public KeyCode MoveBackward = KeyCode.S;
    public KeyCode MoveLeft = KeyCode.A;
    public KeyCode MoveRight = KeyCode.D;
    public KeyCode Sprint = KeyCode.LeftShift;
    public KeyCode Crouch = KeyCode.LeftControl;
    public KeyCode Jump = KeyCode.Space;
    public KeyCode StartControl = KeyCode.F;
    public KeyCode LeanRight = KeyCode.E;
    public KeyCode LeanLeft = KeyCode.Q;

    [Header("Weapon Controls")]
    public KeyCode MainWeapon = KeyCode.Alpha1;
    public KeyCode SubWeapon = KeyCode.Alpha2;
    public KeyCode Fire = KeyCode.Mouse0;
    public KeyCode AimDownSight = KeyCode.Mouse1;
    public KeyCode Reload = KeyCode.R;
    public KeyCode Inspect = KeyCode.I;
    public KeyCode Interact = KeyCode.F;

    [Header("Weapon Slots")]
    public KeyCode Weapon1 = KeyCode.Alpha1;
    public KeyCode Weapon2 = KeyCode.Alpha2;
    public KeyCode Weapon3 = KeyCode.Alpha3;
    public KeyCode Weapon4 = KeyCode.Alpha4;

    [Header("Quick Actions")]
    public KeyCode QuickMelee = KeyCode.V;
    public KeyCode QuickThrow = KeyCode.G;
    public KeyCode WeaponSwap = KeyCode.Q;

    [Header("SAVE GAME")]
    public KeyCode SaveGameKey = KeyCode.F5;
    public KeyCode LoadGameKey = KeyCode.F6;
    public KeyCode ResetSaveFileKey = KeyCode.F7;

    [Header("DEBUG")]
    public KeyCode ChangeDayPartKey = KeyCode.K;
    public KeyCode ChangeWeatherKey = KeyCode.K;

    public void Reset()
    {
        MoveForward = KeyCode.W;
        MoveBackward = KeyCode.S;
        MoveLeft = KeyCode.A;
        MoveRight = KeyCode.D;
        Sprint = KeyCode.LeftShift;
        Crouch = KeyCode.LeftControl;
        Jump = KeyCode.Space;
        StartControl = KeyCode.F;

        Fire = KeyCode.Mouse0;
        AimDownSight = KeyCode.Mouse1;
        Reload = KeyCode.R;
        Inspect = KeyCode.I;

        Weapon1 = KeyCode.Alpha1;
        Weapon2 = KeyCode.Alpha2;
        Weapon3 = KeyCode.Alpha3;
        Weapon4 = KeyCode.Alpha4;

        QuickMelee = KeyCode.V;
        QuickThrow = KeyCode.G;
        WeaponSwap = KeyCode.Q;
    }

    public void LoadFromPlayerpref()
    {
        MoveForward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(MoveForwardInputKey, MoveForward.ToString()));
        MoveBackward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(MoveBackwardInputKey, MoveBackward.ToString()));
        MoveLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(MoveLeftInputKey, MoveLeft.ToString()));
        MoveRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(MoveRightInputKey, MoveRight.ToString()));
        LeanLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(LeanLeftInputKey, LeanLeft.ToString()));
        LeanRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(LeanRightInputKey, LeanRight.ToString()));
        Reload = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(ReloadInputKey, Reload.ToString()));
        MainWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(MainWeaponInputKey, MainWeapon.ToString()));
        SubWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(SubWeaponInputKey, SubWeapon.ToString()));
    }

    public void SaveToPlayerPref()
    {
        PlayerPrefs.SetString(MoveForwardInputKey, MoveForward.ToString());
        PlayerPrefs.SetString(MoveBackwardInputKey, MoveBackward.ToString());
        PlayerPrefs.SetString(MoveLeftInputKey, MoveLeft.ToString());
        PlayerPrefs.SetString(MoveRightInputKey, MoveRight.ToString());
        PlayerPrefs.SetString(LeanLeftInputKey, LeanLeft.ToString());
        PlayerPrefs.SetString(LeanRightInputKey, LeanRight.ToString());
        PlayerPrefs.SetString(ReloadInputKey, Reload.ToString());
        PlayerPrefs.SetString(MainWeaponInputKey, MainWeapon.ToString());
        PlayerPrefs.SetString(SubWeaponInputKey, SubWeapon.ToString());
    }
}