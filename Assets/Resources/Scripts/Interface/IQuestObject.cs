using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestObject
{
    string Name { get; }
    public QuestObjectStatus Status { get; set; }

    void UpdateStatus(QuestObjectStatus newStatus);
    void OnPlayerInteract();
    float GetCurrentProgress();
}

public enum QuestObjectStatus
{
    UnDone,
    InProgress,
    Done,
}
