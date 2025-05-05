using NaughtyAttributes;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SwayEffect : MonoBehaviour
{
    #region PROPERTIES
    [Header("CONFIGURE")]
    public bool SwayOnUpdate = false;
    [SerializeField] private bool swayRotation = false;
    [SerializeField] private bool swayPosition = false;
    [SerializeField] private bool hoveringWhenIdle = false;

    [Header("OBJECT(s)")]
    [SerializeField] private Transform swayObject;
    public Transform SwayObject => swayObject;
    Vector3 originalPos;
    Quaternion originalQuaternion;

    [Header("SWAY SETTING(s)")]
    [SerializeField] private float swaySpeed;
    [SerializeField] private float multiplier_vertical;
    [SerializeField] private float multiplier_horizontal;

    [Header("HOVER SETTING(s)")]
    [SerializeField] private bool hoverRotation = false;
    [SerializeField] private bool hoverPosition = false;
    [SerializeField][ShowIf("hoveringWhenIdle")] private float inputOffsetStartHovering = 2f;
    [SerializeField][ShowIf("hoveringWhenIdle")] private Vector2 hoveringFactor = new Vector2(2, 10);
    [SerializeField][ShowIf("hoveringWhenIdle")] private float hoveringSpeedMultiplier = 0.1f;
    [SerializeField][ShowIf("hoveringWhenIdle")] private float hoverTimerFrequently = 1;
    [SerializeField][ShowIf("hoveringWhenIdle")] private float hoverPositionOffset = 1;
    [SerializeField][ShowIf("hoveringWhenIdle")] private float IdleTimeStartHovering = 1;
    float hoveringTimer = 0;
    Vector2 currentHoverDirection;
    float hoveringSCurrentpeed = 0;
    bool allowToHover = false;
    float idleTimer = 0;

    [Header("SHAKING SETTING(s)")]
    public bool doShake = false;
    [SerializeField] private Vector2 shakeFactor = new Vector2(2, 10);
    [SerializeField] private float shakeSpeedMultiplier = 0.1f;
    [SerializeField] private float shakeTimeFrequently = 1;
    [SerializeField] private float shakeTimeTotal = 1;
    Vector2 currentShakingDirection;
    float shakeTimer = 0;
    float shakeTimerTotal = 0;
    bool shaking = false;

    [Serializable]
    public struct SwayAttributes
    {
        public bool swayRotation;
        public bool swayPosition;
        public Transform swayObject;
        public Vector3 originalPos;
        public Quaternion originalQuaternion;
        public float waySpeed;
        public float multiplier_vertical;
        public float multiplier_horizontal;

        public bool hoveringWhenIdle;
        public bool hoverRotation;
        public bool hoverPosition;
        public float inputOffsetStartHovering;
        public Vector2 hoveringFactor;
        public float hoveringSpeedMultiplier;
        public float hoverTimerFrequently;
        public float hoverPositionOffset;
        public float IdleTimeStartHovering;

        public bool doShake;
        public Vector2 shakeFactor;
        public float shakeSpeedMultiplier;
        public float shakeTimeFrequently;
        public float shakeTimeTotal;
    }

    #endregion

    #region UNITY CORE
    private void Start()
    {
        originalPos = swayObject.localPosition;
        originalQuaternion = swayObject.localRotation;
    }
    private void Update()
    {
        if (SwayOnUpdate) SwayAction();
    }
    public void SwayAction( bool noHovering = false, float speedMultiplier = 1, float amountMultiplier = 1)
    {
        var input = new Vector2
        {
            x = Input.GetAxis("Mouse X"),
            y = Input.GetAxis("Mouse Y")
        };
        float mouseX = input.x * multiplier_horizontal * amountMultiplier;
        float mouseY = input.y * multiplier_vertical * amountMultiplier;
        if (doShake && shaking)
        {
            ShakingAction();
            return;
        }

        if (hoveringWhenIdle)
        {
            if (!allowToHover)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= IdleTimeStartHovering)
                {
                    allowToHover = true;
                    idleTimer = 0;
                }
            }
            else idleTimer = 0;

            if (input.x.InRange(-inputOffsetStartHovering, inputOffsetStartHovering)
                && input.y.InRange(-inputOffsetStartHovering, inputOffsetStartHovering)
                && allowToHover && !noHovering)
            {
                SwayHovering(speedMultiplier);
            }
            else
            {
                hoveringTimer = 0;
                allowToHover = false;
                Sway(mouseX, mouseY, speedMultiplier);
            }
        }
        else
        {
            Sway(mouseX, mouseY, speedMultiplier);
        }
    }
    #endregion

    #region MAIN

    #region action(s)
    private void Sway(float _inputX, float _inputY, float _speedMultiplier = 1)
    {
        if (swayRotation)
        {
            Quaternion rotationX = Quaternion.AngleAxis(-_inputY, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(_inputX, Vector3.up);

            Quaternion targetRotation = rotationX * rotationY;
            swayObject.localRotation = Quaternion.Slerp(swayObject.localRotation, originalQuaternion * targetRotation, swaySpeed * _speedMultiplier * Time.deltaTime);
        }
        if (swayPosition)
        {
            Vector3 targetPos = new Vector3(originalPos.x - _inputX, originalPos.y - _inputY, originalPos.z);
            swayObject.localPosition = Vector3.Slerp(swayObject.localPosition, targetPos, swaySpeed * _speedMultiplier * Time.deltaTime);
        }
    }
    private void SwayHovering(float _multiplier = 1)
    {
        if (hoveringTimer <= 0)
        {
            hoveringSCurrentpeed = 0;
            currentHoverDirection = new Vector2(hoveringFactor.x, hoveringFactor.y);
            bool randomBool = Random.Range(0, 101) >= 50;
            hoveringFactor.x = randomBool ? hoveringFactor.x : -hoveringFactor.x;
            hoveringFactor.y = randomBool ? -hoveringFactor.y : hoveringFactor.y;
        }
        if (hoveringTimer < hoverTimerFrequently)
        {
            hoveringTimer += Time.deltaTime;
            if (hoverRotation)
            {
                Quaternion rotationX = Quaternion.AngleAxis(-currentHoverDirection.y * _multiplier, Vector3.right);
                Quaternion rotationY = Quaternion.AngleAxis(currentHoverDirection.x * _multiplier, Vector3.up);

                Quaternion targetRotation = rotationX * rotationY;
                swayObject.localRotation = Quaternion.Slerp(swayObject.localRotation, originalQuaternion * targetRotation, swaySpeed * hoveringSpeedMultiplier * Time.deltaTime * _multiplier);
            }
            if (hoverPosition)
            {
                hoveringSCurrentpeed += 0.025f;
                hoveringSCurrentpeed = Mathf.Clamp(hoveringSCurrentpeed, 0, swaySpeed);
                Vector3 targetPos = new Vector3(originalPos.x - currentHoverDirection.x * _multiplier, originalPos.y - currentHoverDirection.y * _multiplier, originalPos.z);
                swayObject.localPosition = Vector3.Slerp(swayObject.localPosition, targetPos, hoveringSCurrentpeed * hoveringSpeedMultiplier * Time.deltaTime * _multiplier);
                if (swayObject.localPosition.EqualsWithOffset(target: targetPos, offset: hoverPositionOffset))
                {
                    hoveringTimer = 0;
                }
            }
        }
        else
        {
            hoveringTimer = 0;
        }
    }
    public void Shake(bool _isShaking = true)
    {
        shakeTimerTotal = 0;
        shaking = _isShaking;
    }
    private void ShakingAction()
    {
        if (shakeTimer <= 0)
        {
            hoveringSCurrentpeed = 0;
            currentHoverDirection = new Vector2(shakeFactor.x, shakeFactor.y);
            shakeFactor.x = -shakeFactor.x;
            shakeFactor.y = -shakeFactor.y;
        }
        if (shakeTimerTotal <= shakeTimeTotal)
        {
            if (shakeTimer <= shakeTimeFrequently)
            {
                shakeTimerTotal += Time.deltaTime;
                shakeTimer += Time.deltaTime;

                hoveringSCurrentpeed += 0.025f;
                hoveringSCurrentpeed = Mathf.Clamp(hoveringSCurrentpeed, 0, swaySpeed);
                Vector3 targetPos = new Vector3(originalPos.x - currentHoverDirection.x, originalPos.y - currentHoverDirection.y, originalPos.z);
                swayObject.localPosition = Vector3.Slerp(swayObject.localPosition, targetPos, hoveringSCurrentpeed * shakeSpeedMultiplier * Time.deltaTime);
                if (swayObject.localPosition.EqualsWithOffset(target: targetPos, offset: hoverPositionOffset))
                {
                    shakeTimer = 0;
                }
            }
            else
            {
                shakeTimer = 0;
            }
        }
        else
        {
            shaking = false;
            shakeTimerTotal = 0;
        }
    }
    public void ResetSwayPositionAndRotation()
    {
        swayObject.localPosition = originalPos;
        swayObject.localRotation = originalQuaternion;
    }
    #endregion

    #endregion

    #region SUPPORTIVE(s)
    public void FullyInitializeSwayAttributes(SwayAttributes _attributes)
    {
        swayPosition = _attributes.swayPosition;
        swayRotation = _attributes.swayRotation;
        swayObject = _attributes.swayObject;
        originalQuaternion = _attributes.originalQuaternion;
        originalPos = _attributes.originalPos;
        swaySpeed = _attributes.waySpeed;
        multiplier_horizontal = _attributes.multiplier_horizontal;
        multiplier_vertical = _attributes.multiplier_vertical;

        hoveringWhenIdle = _attributes.hoveringWhenIdle;
        inputOffsetStartHovering = _attributes.inputOffsetStartHovering;
        hoveringFactor = _attributes.hoveringFactor;
        hoveringSpeedMultiplier = _attributes.hoveringSpeedMultiplier;
        hoverTimerFrequently = _attributes.hoverTimerFrequently;
        hoverPositionOffset = _attributes.hoverPositionOffset;
        IdleTimeStartHovering = _attributes.IdleTimeStartHovering;
        hoverRotation = _attributes.hoverRotation;
        hoverPosition = _attributes.hoverPosition;

        doShake = _attributes.doShake;
        shakeFactor = _attributes.shakeFactor;
        shakeSpeedMultiplier = _attributes.shakeSpeedMultiplier;
        shakeTimeFrequently = _attributes.shakeTimeFrequently;
        shakeTimeTotal = _attributes.shakeTimeTotal;
    }
    public void StatsOnlyInitializeSwayAttributes(SwayAttributes _attributes)
    {
        swayPosition = _attributes.swayPosition;
        swayRotation = _attributes.swayRotation;
        swaySpeed = _attributes.waySpeed;
        multiplier_horizontal = _attributes.multiplier_horizontal;
        multiplier_vertical = _attributes.multiplier_vertical;

        hoveringWhenIdle = _attributes.hoveringWhenIdle;
        inputOffsetStartHovering = _attributes.inputOffsetStartHovering;
        hoveringFactor = _attributes.hoveringFactor;
        hoveringSpeedMultiplier = _attributes.hoveringSpeedMultiplier;
        hoverTimerFrequently = _attributes.hoverTimerFrequently;
        hoverPositionOffset = _attributes.hoverPositionOffset;
        hoverRotation = _attributes.hoverRotation;
        hoverPosition = _attributes.hoverPosition;

        doShake = _attributes.doShake;
        shakeFactor = _attributes.shakeFactor;
        shakeSpeedMultiplier = _attributes.shakeSpeedMultiplier;
        shakeTimeFrequently = _attributes.shakeTimeFrequently;
        shakeTimeTotal = _attributes.shakeTimeTotal;
    }
    #endregion
}
