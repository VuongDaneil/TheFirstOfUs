using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    #region PROPETIES
    [Header("QUEST: RADIO TOWER")]
    [ReadOnly] public bool CompletedRadioTowerQuest = false;
    [ReadOnly] public int RadioTowerQuestDone = 0;
    public List<RadioTowerQuestObject> RadioTowerQuestObjects = new List<RadioTowerQuestObject>();

    [Header("QUEST: RADIO CALLING")]
    [ReadOnly] public bool CompletedRadioCallingQuest = false;
    public RadioCallingQuestObject RadioCallingQuestObject;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
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
        //GameplayEventManager.OnPlayerIntialized.AddListener(OnPlayerInitialized);
        GameplayEventManager.OnARadioTowerQuestCompleted.AddListener(OnARadioTowerQuestCompleted);
        GameplayEventManager.OnARadioCallingQuestCompleted.AddListener(OnARadioCallingQuestCompleted);
    }
    private void UnregisterAllEvents()
    {
        //GameplayEventManager.OnPlayerIntialized.RemoveListener(OnPlayerInitialized);
        GameplayEventManager.OnARadioTowerQuestCompleted.RemoveListener(OnARadioTowerQuestCompleted);
        GameplayEventManager.OnARadioCallingQuestCompleted.RemoveListener(OnARadioCallingQuestCompleted);
    }

    private void OnARadioTowerQuestCompleted(RadioTowerQuestObject questObject)
    {
        RadioTowerQuestDone++;
        if (RadioTowerQuestDone >= RadioTowerQuestObjects.Count)
        {
            CompletedRadioTowerQuest = true;
        }
    }

    private void OnARadioCallingQuestCompleted(RadioCallingQuestObject questObject)
    {
        CompletedRadioCallingQuest = true;
    }
    #endregion

    #endregion
}