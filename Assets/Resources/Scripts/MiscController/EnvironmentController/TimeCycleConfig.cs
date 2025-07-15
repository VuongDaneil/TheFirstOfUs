using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using static SceneSharedAttributes;

[CreateAssetMenu(fileName = "TimeCycleConfig", menuName = "SceneSettingAsset/TimeCycleConfig")]
public class TimeCycleConfig : ScriptableObject
{
    #region PROPERTIES
    [Header("DAY-NIGHT CYCLE")]
    [Label("Hour in Second")] public int HourInSecond = 60;

    [Header("PARTS OF THE DAY")]
    public Vector2 Dawn         = new Vector2(5, 7);
    public Vector2 Morning      = new Vector2(7, 12);
    public Vector2 Afternoon    = new Vector2(12, 17);
    public Vector2 Evening      = new Vector2(17, 18);
    public Vector2 Night        = new Vector2(18, 24);
    public Vector2 Midnight     = new Vector2(0, 5);
    #endregion

    #region METHODS
    public DayPart GetDayPart(float hour)
    {
        if (hour >= Dawn.x && hour < Dawn.y)            return DayPart.Dawn;
        if (hour >= Morning.x && hour < Morning.y)      return DayPart.Morning;
        if (hour >= Afternoon.x && hour < Afternoon.y)  return DayPart.Afternoon;
        if (hour >= Evening.x && hour < Evening.y)      return DayPart.Evening;
        if (hour >= Night.x && hour < Night.y)          return DayPart.Night;
        if (hour >= Midnight.x && hour < Midnight.y)    return DayPart.Midnight;
        return DayPart.Morning;
    }
    #endregion
}