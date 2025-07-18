using static GameConstant;
using NaughtyAttributes;
using UnityEngine;

public class PlayerLookController : MonoBehaviour
{
    #region PROPERTIES
    [Header("STAT(s)")]
    public int FOV = 70;
    public float Sensitivity = 100f;
    public Vector2 VerticalAngleClamp = new Vector2(-75f, 75f);

    [Header("LOOK AROUND - OBJECT(s)")]
    public Camera PlayerCamera;
    public Transform PlayerHead;

    [Header("TILT BODY - OBJECT(s)")]
    public Transform PlayerChest;
    public float TiltBodyValue = 35f;
    public float TiltBodyOnSteerValue = 35f;
    public float TiltBodySpeed = 50f;

    [Header("DEBUG")]
    public CharacterControllerBinding ControlMapping;
    public bool Controllable = true;
    private float xLookRotation = 0f;
    private float yLookRotation = 0f;
    private float zTiltRotation = 0f;
    private float zTiltOnSteerRotation = 0f;

    #endregion

    #region UNITY CORE
    private void Awake()
    {
        Cursor.visible = false;
        if (PlayerCamera == null) PlayerCamera = Camera.main;
        PlayerCamera.fieldOfView = FOV;

        UIEventManager.OnSettingSaved.AddListener(LoadSetting);
        PlayerControlEventMananger.OnRecoilAfterShoot.AddListener(OnRecoil);
        PlayerControlEventMananger.OnPlayerSteering.AddListener(OnPlayerSteering);

        LoadSetting();
    }

    private void Update()
    {
        if (!PlayerBrain.Instance.IsAlive || !PlayerBrain.Instance.IsReady) return;
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        CalculateTiltBodyValues();

        if (Controllable)
        {
            float mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;

            yLookRotation += mouseX;
            xLookRotation -= mouseY;
            xLookRotation = Mathf.Clamp(xLookRotation, VerticalAngleClamp.x, VerticalAngleClamp.y);

            Quaternion targetRotation = Quaternion.Euler(xLookRotation, yLookRotation, 0f);
            PlayerHead.localRotation = Quaternion.Lerp(PlayerHead.localRotation, targetRotation, Time.deltaTime * 50f);

            Quaternion chestTargetRotation = Quaternion.Euler(0f, 0, zTiltRotation);
            PlayerChest.localRotation = Quaternion.Lerp(PlayerChest.localRotation, chestTargetRotation, Time.deltaTime * TiltBodySpeed);
        }
    }

    private void OnDestroy()
    {
        UIEventManager.OnSettingSaved.RemoveListener(LoadSetting);
        PlayerControlEventMananger.OnRecoilAfterShoot.RemoveListener(OnRecoil);
        PlayerControlEventMananger.OnPlayerSteering.RemoveListener(OnPlayerSteering);
    }
    #endregion

    #region MAIN
    private void OnRecoil(float recoilXFactor)
    {
        xLookRotation -= recoilXFactor;
    }

    private void OnPlayerSteering(float steerValue)
    {
        zTiltOnSteerRotation = -steerValue * TiltBodyOnSteerValue;
    }

    private void CalculateTiltBodyValues()
    {
        if (Input.GetKey(ControlMapping.LeanRight)) zTiltRotation = -TiltBodyValue;
        else if (Input.GetKey(ControlMapping.LeanLeft)) zTiltRotation = TiltBodyValue;
        else
        {
            if (PlayerBrain.Instance.CurrentMovementState != PlayerMovementStage.StandStill)
            {
                zTiltRotation = zTiltOnSteerRotation;
            }
            else zTiltRotation = 0;
        }
    }

    private void LoadSetting()
    {
        Sensitivity = PlayerPrefs.GetFloat(SensitivityKey, Sensitivity);
    }
    #endregion
}