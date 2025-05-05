using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using WeaponSystem;

namespace WeaponSystem
{
    [RequireComponent(typeof(PlayerMovementController))]
    public class WeaponManager : MonoBehaviour
    {
        #region PROPERTIES
        [Header("OBJECT(s)")]
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private GameObject weaponCamera;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private CharacterControllerBinding controlMapping;

        public Transform CameraRootForEffect => cameraRoot;

        [Header("DEBUG(s)")]
        [ReadOnly] public bool initialized;

        // Component references
        private PlayerMovementController movementController;

        // Weapon inventory
        private Dictionary<WeaponSlot, WeaponBase> equippedWeapons;
        public WeaponBase currentWeapon;
        private WeaponSlot currentSlot;
        #endregion

        private void Awake()
        {
            equippedWeapons = new Dictionary<WeaponSlot, WeaponBase>();
            movementController = GetComponent<PlayerMovementController>();
            initialized = false;
            
            foreach (WeaponSlot slot in System.Enum.GetValues(typeof(WeaponSlot)))
            {
                equippedWeapons[slot] = null;
            }
        }

        private void Start()
        {
            if (controlMapping == null)
            {
                Debug.LogError("WeaponManager: CharacterControllerBinding is not assigned!");
                return;
            }

            if (movementController == null)
            {
                Debug.LogError("WeaponManager: PlayerMovementController is not found!");
                return;
            }
        }

        private void OnEnable()
        {
            EquipWeapon(currentWeapon, WeaponSlot.Primary);
            initialized = false;
        }

        private void Update()
        {
            if (!initialized && Input.GetKeyDown(controlMapping.StartControl))
            {
                initialized = true;
            }

            if (currentWeapon == null) return;

            HandleWeaponInput();
            HandleWeaponSwitching();
            UpdateWeaponMovementState();
        }

        private void HandleWeaponInput()
        {
            if (!initialized || currentWeapon == null) return;

            // Aiming
            if (Input.GetKey(controlMapping.AimDownSight))
                currentWeapon.AimDownSight(true);
            else if (Input.GetKeyUp(controlMapping.AimDownSight))
                currentWeapon.AimDownSight(false);

            // Shooting
            if (Input.GetKey(controlMapping.Fire))
                currentWeapon.Fire();

            // Reloading
            if (Input.GetKeyDown(controlMapping.Reload))
                currentWeapon.Reload();

            // Inspecting
            if (Input.GetKeyDown(controlMapping.Inspect))
                currentWeapon.Inspect();
        }

        private void HandleWeaponSwitching()
        {
            if (!initialized) return;

            KeyCode[] weaponSlotKeys = new KeyCode[]
            {
                controlMapping.Weapon1,
                controlMapping.Weapon2,
                controlMapping.Weapon3,
                controlMapping.Weapon4
            };

            for (int i = 0; i < weaponSlotKeys.Length; i++)
            {
                if (Input.GetKeyDown(weaponSlotKeys[i]))
                {
                    WeaponSlot targetSlot = (WeaponSlot)i;
                    if (equippedWeapons[targetSlot] != null)
                    {
                        SwitchToWeapon(targetSlot);
                    }
                }
            }
        }

        private void UpdateWeaponMovementState()
        {
            if (currentWeapon == null) return;

            bool isWalking = movementController.IsMoving && !movementController.IsRunning;
            bool isRunning = movementController.IsRunning;

            currentWeapon.UpdateMovementState(isWalking, isRunning);
        }

        public void EquipWeapon(WeaponBase weapon, WeaponSlot slot)
        {
            if (equippedWeapons[slot] != null)
            {
                UnequipWeapon(slot);
            }

            weapon.transform.SetParent(weaponHolder);
            weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            weapon.Initialize(this);

            equippedWeapons[slot] = weapon;

            if (currentWeapon == null)
            {
                SwitchToWeapon(slot);
            }
        }

        public void UnequipWeapon(WeaponSlot slot)
        {
            if (equippedWeapons[slot] == null) return;

            if (currentWeapon == equippedWeapons[slot])
            {
                currentWeapon = null;
            }

            Destroy(equippedWeapons[slot].gameObject);
            equippedWeapons[slot] = null;
        }

        public void SwitchToWeapon(WeaponSlot slot)
        {
            if (currentSlot == slot || equippedWeapons[slot] == null) return;

            if (currentWeapon != null)
            {
                currentWeapon.gameObject.SetActive(false);
                if (currentWeapon.IsAiming)
                {
                    currentWeapon.AimDownSight(false);
                }
            }

            currentWeapon = equippedWeapons[slot];
            currentSlot = slot;
            currentWeapon.gameObject.SetActive(true);
        }

        public void AttachOptic(WeaponOptic optic)
        {
            if (currentWeapon != null)
            {
                currentWeapon.AttachOptic(optic);
            }
        }

        public void AttachMuzzle(WeaponMuzzle muzzle)
        {
            if (currentWeapon != null)
            {
                currentWeapon.AttachMuzzle(muzzle);
            }
        }

        public WeaponBase GetCurrentWeapon() => currentWeapon;
        public WeaponSlot GetCurrentSlot() => currentSlot;
    }
}