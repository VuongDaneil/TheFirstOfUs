using DigitalRuby.RainMaker;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneSharedAttributes;

public class WeatherController : MonoBehaviour
{
    #region PROPERTIES
    [Header("WEATHER")]
    public Weather CurrentWeather;

    [Header("WEATHER COMPONENT(s)")]
    [Label("CLEAR")]        public WeatherComponents CLearComponents;
    [Label("FOGGY")]        public WeatherComponents FoggyComponents;
    [Label("OVERCAST")]     public WeatherComponents OvercastComponents;
    [Label("SNOWING")]      public WeatherComponents SnowComponents;
    [Label("LIGHT RAIN")]   public WeatherComponents LightRainComponents;
    [Label("MEDIUM RAIN")]  public WeatherComponents MediumRainComponents;
    [Label("HEAVY RAIN")]   public WeatherComponents HeavyRainComponents;
    [Label("STORM")]        public WeatherComponents StormComponents;
    [ReadOnly]              public WeatherComponents CurrentWeatherComponents;

    [Header("PROP(s)")]
    public List<GameObject> Crows;
    #endregion

    #region MAIN
    public void SetWeather(Weather weather)
    {
        CurrentWeather = weather;
        if (CurrentWeatherComponents != null) CurrentWeatherComponents.Stop();

        switch (weather)
        {
            case Weather.Clear:
                CurrentWeatherComponents = null; // No components for clear weather
                break;
            case Weather.Foggy:
                CurrentWeatherComponents = FoggyComponents;
                break;
            case Weather.Overcast:
                CurrentWeatherComponents = OvercastComponents;
                break;
            case Weather.Snowing:
                CurrentWeatherComponents = SnowComponents;
                break;
            case Weather.LightRain:
                CurrentWeatherComponents = LightRainComponents;
                break;
            case Weather.MediumRain:
                CurrentWeatherComponents = MediumRainComponents;
                break;
            case Weather.HeavyRain:
                CurrentWeatherComponents = HeavyRainComponents;
                break;
            case Weather.Storm:
                CurrentWeatherComponents = StormComponents;
                break;
        }

        if (CurrentWeatherComponents != null)
        {
            CurrentWeatherComponents.Start();
        }
    }
    public void SetCrowsInEnvironment(bool enable)
    {
        foreach (var crow in Crows)
        {
            crow.SetActive(enable);
        }
    }

    public void SetWeatherOnPlayerFirstPlay()
    {
        CurrentWeather = Weather.Storm;
        SetWeather(CurrentWeather);
    }

    public void SetRandomWeather()
    {
        Weather[] weathers = (Weather[])Enum.GetValues(typeof(Weather));
        int randomIndex = UnityEngine.Random.Range(0, weathers.Length);
        SetWeather(weathers[randomIndex]);
    }
    #endregion
}