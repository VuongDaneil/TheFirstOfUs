using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

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
        Overcast,
        Snowing,
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

    #region types
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
        public List<GameObject> GameobjectToActive;
        public List<ParticleSystem> WeatherEffects;
        public UnityEvent OnThisWeatherAppear;
        public UnityEvent OnThisWeatherGone;

        public void Start()
        {
            foreach (var obj in GameobjectToActive) obj.SetActive(true);
            foreach (var effect in WeatherEffects) effect.Play();
            OnThisWeatherAppear?.Invoke();
        }

        public void Stop()
        {
            foreach (var obj in GameobjectToActive) obj.SetActive(false);
            foreach (var effect in WeatherEffects) effect.Stop();
            OnThisWeatherGone?.Invoke();
        }
    }

    [System.Serializable]
    public class SceneLightingSettingPreset
    {
        [Header("ENVIRONMENT")]
        public bool OverrideSkybox = true;
        public Material SkyBox;
        public Color SkyBoxColor;
        public Color RealtimeShadowColor;

        [HorizontalLine(2, EColor.Blue)]
        [Header("SUN/MOON LIGHT SOURCE")]
        public bool OverrideWorldLight = true;
        [Label("Color")] public Color WorldLightColor = Color.white;
        [Label("Intensity")] public float WorldLightIntensity = 1f;

        [HorizontalLine(2, EColor.Blue)]
        [Header("ENVIRONMENT LIGHTING")]
        public bool OverrideEnvironmentLight = true;
        [Label("Source")] public AmbientMode AmbientMode = AmbientMode.Skybox;
        private bool isColorMode => AmbientMode == AmbientMode.Flat || AmbientMode == AmbientMode.Trilight;

        // ___ SKYBOX MODE ___
        [ShowIf("AmbientMode", AmbientMode.Skybox)][Label("Intensity Multiplier")][Range(0f, 8f)] public float Intensity = 0.25f;

        // ___ COLOR MODE ___
        [ShowIf("AmbientMode", AmbientMode.Flat)][Label("Ambient Color")] public Color AmbientColor;

        // ___ GRADIENT MODE ___
        [ShowIf("AmbientMode", AmbientMode.Trilight)][ColorUsage(true, true)][Label("Sky Color")] public Color SkyColor;
        [ShowIf("AmbientMode", AmbientMode.Trilight)][ColorUsage(true, true)][Label("Equator Color")] public Color EquatorColor;
        [ShowIf("AmbientMode", AmbientMode.Trilight)][ColorUsage(true, true)][Label("Ground Color")] public Color GroundColor;

        [HorizontalLine(2, EColor.Blue)]
        [Header("FOG")]
        public bool OverrideFog = true;
        public bool FogEnabled;
        [ShowIf("FogEnabled")][Label("Color")] public Color FogColor;
        [ShowIf("FogEnabled")][Label("Mode")] public FogMode FogMode = FogMode.Linear;
        [ShowIf("FogEnabled")][Label("Start Distance")] public float StartDistance = 0f;
        [ShowIf("FogEnabled")][Label("End Distance")] public float EndDistance = 300f;
        [ShowIf("FogEnabled")][Label("Intensity")] public float FogIntensity;
    }

    [System.Serializable]
    public class WeatherInProgressing
    {
        public Weather Weather;
        public int Duration;
        public float ElapsedTime;
        public WeatherInProgressing(Weather weather, int duration)
        {
            ElapsedTime = 0;
            Weather = weather;
            Duration = duration;
        }

        public bool IsFinished() => ElapsedTime >= Duration;
        public void Update(int frameGap = 1)
        {
            if (!IsFinished()) ElapsedTime += Time.deltaTime * frameGap;
        }
    }
    #endregion
}