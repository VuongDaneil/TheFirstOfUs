using System.Collections.Generic;
using NaughtyAttributes;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour, IDataPersistence
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

    [Button("SKIP RADIO TOWERS")]
    public void SetAllRaidoTowerQuestDone()
    {
        foreach (var questObject in RadioTowerQuestObjects)
        {
            questObject.UpdateStatus(QuestObjectStatus.Done);
        }
        CompletedRadioTowerQuest = true;
        RadioCallingQuestObject.SetAvailabale(true);
        GameplayEventManager.OnAllRadioTowerQuestsCompleted.Invoke();
        InitArrowIndiacator();
    }

    #endregion

    #region SAVE GAME DATA
    public void LoadData(GameData data)
    {
        if (data == null) return;
        CompletedRadioTowerQuest = data.QuestProgressSavedData.CompletedFirstRadioTowerQuest &&
                                   data.QuestProgressSavedData.CompletedSecondRadioTowerQuest &&
                                   data.QuestProgressSavedData.CompletedThirdRadioTowerQuest;
        RadioTowerQuestObjects[0].UpdateStatus(data.QuestProgressSavedData.CompletedFirstRadioTowerQuest ? QuestObjectStatus.Done : QuestObjectStatus.UnDone);
        RadioTowerQuestObjects[1].UpdateStatus(data.QuestProgressSavedData.CompletedSecondRadioTowerQuest ? QuestObjectStatus.Done : QuestObjectStatus.UnDone);
        RadioTowerQuestObjects[2].UpdateStatus(data.QuestProgressSavedData.CompletedThirdRadioTowerQuest ? QuestObjectStatus.Done : QuestObjectStatus.UnDone);
        CompletedRadioCallingQuest = data.QuestProgressSavedData.CompletedRadioCallingQuest;
        if (CompletedRadioCallingQuest)
        {
            RadioCallingQuestObject.UpdateStatus(QuestObjectStatus.Done);
            StartSupportIsComingQuest();
        }
        else
        {
            RadioCallingQuestObject.SetAvailabale(false);
        }
        InitArrowIndiacator();
    }

    public void SaveData(ref GameData data)
    {
        data.QuestProgressSavedData.CompletedFirstRadioTowerQuest = RadioTowerQuestObjects[0].CurrentStatus == QuestObjectStatus.Done;
        data.QuestProgressSavedData.CompletedSecondRadioTowerQuest = RadioTowerQuestObjects[1].CurrentStatus == QuestObjectStatus.Done;
        data.QuestProgressSavedData.CompletedThirdRadioTowerQuest = RadioTowerQuestObjects[2].CurrentStatus == QuestObjectStatus.Done;
        data.QuestProgressSavedData.CompletedRadioCallingQuest = CompletedRadioCallingQuest;
    }
    #endregion
}