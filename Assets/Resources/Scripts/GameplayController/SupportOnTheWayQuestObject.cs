using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class SupportOnTheWayQuestObject : MonoBehaviour, IQuestObject
{
    #region PROPERTIES
    public string Name => "Support are on their way";

    public QuestObjectStatus CurrentStatus = QuestObjectStatus.UnDone;
    public QuestObjectStatus Status => CurrentStatus;

    QuestObjectStatus IQuestObject.Status { get => Status; set => CurrentStatus = value; }

    private Transform playerTransform;

    [Header("STAT(s)")]
    public float DistanceAllowToInteract = 3f;
    public float ProgressSpeed = 1f;

    [Header("AUDIO")]
    public AudioSource QuestObjectAudioSource;
    public AudioClip StartProgressAudioClip;
    public AudioClip CompleteProgressAudioClip;

    [Header("DEBUG(s)")]
    [ReadOnly] public float CurrentProgress = 0f;

    private Coroutine ProgressCoroutine;
    #endregion

    #region UNITY CORE
    private void OnValidate()
    {
        if (QuestObjectAudioSource == null) QuestObjectAudioSource = GetComponent<AudioSource>();
    }
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
        QuestObjectAudioSource.PlayOneShot(StartProgressAudioClip);
        GameplayEventManager.OnStartSupportComingQuest?.Invoke(this);
    }

    private IEnumerator Progress()
    {
        while (CurrentProgress < 100f)
        {
            if (!PlayerBrain.Instance.IsAlive)
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
        QuestObjectAudioSource.PlayOneShot(CompleteProgressAudioClip);
        GameplayEventManager.OnSupportComingQuestCompleted?.Invoke(this);
    }

    public float GetCurrentProgress()
    {
        return CurrentProgress / 100f;
    }
    #endregion

    #endregion
}
