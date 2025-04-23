using UnityEngine;
using WeaponSystem;

namespace WeaponSystem
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Weapon Info")]
        public string weaponName;
        public WeaponType weaponType;
        public WeaponSlot weaponSlot;

        [Header("Base Stats")]
        public float damage = 10f;
        public float fireRate = 0.1f;
        public int magazineSize = 30;

        [Header("Accuracy & Recoil")]
        public float baseAccuracy = 0.9f;
        public float adsAccuracyMultiplier = 1.5f;
        public float recoilForce = 2f;
        public float adsRecoilMultiplier = 0.7f;
        public Vector2 recoilPattern = new Vector2(1f, 2f);

        [Header("Position Settings")]
        public Vector3 hipFirePosition = new Vector3(0.2f, -0.1f, 0.4f);
        public Vector3 hipFireRotation = Vector3.zero;
        public Vector3 adsPosition = new Vector3(0f, -0.05f, 0.2f);
        public Vector3 adsRotation = Vector3.zero;

        [Header("Movement Settings")]
        public float movementTransitionSpeed = 0.2f;

        [Header("Animation States")]
        [Tooltip("Names of animation states in the Animator")]
        public string idleState = "Idle";
        public string walkState = "Walk";
        public string runState = "Run";
        public string inspectState = "Inspect";
        public string inspectIdleState = "Inspect_Idle";
        public string attackState = "Attack";
        public string reloadState = "Reload";
        public string switchInState = "Switch_In";
        public string switchOutState = "Switch_Out";

        [Header("Animation Settings")]
        public float defaultTransitionDuration = 0.2f;
        public float attackTransitionDuration = 0.1f;
        public float attackAnimationDuration = 0.1f;
        public float movementTransitionDuration = 0.15f;
        public float inspectTransitionDuration = 0.25f;
        public float switchTransitionDuration = 0.15f;

        [Header("FOV Settings")]
        public float defaultFOV = 60f;
        public float aimingFOV = 40f;
        public float fovTransitionSpeed = 0.3f;

        [Header("Rifle Settings")]
        public FireMode defaultFireMode = FireMode.Single;
        public int burstCount = 3;
        public bool hasAutoFire = true;

        [Header("Attachments")]
        public Vector3 opticMountPoint = new Vector3(0f, 0.1f, 0.05f);
        public Vector3 muzzleMountPoint = new Vector3(0f, 0f, 0.5f);

        // Weapon type specific getters
        public bool IsRifle => weaponType == WeaponType.Rifle;
        public bool IsMachineGun => weaponType == WeaponType.MachineGun;
        public bool CanBurst => IsRifle || IsMachineGun;
        public bool CanAutoFire => hasAutoFire && (IsRifle || IsMachineGun);
    }
}
