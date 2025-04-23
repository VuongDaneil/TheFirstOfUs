using DigitalRuby.RainMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SceneSharedAttributes;

public class ScenePersonalEnvironmentController : MonoBehaviour
{
    #region UNITY CORE
    [Header("WEATHER")]
    public Weather CurrentWeather;
    public RainScript RainController;
    public SnowWeatherController SnowController;
    public WeatherLightingSimulation StormController;

    [Header("PROP(s)")]
    public List<GameObject> Crows;
    #endregion

    #region UNITY CORE

    #endregion

    #region MAIN
    public void SetCrowsInEnvironment(bool enable)
    {
        foreach (var crow in Crows)
        {
            crow.SetActive(enable);
        }
    }
    public void SetWeather(Weather weather)
    {
        CurrentWeather = weather;
        switch (weather)
        {
            case Weather.Clear:
                RainController.SetRainIntensity(0);
                StormController.enabled = false;
                break;
            case Weather.Rain:
                RainController.SetRainIntensity(1);
                StormController.enabled = false;
                break;
            case Weather.Snow:
                SnowController.SetSnow();
                RainController.SetRainIntensity(0);
                StormController.enabled = false;
                break;
            case Weather.SnowStorm:
                SnowController.SetSnowStorm();
                RainController.SetRainIntensity(0);
                StormController.enabled = false;
                break;
            case Weather.Storm:
            RainController.SetRainIntensity(1);
                StormController.enabled = true;
                break;
        }

        //Input.GetKey(KeyCode.Keypad1)
    }
    #endregion
}