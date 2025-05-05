using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowWeatherController : MonoBehaviour
{
    #region PROPERTIES
    public ParticleSystem SnowWeather;
    public ParticleSystem SnowStormWeather;

    public AudioSource WindAudioSource;
    public float SnowWindAudioVolume;
    public float SnowStormWindAudioVolume;

    Transform _transform;
    int frameCounter = 0;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        frameCounter = 0;
        _transform = transform;
    }

    private void Update()
    {
        frameCounter++;
        if (frameCounter >= 10)
        {
            _transform.position = Camera.main.transform.position;
            frameCounter = 0;
        }
    }
    #endregion

    #region MAIN
    public void SetSnow()
    {
        SnowWeather.Play();
        WindAudioSource.volume = SnowWindAudioVolume;
        WindAudioSource.Play();
    }

    public void SetSnowStorm()
    {
        SnowStormWeather.Play();
        WindAudioSource.volume = SnowStormWindAudioVolume;
        WindAudioSource.Play();
    }
    #endregion
}
