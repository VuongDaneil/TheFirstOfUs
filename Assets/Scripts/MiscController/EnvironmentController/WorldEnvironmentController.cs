using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using static SceneSharedAttributes;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WorldEnvironmentController : MonoBehaviour, IDataPersistence
{
    #region PROPERTIES
    [Header("CONTROLLER(s)")]
    public WeatherController WorldWeather;
    public GameDebugOptions DebugOptions;
    public Light SunAndMoonLightSource;

    [Header("SEASON CONFIG(s)")]
    public WorldSeasonConfig SpringSeasonConfig;
    public WorldSeasonConfig SummerSeasonConfig;
    public WorldSeasonConfig WinterSeasonConfig;

    [Header("WEATHER(s)")]
    public List<WeatherComponents> WeatherComponents = new List<WeatherComponents>();

    [Header("DEBUG(s)")]
    public bool PauseTime = false;
    [ReadOnly] public Weather CurrentWeather;
    [ReadOnly] public WorldSeasonConfig CurrentSeasonConfig;
    public WorldSeason CurrentSeason => CurrentSeasonConfig == null ? WorldSeason.SPRING : CurrentSeasonConfig.Season;
    [ReadOnly] public int DayCount = 0;
    [ReadOnly] public TimeSpan DayTime = new TimeSpan(0, 0, 0);
    [ReadOnly] public DayPart CurrentDayPart = DayPart.Evening;
    [SerializeField] private CharacterControllerBinding controlMapping;

    UnityEvent OnHourPassed = new UnityEvent();
    UnityEvent OnDayPassed = new UnityEvent();
    UnityEvent OnSeasonPassed = new UnityEvent();

    private float realSecondLengthAsIngameSecond = 1;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        ValidateCurrentSeason();
        ValidateDayPart();
        ApplyDaypartLightingSettingInstantly(CurrentDayPart);
    }
    private void Update()
    {
        if (Input.GetKeyDown(controlMapping.ChangeDayPartKey))
        {
            ApplyNextDaypartLightingSettingSmoothly();
        }
        if (!PauseTime) { UpdateDayTime(); }
    }
    #endregion

    #region MAIN

    #region TIME SYSTEM
    private void UpdateDayTime()
    {
        realSecondLengthAsIngameSecond = 3600f / CurrentSeasonConfig.DayTimeConfig.HourInSecond;

        DayTime += TimeSpan.FromSeconds(Time.deltaTime * realSecondLengthAsIngameSecond);

        if (DayTime.Hours >= 24)
        {
            DayCount++;
            DayTime = new TimeSpan(0, 0, 0);
            ValidateCurrentSeason();
            OnDayPassed?.Invoke();
        }

        if (DayTime.Minutes == 0)
        {
            ValidateDayPart();
            OnHourPassed?.Invoke();
        }
    }

    #endregion

    #endregion

    #region SUPPORTIVE
    private void ValidateDayPart()
    {
        var dayTimeCycleConfig = CurrentSeasonConfig.DayTimeConfig;
        int currentHour = DayTime.Hours;
        DayPart toCheckDaypart = CurrentDayPart;

        if (currentHour >= dayTimeCycleConfig.Dawn.x && currentHour <= dayTimeCycleConfig.Dawn.y) toCheckDaypart = DayPart.Dawn;
        else if (currentHour >= dayTimeCycleConfig.Morning.x && currentHour <= dayTimeCycleConfig.Morning.y) toCheckDaypart = DayPart.Morning;
        else if (currentHour >= dayTimeCycleConfig.Afternoon.x && currentHour <= dayTimeCycleConfig.Afternoon.y) toCheckDaypart = DayPart.Afternoon;
        else if (currentHour >= dayTimeCycleConfig.Evening.x && currentHour <= dayTimeCycleConfig.Evening.y) toCheckDaypart = DayPart.Evening;
        else if (currentHour >= dayTimeCycleConfig.Night.x && currentHour <= dayTimeCycleConfig.Night.y) toCheckDaypart = DayPart.Night;
        else if (currentHour >= dayTimeCycleConfig.Midnight.x && currentHour <= dayTimeCycleConfig.Midnight.y) toCheckDaypart = DayPart.Midnight;

        if (CurrentDayPart != toCheckDaypart)
        {
            CurrentDayPart = toCheckDaypart;
            ApplyDaypartLightingSetting(CurrentDayPart);
        }
    }
    private void ValidateCurrentSeason()
    {
        int daysPerYear = SpringSeasonConfig.DaysInSeason + SummerSeasonConfig.DaysInSeason + WinterSeasonConfig.DaysInSeason;
        int daysInCurrentYear = DayCount % daysPerYear;
        WorldSeasonConfig nextSeason;
        if (daysInCurrentYear < SpringSeasonConfig.DaysInSeason)
        {
            nextSeason = SpringSeasonConfig;
        }
        else if (daysInCurrentYear < (SpringSeasonConfig.DaysInSeason + SummerSeasonConfig.DaysInSeason))
        {
            nextSeason = SummerSeasonConfig;
        }
        else
        {
            nextSeason = WinterSeasonConfig;
        }

        if (nextSeason != CurrentSeasonConfig)
        {
            CurrentSeasonConfig = nextSeason;
            OnSeasonPassed?.Invoke();
        }

        realSecondLengthAsIngameSecond = 3600f / CurrentSeasonConfig.DayTimeConfig.HourInSecond;
    }
    private void ApplyDaypartLightingSetting(DayPart daypart)
    {
        switch (daypart)
        {
            case DayPart.Dawn:
                CurrentSeasonConfig.DawnLightingPreset.ApplySmoothly();
                break;
            case DayPart.Morning:
                CurrentSeasonConfig.MorningLightingPreset.ApplySmoothly();
                break;
            case DayPart.Afternoon:
                CurrentSeasonConfig.AfternoonLightingPreset.ApplySmoothly();
                break;
            case DayPart.Evening:
                CurrentSeasonConfig.EveningLightingPreset.ApplySmoothly();
                break;
            case DayPart.Night:
                CurrentSeasonConfig.NightLightingPreset.ApplySmoothly();
                break;
            case DayPart.Midnight:
                CurrentSeasonConfig.MidnightLightingPreset.ApplySmoothly();
                break;
        }
    }
    private void ApplyDaypartLightingSettingInstantly(DayPart daypart)
    {
        switch (daypart)
        {
            case DayPart.Dawn:
                CurrentSeasonConfig.DawnLightingPreset.Apply();
                break;
            case DayPart.Morning:
                CurrentSeasonConfig.MorningLightingPreset.Apply();
                break;
            case DayPart.Afternoon:
                CurrentSeasonConfig.AfternoonLightingPreset.Apply();
                break;
            case DayPart.Evening:
                CurrentSeasonConfig.EveningLightingPreset.Apply();
                break;
            case DayPart.Night:
                CurrentSeasonConfig.NightLightingPreset.Apply();
                break;
            case DayPart.Midnight:
                CurrentSeasonConfig.MidnightLightingPreset.Apply();
                break;
        }
    }
    private void ApplyNextDaypartLightingSettingSmoothly()
    {
        switch (CurrentDayPart)
        {
            case DayPart.Dawn:
                CurrentDayPart = DayPart.Morning;
                CurrentSeasonConfig.MorningLightingPreset.ApplySmoothly();
                break;
            case DayPart.Morning:
                CurrentDayPart = DayPart.Afternoon;
                CurrentSeasonConfig.AfternoonLightingPreset.ApplySmoothly();
                break;
            case DayPart.Afternoon:
                CurrentDayPart = DayPart.Evening;
                CurrentSeasonConfig.EveningLightingPreset.ApplySmoothly();
                break;
            case DayPart.Evening:
                CurrentDayPart = DayPart.Night;
                CurrentSeasonConfig.NightLightingPreset.ApplySmoothly();
                break;
            case DayPart.Night:
                CurrentDayPart = DayPart.Midnight;
                CurrentSeasonConfig.MidnightLightingPreset.ApplySmoothly();
                break;
            case DayPart.Midnight:
                CurrentDayPart = DayPart.Dawn;
                CurrentSeasonConfig.DawnLightingPreset.ApplySmoothly();
                break;
        }
    }
    #endregion

    #region SAVE GAME SYSTEM
    public void LoadData(GameData data)
    {
        DayCount = data.WorldSavedData.DayCount;
        DayTime = new TimeSpan(data.WorldSavedData.DayTimeHour, data.WorldSavedData.DayTimeMinute, 0);
        ValidateCurrentSeason();
        ValidateDayPart();
    }

    public void SaveData(ref GameData data)
    {
        data.WorldSavedData.DayCount = DayCount;
        data.WorldSavedData.DayTimeHour = DayTime.Hours;
        data.WorldSavedData.DayTimeMinute = DayTime.Minutes;
    }
    #endregion

