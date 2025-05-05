using UnityEngine;

[CreateAssetMenu(fileName = "CharacterControllerBinding", menuName = "Config/Character Controller Binding")]
public class CharacterControllerBinding : ScriptableObject
{
    [Header("Movement Controls")]
    public KeyCode Sprint = KeyCode.LeftShift;
    public KeyCode Crouch = KeyCode.LeftControl;
    public KeyCode Jump = KeyCode.Space;
    public KeyCode StartControl = KeyCode.F;

    [Header("Weapon Controls")]
    public KeyCode Fire = KeyCode.Mouse0;
    public KeyCode AimDownSight = KeyCode.Mouse1;
    public KeyCode Reload = KeyCode.R;
    public KeyCode Inspect = KeyCode.I;

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

    // Add default preset
    public void Reset()
    {
        // Movement
        Sprint = KeyCode.LeftShift;
        Crouch = KeyCode.LeftControl;
        Jump = KeyCode.Space;
        StartControl = KeyCode.F;

        // Weapon core
        Fire = KeyCode.Mouse0;
        AimDownSight = KeyCode.Mouse1;
        Reload = KeyCode.R;
        Inspect = KeyCode.I;

        // Weapon slots
        Weapon1 = KeyCode.Alpha1;
        Weapon2 = KeyCode.Alpha2;
        Weapon3 = KeyCode.Alpha3;
        Weapon4 = KeyCode.Alpha4;

        // Quick actions
        QuickMelee = KeyCode.V;
        QuickThrow = KeyCode.G;
        WeaponSwap = KeyCode.Q;
    }
}