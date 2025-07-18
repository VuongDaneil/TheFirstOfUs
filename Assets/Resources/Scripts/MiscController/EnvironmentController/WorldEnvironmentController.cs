using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using static SceneSharedAttributes;
using UnityEngine.Events;
using static VExtension;
using UnityEditor.iOS.Xcode;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class WorldEnvironmentController : MonoBehaviour, IDataPersistence
{
    #region PROPERTIES
    [Header("CONTROLLER(s)")]
    public WeatherController WorldWeatherController;
    public GameDebugOptions DebugOptions;
    public Light SunAndMoonLightSource;
    [SerializeField] private CharacterControllerBinding controlMapping;

    [Header("SEASON CONFIG(s)")]
    public WorldSeasonConfig SpringSeasonConfig;
    public WorldSeasonConfig SummerSeasonConfig;
    public WorldSeasonConfig WinterSeasonConfig;

    [Header("STAT(s)")]
    public int UpdateWeatherEveryFrames = 60;
    private int weatherFrameCounter = 0;

    [Header("DAY PARTS DEBUG(s)")]
    public bool PauseTime = false;
    [ReadOnly] public int CurrentDayCount = 1;
    [ReadOnly] public TimeSpan DayTime = new TimeSpan(0, 0, 0);
    [ReadOnly] public DayPart CurrentDayPart = DayPart.Evening;
    [ReadOnly] public WorldSeasonConfig CurrentSeasonConfig;

    [Header("WEATHER DEBUG(s)")]
    public bool DebugTime;
    [ReadOnly] public WeatherInProgressing CurrentWeatherProgress;

    public Weather CurrentWeather => WorldWeatherController.CurrentWeather;
    public WorldSeason CurrentSeason => CurrentSeasonConfig == null ? WorldSeason.SPRING : CurrentSeasonConfig.Season;

    UnityEvent OnDayPassed = new UnityEvent();
    UnityEvent OnHourPassed = new UnityEvent();
    UnityEvent OnSeasonPassed = new UnityEvent();

    private float realSecondLengthAsIngameSecond = 1;
    #endregion

    #region UNITY CORE
    private void Start()
    {
        if (DataPersistenceManager.Instance.IsNewGameProgress)
        {
            WorldWeatherController.SetWeatherOnPlayerFirstPlay();
            CurrentWeatherProgress = new WeatherInProgressing(WorldWeatherController.CurrentWeather, 12 * CurrentSeasonConfig.DayTimeConfig.HourInSecond);
        }
        else
        {
            WorldWeatherController.SetRandomWeather();
            CurrentWeatherProgress = new WeatherInProgressing(WorldWeatherController.CurrentWeather, 4 * CurrentSeasonConfig.DayTimeConfig.HourInSecond);
        }
        ValidateWorld();
    }
    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(controlMapping.ChangeDayPartKey))
        {
            ApplyNextDaypartLightingSettingSmoothly();
        }
        if (Input.GetKeyDown(controlMapping.ChangeWeatherKey))
        {
            SetRandomWeather();
        }
