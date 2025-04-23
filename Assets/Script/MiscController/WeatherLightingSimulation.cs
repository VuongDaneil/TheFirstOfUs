using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherLightingSimulation : MonoBehaviour
{
    #region PROPERTIES
    public LightObjectHandler LightingLight;

    [Header("STATS")]
    public int MaxFlashingTimes = 4;
    public float MinimumTimeBetweenEachLighting = 5;
    float timer = 0;
    float nextLightingIn = 3;

    [Header("SOUND(s)")]
    public AudioSource ThunderAudioSource;
    public List<AudioClip> ThunderAudioFx;
    #endregion

    #region UNITY CORE
    private void OnEnable()
    {
        timer = 0;
        LightingLight.DisableLight();
        nextLightingIn = Random.Range(3f, MinimumTimeBetweenEachLighting + 1f);
    }

    private void Update()
    {
        if(timer > nextLightingIn)
        {
            timer = 0;
            ThunderAudioSource.PlayOneShot(ThunderAudioFx.GetRandom());
            LightingLight.LightFlashLoop(Random.Range(1, MaxFlashingTimes + 1));
            nextLightingIn = Random.Range(MinimumTimeBetweenEachLighting, MinimumTimeBetweenEachLighting * 2);
        }

        timer += Time.deltaTime;
    }

    private void OnDisable()
    {
        LightingLight.DisableLight();
    }

    #endregion
}