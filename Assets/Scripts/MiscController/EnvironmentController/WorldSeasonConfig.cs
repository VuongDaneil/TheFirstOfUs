using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SceneSharedAttributes;

[CreateAssetMenu(fileName = "WorldSeasonConfig", menuName = "SceneSettingAsset/WorldSeasonConfig")]
public class WorldSeasonConfig : ScriptableObject
{
    #region PROPERTIES
    [Header("SEASON")]
    public WorldSeason Season;
    public TimeCycleConfig DayTimeConfig;
    public int DaysInSeason = 7;

    [Header("DAY PART(s)")]
    public SceneLightingPreset DawnLightingPreset;
    public SceneLightingPreset MorningLightingPreset;
    public SceneLightingPreset AfternoonLightingPreset;
    public SceneLightingPreset EveningLightingPreset;
    public SceneLightingPreset NightLightingPreset;
    public SceneLightingPreset MidnightLightingPreset;
    public SceneLightingPreset DefaultLightingPreset;

    [Header("WEATHER")]
    public Weather DefaultWeather;
    public WeatherPossibilityConfig WeatherPossibilities;
    [Space]
    public Vector2 ClearDuration        = new Vector2(1, 3);
    public Vector2 FoggyDuration        = new Vector2(1, 3);
    public Vector2 SunnyDuration        = new Vector2(1, 3);
    public Vector2 OvercastDuration     = new Vector2(1, 3);
    public Vector2 SnowDuration         = new Vector2(1, 3);
    public Vector2 SnowStormDuration    = new Vector2(1, 3);
    public Vector2 LightRainDuration    = new Vector2(1, 3);
    public Vector2 MediumRainDuration   = new Vector2(1, 3);
    public Vector2 HeavyRainDuration    = new Vector2(1, 3);
    public Vector2 StormDuration        = new Vector2(1, 3);
    #endregion

    #region MAIN
    public void SetNewWeather(out Weather newWeather, out int duration)
    {
        newWeather = WeatherPossibilities.ChooseWeatherByChances();
        duration = 0;
        switch (newWeather)
        {
            case Weather.Clear:
                duration = (int)Random.Range(ClearDuration.x, ClearDuration.y);
                break;
            case Weather.Foggy:
                duration = (int)Random.Range(FoggyDuration.x, FoggyDuration.y);
                break;
            case Weather.Overcast:
                duration = (int)Random.Range(OvercastDuration.x, OvercastDuration.y);
                break;
            case Weather.Snowing:
                duration = (int)Random.Range(SnowDuration.x, SnowDuration.y);
                break;
            case Weather.LightRain:
                duration = (int)Random.Range(LightRainDuration.x, LightRainDuration.y);
                break;
            case Weather.MediumRain:
                duration = (int)Random.Range(MediumRainDuration.x, MediumRainDuration.y);
                break;
            case Weather.HeavyRain:
                duration = (int)Random.Range(HeavyRainDuration.x, HeavyRainDuration.y);
                break;
            case Weather.Storm:
                duration = (int)Random.Range(StormDuration.x, StormDuration.y);
                break;
        }
    }
    #endregion
}