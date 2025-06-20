using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainWeatherController : MonoBehaviour
{
    #region PROPERTIES
    [Header("OBJECT(s)")]
    public Camera PlayerCamera;
    public AudioSource RainAudioSource;
    public AudioSource WindAudioSource;
    public Transform RainParticleTransform;
    public Transform RainMistParticleTransform;
    private Transform cameraTransform;

    [Header("SETTING(s)")]
    public bool FollowCamera = true;
    public int FollowAfterFrames = 10;
    [Tooltip("How far the rain particle system is ahead of the player")]
    public float RainForwardOffset = -7.0f;
    public float RainHeight = 25.0f;

    private int frameCounter = 0;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        PlayerCamera = Camera.main;
        cameraTransform = PlayerCamera.transform;
    }
    private void OnEnable()
    {
        frameCounter = 0;
        if (RainAudioSource != null) RainAudioSource.Play();
        if (WindAudioSource != null) WindAudioSource.Play();
    }
    private void Update()
    {
        if (frameCounter >= FollowAfterFrames)
        {
            if (FollowCamera)
            {
                Vector3 pos = cameraTransform.position;

                RainParticleTransform.position = pos;
                RainParticleTransform.Translate(0.0f, RainHeight, RainForwardOffset);
                RainParticleTransform.rotation = Quaternion.Euler(0.0f, cameraTransform.rotation.eulerAngles.y, 0.0f);

                RainMistParticleTransform.position = pos;
            }
            else
            {
                RainMistParticleTransform.position = RainParticleTransform.position;
            }

            frameCounter = 0;
        }
        frameCounter++;
    }
    private void OnDisable()
    {
        if (RainAudioSource != null) RainAudioSource.Stop();
        if (WindAudioSource != null) WindAudioSource.Stop();
    }
    #endregion
}
