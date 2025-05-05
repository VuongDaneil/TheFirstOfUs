using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public PlayerPersistenceData PlayerSavedData;
    public WorldPersistenceData WorldSavedData;

    public GameData()
    {
        PlayerSavedData = new PlayerPersistenceData();
        WorldSavedData = new WorldPersistenceData();
    }
}

[System.Serializable]
public class PlayerPersistenceData
{
    public int PlayerHealth;
    public float PlayerStamina;
    public Vector3 PlayerPosition;
    public Vector3 PlayerEulerAngle;

    public PlayerPersistenceData()
    {
        PlayerHealth = 100;
        PlayerStamina = 100f;
        PlayerPosition = Vector3.zero;
        PlayerEulerAngle = Vector3.zero;
    }
}

[System.Serializable]
public class WorldPersistenceData
{
    public int DayCount;
    public int DayTimeHour;
    public int DayTimeMinute;

    public WorldPersistenceData()
    {
        DayCount = 0;
        DayTimeHour = 0;
        DayTimeMinute = 0;
    }
}
