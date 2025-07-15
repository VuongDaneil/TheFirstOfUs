using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using WeaponSystem;
using Unity.VisualScripting;

namespace WeaponSystem
{
    [RequireComponent(typeof(PlayerMovementController))]
    public class WeaponManager : MonoBehaviour
    {
        #region PROPERTIES
        [ReadOnly] public bool CurrentWeaponReady = false;

        [Header("OBJECT(s)")]
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private GameObject weaponCamera;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private CharacterControllerBinding controlMapping;

        public Transform CameraRootForEffect => cameraRoot;

        [Header("DEBUG(s)")]
        [ReadOnly] public bool initialized;
        private PlayerMovementController movementController;
        public WeaponBase currentWeapon;
        private WeaponSlot currentSlot;
        private Coroutine SwitchWeaponCoroutine;

        [Header("WEAPONS")]
        public Transform WeaponsContainer;
        [ReadOnly] public WeaponBase MainWeapon;
        [ReadOnly] public WeaponBase SubWeapon;
        [ReadOnly] public WeaponBase MeleeWeapon;

        [Header("WEAPON PREFABS")]
        public WeaponBase MainWeaponPrefab;
        public WeaponBase SubWeaponPrefab;
        public WeaponBase MeleeWeaponPrefab;
        #endregion

        #region UNITY CORE
        private void Awake()
        {
            movementController = GetComponent<PlayerMovementController>();
            initialized = false;
            RegisterEvents();
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
            InitializeWeapons();
        }

        private void Update()
        {
            if (currentWeapon == null || !PlayerBrain.Instance.IsAlive) return;

            HandleWeaponInput();
            HandleInteractInput();
            HandleWeaponSwitching();
            UpdateWeaponMovementState();
        }
        private void OnDestroy()
        {
            UnRegisterEvents();
        }
        #endregion

        #region MAIN

        #region _events
        private void RegisterEvents()
        {
            PlayerControlEventMananger.OnPlayerDie.AddListener(OnPlayerDie);
            PlayerControlEventMananger.OnWeaponSwitchInDone.AddListener(OnCurrentWeaponSwitchedIn);
            PlayerControlEventMananger.OnWeaponSwitchOutDone.AddListener(OnPreviousWeaponSwitchedOut);
        }

        private void UnRegisterEvents()
        {
            PlayerControlEventMananger.OnPlayerDie.RemoveListener(OnPlayerDie);
            PlayerControlEventMananger.OnWeaponSwitchInDone.RemoveListener(OnCurrentWeaponSwitchedIn);
            PlayerControlEventMananger.OnWeaponSwitchOutDone.RemoveListener(OnPreviousWeaponSwitchedOut);
        }

        private void OnCurrentWeaponSwitchedIn()
        {
            CurrentWeaponReady = true;
        }
        private void OnPreviousWeaponSwitchedOut()
        {
            currentWeapon.gameObject.SetActive(true);
            currentWeapon.transform.SetParent(weaponHolder);
            currentWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            currentWeapon.OnSwitchIn();
        }
        private void OnPlayerDie()
        {
            currentWeapon?.OnUnequip();
        }
        #endregion

        private void HandleInteractInput()
        {
            if (!initialized || Time.timeScale == 0) return;
            if (Input.GetKeyDown(controlMapping.Interact))
            {
                if (Physics.Raycast(weaponCamera.transform.position, weaponCamera.transform.forward, out RaycastHit hit, 3f))
                {
                    if (hit.collider.TryGetComponent(out IQuestObject interactable))
                    {
                        interactable.OnPlayerInteract();
                    }
                }
            }
        }

        private void HandleWeaponInput()
        {
            if (!initialized || !CurrentWeaponReady || currentWeapon == null || Time.timeScale == 0) return;

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

            if (Input.GetKeyDown(controlMapping.Weapon1)) EquipWeapon(MainWeapon);
            if (Input.GetKeyDown(controlMapping.Weapon2)) EquipWeapon(SubWeapon);
        }

        private void UpdateWeaponMovementState()
        {
            if (currentWeapon == null || !CurrentWeaponReady) return;

            bool isWalking = movementController.IsMoving && !movementController.IsRunning;
            bool isRunning = movementController.IsRunning && PlayerBrain.Instance.CanSprint();

            currentWeapon.UpdateMovementState(isWalking, isRunning);
        }

        public void EquipWeapon(WeaponBase weapon, bool immediately = false)
        {
            if (currentWeapon != null) UnequipWeapon(currentWeapon);

            currentWeapon = weapon;
            PlayerControlEventMananger.OnSwitchingWeapon?.Invoke(weapon.WeaponID, weapon.CurrentAmmo, weapon.CurrentAmmoCapacity);

            if (immediately)
            {
                OnPreviousWeaponSwitchedOut();
            }
        }

        public void UnequipWeapon(WeaponBase weapon)
        {
            if (weapon == null) return;

            if (currentWeapon == weapon)
            {
                currentWeapon.OnUnequip();
                CurrentWeaponReady = false;
                currentWeapon = null;
            }
        }

        public void AttachAttachment(WeaponAttachment attachment)
        {
            if (currentWeapon != null)
            {
                currentWeapon.AttachAttachment(attachment);
            }
        }
        #endregion

        public WeaponBase GetCurrentWeapon() => currentWeapon;
        public WeaponSlot GetCurrentSlot() => currentSlot;

        #region SUPPORTIVE
        private void InitializeWeapons()
        {
            MainWeapon = GameObject.Instantiate(MainWeaponPrefab, WeaponsContainer);
            SubWeapon = GameObject.Instantiate(SubWeaponPrefab, WeaponsContainer);

            SubWeapon.Initialize(this);
            MainWeapon.Initialize(this);

            MainWeapon.gameObject.SetActive(false);
            SubWeapon.gameObject.SetActive(false);
            MainWeapon.ApplyDefaultPosition();
            SubWeapon.ApplyDefaultPosition();

            SubWeapon.OnUnequip();
            EquipWeapon(MainWeapon, immediately: true);

            initialized = true;
        }
        #endregion
    }
}