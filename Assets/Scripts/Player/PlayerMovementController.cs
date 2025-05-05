using UnityEngine;
using System.Collections;
using NaughtyAttributes;
using Unity.VisualScripting;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour, IDataPersistence
{
    #region PROPERTIES
    [Header("Object - crouch / slide")]
    public CharacterControllerBinding ControlMapping;
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
    [SerializeField] float headBobFrequency = 1.5f;             // the base speed of the head bobbing (in cycles per metre)
    [SerializeField] float headBobHeight = 0.3f;                // the height range of the head bob
    [SerializeField] float headBobSwayAngle = 0.5f;             // the angle which the head tilts to left & right during the bob cycle
    [SerializeField] float headBobSideMovement = 0.05f;         // the distance the head moves to left & right during the bob cycle
    [SerializeField] float bobHeightSpeedMultiplier = 0.3f;     // the amount the bob height increases as the character's speed increases (for a good 'run' effect compared with walking)
    [SerializeField] float bobStrideSpeedLengthen = 0.3f;       // the amount the stride lengthens based on speed (so that running isn't like a silly speedwalk!)
    [SerializeField] float jumpLandMove = 3;
    [SerializeField] float jumpLandTilt = 60;

    [Header("SOUND(s)")]
    public AudioSource FootAudioSource;
    [SerializeField] AudioClip[] footstepSounds;        // an array of footstep sounds that will be randomly selected from.
    [SerializeField] AudioClip jumpSound;               // the sound played when character leaves the ground.
    [SerializeField] AudioClip landSound;				// the sound played when character touches back on ground.

    float nextStepTime = 0.5f;                                  // the time at which the next footstep sound is due to occur
    float headBobCycle = 0;                             // the current position through the headbob cycle
    float headBobFade = 0;								// the current amount to which the head bob position is being applied or not (it is faded out when the character is not moving)

    // Fields for simple spring calculation:
    float springPos = 0;
    float springVelocity = 0;
    float springElastic = 1.1f;
    float springDampen = 0.8f;
    float springVelocityThreshold = 0.05f;
    float springPositionThreshold = 0.05f;
    Vector3 originalLocalPos;							// the original local position of this gameobject at Start

    private UnityEngine.CharacterController characterController;
    private Transform player => characterController.transform;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private float bobbingTimer = 0f;
    private float defaultCameraY;
    bool initialized = false;

    [ReadOnly] public Transform gameplayCamera;
    public Transform head;
    public Transform neck;
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
    }
    private void Start()
    {
        characterController = GetComponent<UnityEngine.CharacterController>();
    }
    private void OnEnable()
    {
        initialized = false;
    }
    private void Update()
    {
        HandleMovement();
    }
    #endregion

    #region MAIN
    void HandleMovement()
    {
        if (!initialized)
        {
            if (Input.GetKeyDown(ControlMapping.StartControl))
            {
                initialized = true;
                transform.SetParent(null);
                gameplayCamera = Camera.main.transform;
                defaultCameraY = head.localPosition.y;
                originalLocalPos = head.localPosition;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            return;
        }
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = gameplayCamera.right * moveX + gameplayCamera.forward * moveZ;

        if (TiltBobbingType) HeadBobbingWithFootstep();
        else HeadBobbing();

        PlayerSoundHandler();

        if (Input.GetKey(ControlMapping.Crouch)) ChangeStance(CharacterStanceStatus.Crouching);
        else ChangeStance(CharacterStanceStatus.Standing);

        if (moveX == 0 && moveZ == 0) { CurrentMovementStage = PlayerMovementStage.StandStill; }
        else
        {
            if (CurrentStanceStage == CharacterStanceStatus.Standing)
            {
                if (Input.GetKey(ControlMapping.Sprint))
                {
                    CurrentMovementStage = PlayerMovementStage.Sprinting;
                    characterController.Move(runSpeed * Time.deltaTime * move);
                }
                else
                {
                    CurrentMovementStage = PlayerMovementStage.Walking;
                    characterController.Move(Time.deltaTime * walkSpeed * move);
                }
            }
            else if (CurrentStanceStage == CharacterStanceStatus.Crouching)
            {
                CurrentMovementStage = PlayerMovementStage.Walking;
                characterController.Move(Time.deltaTime * walkSpeed * move);
            }
        }

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to ensure the character stays grounded
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
            currentHeadPosition = head.localPosition;
            head.localPosition = new Vector3(currentHeadPosition.x, defaultCameraY + bobbingOffset, currentHeadPosition.z);
        }
        else if (CurrentMovementStage == PlayerMovementStage.Sprinting)
        {
            bobbingTimer += Time.deltaTime * bobbingSpeed * 2;
            float bobbingOffset = Mathf.Sin(bobbingTimer) * bobbingAmount;
            currentHeadPosition = head.localPosition;
            head.localPosition = new Vector3(currentHeadPosition.x, defaultCameraY + bobbingOffset, currentHeadPosition.z);
        }
        else if (CurrentMovementStage == PlayerMovementStage.StandStill)
        {
            bobbingTimer = 0f;
        }
    }
    void HeadBobbingWithFootstep()
    {
        Vector3 velocity = (player.position - prevPosition) / Time.deltaTime;
        Vector3 velocityChange = velocity - prevVelocity;
        prevPosition = player.position;
        prevVelocity = velocity;

        // vertical head position "spring simulation" for jumping/landing impacts
        springVelocity -= velocityChange.y;                         // input to spring from change in character Y velocity
        springVelocity -= springPos * springElastic;                    // elastic spring force towards zero position
        springVelocity *= springDampen;                             // damping towards zero velocity
        springPos += springVelocity * Time.deltaTime;               // output to head Y position
        springPos = Mathf.Clamp(springPos, -.3f, .3f);          // clamp spring distance

        // snap spring values to zero if almost stopped:
        if (Mathf.Abs(springVelocity) < springVelocityThreshold && Mathf.Abs(springPos) < springPositionThreshold)
        {
            springVelocity = 0;
            springPos = 0;
        }

        // head bob cycle is based on "flat" velocity (i.e. excluding Y)
        float flatVelocity = new Vector3(velocity.x, 0, velocity.z).magnitude;

        // lengthen stride based on speed (so run bobbing isn't lots of little steps)
        float strideLengthen = 1 + (flatVelocity * bobStrideSpeedLengthen);

        // increment cycle
        headBobCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / headBobFrequency);

        // actual bobbing and swaying values calculated using Sine wave
        float bobFactor = Mathf.Sin(headBobCycle * Mathf.PI * 2);
        float bobSwayFactor = Mathf.Sin(headBobCycle * Mathf.PI * 2 + Mathf.PI * .5f); // sway is offset along the sin curve by a quarter-turn in radians
        bobFactor = 1 - (bobFactor * .5f + 1); // bob value is brought into 0-1 range and inverted
        bobFactor *= bobFactor; // bob value is biased towards 0

        // fade head bob effect to zero if not moving
        if (new Vector3(velocity.x, 0, velocity.z).magnitude < 0.1f)
        {
            headBobFade = Mathf.Lerp(headBobFade, 0, Time.deltaTime);
        }
        else
        {
            headBobFade = Mathf.Lerp(headBobFade, 1, Time.deltaTime);
        }

        // height of bob is exaggerated based on speed
        float speedHeightFactor = 1 + (flatVelocity * bobHeightSpeedMultiplier);

        // finally, set the position and rotation values
        float xPos = -headBobSideMovement * bobSwayFactor;
        float yPos = springPos * jumpLandMove + bobFactor * headBobHeight * headBobFade * speedHeightFactor;
        float xTilt = -springPos * jumpLandTilt;
        float zTilt = bobSwayFactor * headBobSwayAngle * headBobFade;
        neck.SetLocalPositionAndRotation(originalLocalPos + new Vector3(xPos, yPos, 0), Quaternion.Euler(xTilt, 0, zTilt));
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

                // head bob cycle is based on "flat" velocity (i.e. excluding Y)
                float flatVelocity = new Vector3(velocity.x, 0, velocity.z).magnitude;

                // lengthen stride based on speed (so run bobbing isn't lots of little steps)
                float strideLengthen = 1 + (flatVelocity * bobStrideSpeedLengthen);
                headBobCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / headBobFrequency);
                if (headBobCycle > nextStepTime)
                {
                    // time for next footstep sound:

                    nextStepTime = headBobCycle + .5f;

                    // pick & play a random footstep sound from the array,
                    // excluding sound at index 0
                    int n = Random.Range(1, footstepSounds.Length);
                    FootAudioSource.clip = footstepSounds[n];
                    FootAudioSource.Play();

                    // move picked sound to index 0 so it's not picked next time
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
    #endregion

    #region SAVE GAME SYSTEM
    public void LoadData(GameData data)
    {
        thisTransform.position = data.PlayerSavedData.PlayerPosition;

    }

    public void SaveData(ref GameData data)
    {
        data.PlayerSavedData.PlayerPosition = thisTransform.position;
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