#endif

        if (!PauseTime) 
        { 
            UpdateDayTime();
            UpdateWeather();
        }
    }
    #endregion

    #region MAIN

    #region _time system
    private void UpdateDayTime()
    {
        realSecondLengthAsIngameSecond = 3600f / CurrentSeasonConfig.DayTimeConfig.HourInSecond;

        DayTime += TimeSpan.FromSeconds(Time.deltaTime * realSecondLengthAsIngameSecond);

        if (DayTime.Hours >= 24)
        {
            CurrentDayCount++;
            DayTime = new TimeSpan(0, 0, 0);
            ValidateCurrentSeason();
            OnDayPassed?.Invoke();
        }

        if (DayTime.Minutes == 0)
        {
            ValidateDayPart();
            OnHourPassed?.Invoke();
            GameplayEventManager.OnAnHourPassed?.Invoke(CurrentDayCount, CurrentDayPart);
        }
    }

    private void UpdateWeather()
    {
        weatherFrameCounter++;
        if (weatherFrameCounter >= UpdateWeatherEveryFrames)
        {
            bool allowedToChangeWeather = CurrentWeatherProgress == null || (CurrentWeatherProgress != null && CurrentWeatherProgress.IsFinished());
            if (allowedToChangeWeather)
            {
                CurrentSeasonConfig.SetNewWeather(out Weather nextWeather, out int duration);
                if (nextWeather != CurrentWeather && duration > 1)
                {
                    ChangeWeather(nextWeather, duration);
                }
            } 
            else CurrentWeatherProgress?.Update(UpdateWeatherEveryFrames);

            weatherFrameCounter = 0;
        }
    }

    private void SetRandomWeather()
    {
        Weather randomWeather = GetRandomEnumValue<Weather>();
        ChangeWeather(randomWeather, UnityEngine.Random.Range(1, 5));
    }

    private void ChangeWeather(Weather weather, int duration)
    {
        ApplyDaypartWeatherLighting(weather);
        WorldWeatherController.SetWeather(weather);
        CurrentWeatherProgress = new WeatherInProgressing(weather, duration * CurrentSeasonConfig.DayTimeConfig.HourInSecond);
    }

    #endregion

    #region _on time passes
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
        int daysInCurrentYear = CurrentDayCount % daysPerYear;
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
    #endregion

    #endregion

    #region SUPPORTIVE

    private void ApplyDaypartLightingSetting(DayPart daypart)
    {
        switch (daypart)
        {
            case DayPart.Dawn:
                CurrentSeasonConfig.DawnLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Morning:
                CurrentSeasonConfig.MorningLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Afternoon:
                CurrentSeasonConfig.AfternoonLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Evening:
                CurrentSeasonConfig.EveningLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Night:
                CurrentSeasonConfig.NightLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Midnight:
                CurrentSeasonConfig.MidnightLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
        }
    }
    private void ApplyDaypartLightingSettingInstantly(DayPart daypart)
    {
        switch (daypart)
        {
            case DayPart.Dawn:
                CurrentSeasonConfig.DawnLightingPreset.ApplyDefault(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Morning:
                CurrentSeasonConfig.MorningLightingPreset.ApplyDefault(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Afternoon:
                CurrentSeasonConfig.AfternoonLightingPreset.ApplyDefault(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Evening:
                CurrentSeasonConfig.EveningLightingPreset.ApplyDefault(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Night:
                CurrentSeasonConfig.NightLightingPreset.ApplyDefault(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Midnight:
                CurrentSeasonConfig.MidnightLightingPreset.ApplyDefault(WorldWeatherController.CurrentWeather);
                break;
        }
    }
    private void ApplyNextDaypartLightingSettingSmoothly()
    {
        switch (CurrentDayPart)
        {
            case DayPart.Dawn:
                CurrentDayPart = DayPart.Morning;
                CurrentSeasonConfig.MorningLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Morning:
                CurrentDayPart = DayPart.Afternoon;
                CurrentSeasonConfig.AfternoonLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Afternoon:
                CurrentDayPart = DayPart.Evening;
                CurrentSeasonConfig.EveningLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Evening:
                CurrentDayPart = DayPart.Night;
                CurrentSeasonConfig.NightLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Night:
                CurrentDayPart = DayPart.Midnight;
                CurrentSeasonConfig.MidnightLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
            case DayPart.Midnight:
                CurrentDayPart = DayPart.Dawn;
                CurrentSeasonConfig.DawnLightingPreset.ApplySmoothly(WorldWeatherController.CurrentWeather);
                break;
        }
    }
    private void ApplyDaypartWeatherLighting(Weather weather)
    {
        switch (CurrentDayPart)
        {
            case DayPart.Dawn:
                CurrentSeasonConfig.DawnLightingPreset.ApplySmoothly(weather); break;
            case DayPart.Morning:
                CurrentSeasonConfig.MorningLightingPreset.ApplySmoothly(weather); break;
            case DayPart.Afternoon:
                CurrentSeasonConfig.AfternoonLightingPreset.ApplySmoothly(weather); break;
            case DayPart.Evening:
                CurrentSeasonConfig.EveningLightingPreset.ApplySmoothly(weather); break;
            case DayPart.Night:
                CurrentSeasonConfig.NightLightingPreset.ApplySmoothly(weather); break;
            case DayPart.Midnight:
                CurrentSeasonConfig.MidnightLightingPreset.ApplySmoothly(weather); break;
        }
    }

    private void ValidateWorld()
    {
        ValidateCurrentSeason();
        ValidateDayPart();
        ApplyDaypartLightingSettingInstantly(CurrentDayPart);

        ValidateCurrentSeason();
        ValidateDayPart();
    }
    #endregion

    #region SAVE GAME SYSTEM
    public void LoadData(GameData data)
    {
        if (DataPersistenceManager.Instance.IsNewGameProgress)
        {
            WorldWeatherController.SetWeatherOnPlayerFirstPlay();
            CurrentWeatherProgress = new WeatherInProgressing(WorldWeatherController.CurrentWeather, 8 * CurrentSeasonConfig.DayTimeConfig.HourInSecond);
        }
        else
        {
            WorldWeatherController.SetRandomWeather();
            CurrentWeatherProgress = new WeatherInProgressing(WorldWeatherController.CurrentWeather, 2 * CurrentSeasonConfig.DayTimeConfig.HourInSecond);
        }
        
        CurrentDayCount = Mathf.Max(1, CurrentDayCount);
        ValidateCurrentSeason();
        ValidateDayPart();
        ApplyDaypartLightingSettingInstantly(CurrentDayPart);

        if (data == null) return;
        CurrentDayCount = data.WorldSavedData.DayCount;
        CurrentDayCount = Mathf.Max(1, CurrentDayCount);
        DayTime = new TimeSpan(data.WorldSavedData.DayTimeHour, data.WorldSavedData.DayTimeMinute, 0);
        ValidateCurrentSeason();
        ValidateDayPart();
    }

    public void SaveData(ref GameData data)
    {
        if (data == null) data = new GameData();
        data.WorldSavedData = new WorldPersistenceData();
        data.WorldSavedData.DayCount = CurrentDayCount;
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
        GUI.Label(new Rect(10, style.fontSize * 2, 500, style.fontSize + 5), "DAY: " + CurrentDayCount + " - TIME: " + DayTime.Hours.ToString("00") + ":" + DayTime.Minutes.ToString("00") + " - " + CurrentDayPart.ToString(), style);
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
        GUILayout.Label("DAY: "     + controller.CurrentDayCount + " - TIME: " + controller.DayTime.Hours + ":" + controller.DayTime.Minutes + " - " + controller.CurrentDayPart.ToString());
        GUILayout.Label("WEATHER: " + controller.CurrentWeather.ToString());
    }
}
#endif
