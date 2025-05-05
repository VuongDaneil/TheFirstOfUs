using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class SceneSharedAttributes
{
    #region custom attributes
    public enum WorldSeason
    {
        SPRING,
        SUMMER,
        WINTER
    }
    public enum Weather
    {
        Clear,
        Foggy,
        Sunny,
        Overcast,
        Snow,
        SnowStorm,
        LightRain,
        MediumRain,
        HeavyRain,
        Storm,
    }
    public enum DayPart
    {
        Dawn,
        Morning,
        Afternoon,
        Evening,
        Night,
        Midnight
    }
    #endregion

    #region structs
    [System.Serializable]
    public struct WeatherChances
    {
        public Weather Weather;
        [Range(0, 100)] public int Possibility;
    }

    [System.Serializable]
    public class WeatherComponents
    {
        public Weather Weather;
        public SceneLightingPreset LightingPreset;
        public List<GameObject> GameobjectToActive;
        public List<ParticleSystem> WeatherEffects;
        public UnityEvent OnThisWeatherAppear;
        public UnityEvent OnThisWeatherGone;

        public void ActiveThisWeather()
        {
            foreach (var obj in GameobjectToActive) obj.SetActive(true);
            foreach (var effect in WeatherEffects) effect.Play();
            OnThisWeatherAppear?.Invoke();
        }

        public void DeactiveThisWeather()
        {
            foreach (var obj in GameobjectToActive) obj.SetActive(false);
            foreach (var effect in WeatherEffects) effect.Stop();
            OnThisWeatherGone?.Invoke();
        }
    }
    #endregion
}