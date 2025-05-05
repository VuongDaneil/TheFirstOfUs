using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SceneSharedAttributes;

[CreateAssetMenu(fileName = "WeatherPossibilityConfig", menuName = "SceneSettingAsset/WeatherPossibilityConfig")]
public class WeatherPossibilityConfig : ScriptableObject
{
    public List<WeatherChances> WeathersPossibility = new List<WeatherChances>();

    /// <summary>
    /// using Weighted Random Algorithm
    /// </summary>
    /// <returns></returns>
    public Weather ChooseWeatherByChances()
    {
        int totalPossibility = 0;
        foreach (var weather in WeathersPossibility) totalPossibility += weather.Possibility;

        int randomValue = Random.Range(0, totalPossibility);
        int cursor = 0;

        for (int i = 0; i < 10; i++)
        {
            cursor += WeathersPossibility[i].Possibility;
            if (cursor >= randomValue) return WeathersPossibility[i].Weather;
        }

        return Weather.Storm;
    }
}