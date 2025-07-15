using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class RadioCallingQuestObject : MonoBehaviour,IQuestObject
{
    #region PROPERTIES
    public string Name => "Radio";

    public QuestObjectStatus CurrentStatus = QuestObjectStatus.UnDone;
    public QuestObjectStatus Status => CurrentStatus;

    QuestObjectStatus IQuestObject.Status { get => Status; set => CurrentStatus = value; }

    private Transform playerTransform;

    [Header("STAT(s)")]
    public float DistanceAllowToInteract = 3f;
    public float ProgressSpeed = 1f;

    [Header("DEBUG(s)")]
    [ReadOnly] public float CurrentProgress = 0f;

    private Coroutine ProgressCoroutine;
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
        GameplayEventManager.OnPlayerIntialized.AddListener(OnPlayerInitialized);
    }

    private void UnregisterAllEvents()
    {
        GameplayEventManager.OnPlayerIntialized.RemoveListener(OnPlayerInitialized);
    }
    #endregion

    public void UpdateStatus(QuestObjectStatus newStatus)
    {
        CurrentStatus = newStatus;
    }

    public void OnPlayerInteract()
    {
        StartProgress();
    }

    private void OnPlayerInitialized()
    {
        playerTransform = PlayerBrain.Instance.transform;
    }

    #region _action
    private void StartProgress()
    {
        if (CurrentStatus == QuestObjectStatus.Done || CurrentStatus == QuestObjectStatus.InProgress) return;
        if (ProgressCoroutine != null) StopCoroutine(ProgressCoroutine);

        CurrentStatus = QuestObjectStatus.InProgress;
        ProgressCoroutine = StartCoroutine(Progress());
    }

    private IEnumerator Progress()
    {
        while (CurrentProgress < 100f)
        {
            if (Vector3.Distance(playerTransform.position, transform.position) > DistanceAllowToInteract)
            {
                CurrentProgress = 0;
                CurrentStatus = QuestObjectStatus.UnDone;
                yield break;
            }
            CurrentProgress += ProgressSpeed * Time.deltaTime;
            yield return null;
        }
        CurrentProgress = 100f;
        CurrentStatus = QuestObjectStatus.Done;
        GameplayEventManager.OnARadioCallingQuestCompleted?.Invoke(this);
    }
    #endregion
    #endregion
}
