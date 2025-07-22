using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    #region PROPERTIES
    public bool Controllable = true;

    [Header("Object - crouch / slide")]
    public CharacterControllerBinding ControlMapping;
    public Transform CharacterMainRoot;
    public Transform CharacterDynamicBody;
    public float CrouchHeightOffset = 1;
    public float SlideHeightOffset = 1;
    private Vector3 characterHeightStanding = Vector3.zero;
    private Vector3 characterHeightCrouching = Vector3.zero;
    private Vector3 characterHeightSliding = Vector3.zero;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.8f;

    [Header("Headbobbing Settings")]
    public bool TiltBobbingType = false;
    public float bobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    [SerializeField] float headBobFrequency = 1.5f;
    [SerializeField] float headBobHeight = 0.3f;
    [SerializeField] float headBobSwayAngle = 0.5f;
    [SerializeField] float headBobSideMovement = 0.05f;
    [SerializeField] float bobHeightSpeedMultiplier = 0.3f;
    [SerializeField] float bobStrideSpeedLengthen = 0.3f;
    [SerializeField] float jumpLandMove = 3;
    [SerializeField] float jumpLandTilt = 60;

    [Header("SOUND(s)")]
    public AudioSource FootAudioSource;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;

    float nextStepTime = 0.5f;
    float headBobCycle = 0;
    float headBobFade = 0;

    // Fields for simple spring calculation:
    float springPos = 0;
    float springVelocity = 0;
    float springElastic = 1.1f;
    float springDampen = 0.8f;
    float springVelocityThreshold = 0.05f;
    float springPositionThreshold = 0.05f;
    Vector3 originalLocalPos;

    private UnityEngine.CharacterController characterController;
    private Transform player => characterController.transform;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private float bobbingTimer = 0f;
    private float defaultCameraY;
    bool initialized = false;

    [ReadOnly] public Transform gameplayCamera;
    public Transform Head;
    public Transform Neck;
    private Transform thisTransform;

    [Header("DEBUG")]
    public KeyCode RunKey;
    public PlayerMovementStage CurrentMovementStage = PlayerMovementStage.StandStill;
    public CharacterStanceStatus CurrentStanceStage = CharacterStanceStatus.Standing;
    Vector3 currentHeadPosition = Vector3.zero;

    Vector3 prevPosition;                               // the position from last frame
    Vector3 prevVelocity = Vector3.zero;                // the velocity from last frame
    bool prevGrounded = true;							// whether the character was grounded last frame


    public bool IsMoving => CurrentMovementStage == PlayerMovementStage.Walking;
    public bool IsRunning => CurrentMovementStage == PlayerMovementStage.Sprinting;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        thisTransform = transform;
        characterHeightStanding = CharacterDynamicBody.localPosition;
        characterHeightCrouching = characterHeightStanding - new Vector3(0, CrouchHeightOffset, 0);
        characterHeightSliding = characterHeightStanding - new Vector3(0, SlideHeightOffset, 0);
        UIEventManager.OnQuitToMainMenu.AddListener(OnQuitToMainMenu);
        PlayerControlEventMananger.OnPlayerReady.AddListener(OnPLayerReady);
    }
    private void Start()
    {
        characterController = GetComponent<UnityEngine.CharacterController>();
    }
    private void OnEnable()
    {
        if (!initialized)
        {
            initialized = true;
            transform.SetParent(null);
            gameplayCamera = Camera.main.transform;
            defaultCameraY = Head.localPosition.y;
            originalLocalPos = Head.localPosition;
            return;
        }
    }
    private void Update()
    {
        if (!Controllable || !PlayerBrain.Instance.IsAlive || !PlayerBrain.Instance.IsReady) return;
        HandleMovement();

        if (player.position.y <= -10) player.position = new Vector3(player.position.x, 10, player.position.y);
    }
    private void OnDestroy()
    {
        UIEventManager.OnQuitToMainMenu.RemoveListener(OnQuitToMainMenu);
        PlayerControlEventMananger.OnPlayerReady.RemoveListener(OnPLayerReady);
    }
    #endregion

    #region MAIN
    private void OnPLayerReady()
    {
        CharacterMainRoot.DOLocalMove(Vector3.zero, 0.5f);
        CharacterMainRoot.DOLocalRotate(Vector3.zero, 0.5f);
    }
    void HandleMovement()
    {

        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(ControlMapping.MoveForward)) moveZ += 1;
        if (Input.GetKey(ControlMapping.MoveBackward)) moveZ -= 1;
        if (Input.GetKey(ControlMapping.MoveRight)) moveX += 1;
        if (Input.GetKey(ControlMapping.MoveLeft)) moveX -= 1;


        Vector3 move = gameplayCamera.right * moveX + gameplayCamera.forward * moveZ;

        if (TiltBobbingType) HeadBobbingWithFootstep();
        else HeadBobbing();

        PlayerSoundHandler();

        if (Input.GetKey(ControlMapping.Crouch)) ChangeStance(CharacterStanceStatus.Crouching);
        else ChangeStance(CharacterStanceStatus.Standing);

        if (moveX == 0 && moveZ == 0) { SetCurrentMovementState(PlayerMovementStage.StandStill); }
        else
        {
            if (CurrentStanceStage == CharacterStanceStatus.Standing)
            {
                if (Input.GetKey(ControlMapping.Sprint) && PlayerBrain.Instance.CanSprint())
                {
                    SetCurrentMovementState(PlayerMovementStage.Sprinting);
                    characterController.Move(runSpeed * Time.deltaTime * move);
                }
                else
                {
                    SetCurrentMovementState(PlayerMovementStage.Walking);
                    characterController.Move(Time.deltaTime * walkSpeed * move);
                }
            }
            else if (CurrentStanceStage == CharacterStanceStatus.Crouching)
            {
                SetCurrentMovementState(PlayerMovementStage.Walking);
                characterController.Move(Time.deltaTime * walkSpeed * move);
            }

            PlayerControlEventMananger.OnPlayerSteering?.Invoke(moveX);
        }

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(ControlMapping.Jump))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }
    void HeadBobbing()
    {
        if (CurrentMovementStage == PlayerMovementStage.Walking)
        {
            bobbingTimer += Time.deltaTime * bobbingSpeed;
            float bobbingOffset = Mathf.Sin(bobbingTimer) * bobbingAmount;
            currentHeadPosition = Head.localPosition;
            Head.localPosition = new Vector3(currentHeadPosition.x, defaultCameraY + bobbingOffset, currentHeadPosition.z);
        }
        else if (CurrentMovementStage == PlayerMovementStage.Sprinting)
        {
            bobbingTimer += Time.deltaTime * bobbingSpeed * 2;
            float bobbingOffset = Mathf.Sin(bobbingTimer) * bobbingAmount;
            currentHeadPosition = Head.localPosition;
            Head.localPosition = new Vector3(currentHeadPosition.x, defaultCameraY + bobbingOffset, currentHeadPosition.z);
        }
        else if (CurrentMovementStage == PlayerMovementStage.StandStill)
        {
            bobbingTimer = 0f;
        }
    }
    void HeadBobbingWithFootstep()
    {
        if (Time.timeScale < 0) return;
        try
        {
            Vector3 velocity = (player.position - prevPosition) / Time.deltaTime;
            Vector3 velocityChange = velocity - prevVelocity;
            prevPosition = player.position;
            prevVelocity = velocity;

            if (double.IsNaN(springPos)) springPos = 0;
            if (double.IsNaN(springVelocity)) springVelocity = 0;
            springVelocity -= velocityChange.y;
            springVelocity -= springPos * springElastic;
            springVelocity *= springDampen;
            springPos += springVelocity * Time.deltaTime;
            springPos = Mathf.Clamp(springPos, -.3f, .3f);

            if (Mathf.Abs(springVelocity) < springVelocityThreshold && Mathf.Abs(springPos) < springPositionThreshold)
            {
                springVelocity = 0;
                springPos = 0;
            }

            float flatVelocity = new Vector3(velocity.x, 0, velocity.z).magnitude;

            float strideLengthen = 1 + (flatVelocity * bobStrideSpeedLengthen);

            headBobCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / headBobFrequency);

            float bobFactor = Mathf.Sin(headBobCycle * Mathf.PI * 2);
            float bobSwayFactor = Mathf.Sin(headBobCycle * Mathf.PI * 2 + Mathf.PI * .5f); 
            bobFactor = 1 - (bobFactor * .5f + 1);
            bobFactor *= bobFactor;

            if (new Vector3(velocity.x, 0, velocity.z).magnitude < 0.1f)
            {
                headBobFade = Mathf.Lerp(headBobFade, 0, Time.deltaTime);
            }
            else
            {
                headBobFade = Mathf.Lerp(headBobFade, 1, Time.deltaTime);
            }
            float speedHeightFactor = 1 + (flatVelocity * bobHeightSpeedMultiplier);

            float xPos = -headBobSideMovement * bobSwayFactor;
            float yPos = springPos * jumpLandMove + bobFactor * headBobHeight * headBobFade * speedHeightFactor;
            float xTilt = -springPos * jumpLandTilt;
            float zTilt = bobSwayFactor * headBobSwayAngle * headBobFade;
            if (double.IsNaN(xPos) || double.IsNaN(yPos)) return;
            Neck.localPosition = originalLocalPos + new Vector3(xPos, yPos, 0);
            Neck.localRotation = Quaternion.Euler(xTilt, 0, zTilt);
        }
        catch
        {

        }
    }
    private void PlayerSoundHandler()
    {
        if (characterController.isGrounded)
        {
            if (!prevGrounded)
            {
                FootAudioSource.clip = landSound;
                FootAudioSource.Play();
                nextStepTime = headBobCycle + .5f;

            }
            else
            {
                if (float.IsNaN(headBobCycle)) headBobCycle = 0;
                if (float.IsNaN(nextStepTime)) nextStepTime = 0;

                Vector3 velocity = (player.position - prevPosition) / Time.deltaTime;
                prevPosition = player.position;

                float flatVelocity = new Vector3(velocity.x, 0, velocity.z).magnitude;

                float strideLengthen = 1 + (flatVelocity * bobStrideSpeedLengthen);
                headBobCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / headBobFrequency);
                if (headBobCycle > nextStepTime)
                {
                    nextStepTime = headBobCycle + .5f;

                    int n = Random.Range(1, footstepSounds.Length);
                    FootAudioSource.clip = footstepSounds[n];
                    FootAudioSource.Play();

                    footstepSounds[n] = footstepSounds[0];
                    footstepSounds[0] = FootAudioSource.clip;

                }
            }
            prevGrounded = true;

        }
        else
        {

            if (prevGrounded && FootAudioSource != null)
            {
                FootAudioSource.clip = jumpSound;
                FootAudioSource.Play();
            }
            prevGrounded = false;
        }
    }

    private void OnQuitToMainMenu() => initialized = false;
    #endregion

    #region SUPPORTIVE
    public void DisableMovement() => initialized = false;

    private void ChangeStance(CharacterStanceStatus stance)
    {
        if (CurrentStanceStage == stance) return;
        switch (stance)
        {
            case CharacterStanceStatus.Standing:
                CurrentStanceStage = CharacterStanceStatus.Standing;
                LeanTween.moveLocal(CharacterDynamicBody.gameObject, characterHeightStanding, 0.25f);
                break;
            case CharacterStanceStatus.Crouching:
                CurrentStanceStage = CharacterStanceStatus.Crouching;
                LeanTween.moveLocal(CharacterDynamicBody.gameObject, characterHeightCrouching, 0.25f);
                break;
            case CharacterStanceStatus.Sliding:
                CurrentStanceStage = CharacterStanceStatus.Sliding;
                LeanTween.moveLocal(CharacterDynamicBody.gameObject, characterHeightSliding, 0.25f);
                break;
        }
    }

    private void SetCurrentMovementState(PlayerMovementStage stage)
    {
        bool changed = CurrentMovementStage != stage;
        CurrentMovementStage = stage;

        switch (CurrentMovementStage)
        {
            case PlayerMovementStage.StandStill:
                PlayerBrain.Instance?.RecoverStamina(2f);
                break;
            case PlayerMovementStage.Walking:
                PlayerBrain.Instance?.RecoverStamina();
                break;
            case PlayerMovementStage.Sprinting:
                PlayerBrain.Instance?.ConsumeStamina();
                break;
        }

        if (changed) PlayerControlEventMananger.OnPlayerChangeMovementState?.Invoke(stage);
    }
    #endregion

}
public enum PlayerMovementStage
{
    StandStill,
    Walking,
    Sprinting
}

public enum CharacterStanceStatus
{
    Standing,
    Crouching,
    Sliding,
}