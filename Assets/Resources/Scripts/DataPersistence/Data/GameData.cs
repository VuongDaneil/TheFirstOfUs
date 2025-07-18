using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public PlayerPersistenceData PlayerSavedData;
    public WorldPersistenceData WorldSavedData;
    public QuestProgressPersistenceData QuestProgressSavedData;

    public GameData()
    {
        PlayerSavedData = new PlayerPersistenceData();
        WorldSavedData = new WorldPersistenceData();
        QuestProgressSavedData = new QuestProgressPersistenceData();
    }
}

[System.Serializable]
public class PlayerPersistenceData
{
    // Stats
    public int PlayerHealth;
    public float PlayerStamina;
    public Vector3 PlayerPosition;
    public Vector3 PlayerEulerAngle;

    // Weapon
    public int MainWeaponID;
    public int SubWeaponID;
    public int MainWeaponCurrentMagazine;
    public int MainWeaponCurrentAmmoCapacity;
    public int SubWeaponCurrentMagazine;

    public PlayerPersistenceData()
    {
        PlayerHealth = 100;
        PlayerStamina = 100f;
        PlayerPosition = new Vector3(-30, 6.5f, -220);
        PlayerEulerAngle = Vector3.zero;

        MainWeaponID = 1;
        SubWeaponID = 2;
        SubWeaponCurrentMagazine = 10;
        MainWeaponCurrentMagazine = 45;
        MainWeaponCurrentAmmoCapacity = 500;
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
        DayCount = 1;
        DayTimeHour = 0;
        DayTimeMinute = 0;
    }
}

[System.Serializable]
public class QuestProgressPersistenceData
{
    public bool CompletedFirstRadioTowerQuest;
    public bool CompletedSecondRadioTowerQuest;
    public bool CompletedThirdRadioTowerQuest;
    public bool CompletedRadioCallingQuest;

    public QuestProgressPersistenceData()
    {
        CompletedFirstRadioTowerQuest = false;
        CompletedSecondRadioTowerQuest = false;
        CompletedThirdRadioTowerQuest = false;
        CompletedRadioCallingQuest = false;
    }
}