#if UNITY_EDITOR
    void OnGUI()
    {
        if (DebugOptions == null || !DebugOptions.DateTimeDebug) return;
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 25;
        style.normal.textColor = Color.green;

        GUI.Label(new Rect(10, style.fontSize, 500, style.fontSize + 5), "SEASON: " + CurrentSeason.ToString(), style);
        GUI.Label(new Rect(10, style.fontSize * 2, 500, style.fontSize + 5), "DAY: " + DayCount + " - TIME: " + DayTime.Hours.ToString("00") + ":" + DayTime.Minutes.ToString("00") + " - " + CurrentDayPart.ToString(), style);
        GUI.Label(new Rect(10, style.fontSize * 3, 500, style.fontSize + 5), "WEATHER: " + CurrentWeather.ToString(), style);
    }
#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(WorldEnvironmentController))]
public class WorldEnvironmentControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WorldEnvironmentController controller = (WorldEnvironmentController)target;

        DrawDefaultInspector();
        GUILayout.Space(15);
        GUILayout.Button("_DEBUG_");
        GUILayout.Label("SEASON: "  + controller.CurrentSeason.ToString());
        GUILayout.Label("DAY: "     + controller.DayCount + " - TIME: " + controller.DayTime.Hours + ":" + controller.DayTime.Minutes + " - " + controller.CurrentDayPart.ToString());
        GUILayout.Label("WEATHER: " + controller.CurrentWeather.ToString());
    }
}
#endif
