using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.XR;

public class QuestManager : MonoBehaviour
{
    #region PROPETIES
    public static QuestManager Instance;

    [Header("QUEST: RADIO TOWER")]
    [ReadOnly] public bool CompletedRadioTowerQuest = false;
    [ReadOnly] public int RadioTowerQuestDone = 0;
    public List<RadioTowerQuestObject> RadioTowerQuestObjects = new List<RadioTowerQuestObject>();

    [Header("QUEST: RADIO CALLING")]
    [ReadOnly] public bool CompletedRadioCallingQuest = false;
    public RadioCallingQuestObject RadioCallingQuestObject;

    [Header("QUEST: SUPPORTS ARE ON THEIR WAY")]
    [ReadOnly] public bool SupportComingIsCompleted = false;
    public SupportOnTheWayQuestObject SupportComingQuestObject;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        RegisterAllEvents();
    }

    private void OnDestroy()
    {
        UnregisterAllEvents();
    }
    #endregion

    #region MAIN
    #region _events
    private void RegisterAllEvents()
    {
        GameplayEventManager.OnPlayerIntialized.AddListener(OnPlayerInitialized);
        GameplayEventManager.OnARadioTowerQuestCompleted.AddListener(OnARadioTowerQuestCompleted);
        GameplayEventManager.OnRadioCallingQuestCompleted.AddListener(OnARadioCallingQuestCompleted);
    }

    private void UnregisterAllEvents()
    {
        GameplayEventManager.OnPlayerIntialized.RemoveListener(OnPlayerInitialized);
        GameplayEventManager.OnARadioTowerQuestCompleted.RemoveListener(OnARadioTowerQuestCompleted);
        GameplayEventManager.OnRadioCallingQuestCompleted.RemoveListener(OnARadioCallingQuestCompleted);
    }

    private void OnPlayerInitialized()
    {
        InitArrowIndiacator();
    }

    private void OnARadioTowerQuestCompleted(RadioTowerQuestObject questObject)
    {
        RadioTowerQuestDone++;
        if (RadioTowerQuestDone >= RadioTowerQuestObjects.Count)
        {
            CompletedRadioTowerQuest = true;
            RadioCallingQuestObject.SetAvailabale(true);
            GameplayEventManager.OnAllRadioTowerQuestsCompleted.Invoke();
        }
        InitArrowIndiacator();
    }

    private void OnARadioCallingQuestCompleted(RadioCallingQuestObject questObject)
    {
        CompletedRadioCallingQuest = true;
        StartSupportIsComingQuest();
        InitArrowIndiacator();
    }

    private void StartSupportIsComingQuest()
    {
        if (SupportComingQuestObject == null) return;
        SupportComingQuestObject.OnPlayerInteract();
    }
    #endregion

    #endregion

    #region SUPPORTIVE
    private void InitArrowIndiacator()
    {
        if (ArrowCompassUI.Instance == null) return;
        if (CompletedRadioTowerQuest) ArrowCompassUI.Instance.SetTarget(RadioCallingQuestObject.transform);
        else
        {
            if (RadioTowerQuestObjects.Any(x => x.Status == QuestObjectStatus.InProgress))
            {
                ArrowCompassUI.Instance.SetTarget(RadioTowerQuestObjects.FirstOrDefault(x => x.Status == QuestObjectStatus.InProgress).transform);
            }
            else if (RadioTowerQuestObjects.Any(x => x.Status == QuestObjectStatus.UnDone))
            {
                ArrowCompassUI.Instance.SetTarget(RadioTowerQuestObjects.FirstOrDefault(x => x.Status == QuestObjectStatus.UnDone).transform);
            }
        }
    }
    #endregion
}