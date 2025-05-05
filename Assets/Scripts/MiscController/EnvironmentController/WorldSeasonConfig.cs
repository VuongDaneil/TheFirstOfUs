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
}