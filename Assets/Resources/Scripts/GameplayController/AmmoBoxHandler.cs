using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBoxHandler : MonoBehaviour, IQuestObject
{
    public string Name => "AMMO BOX";

    public QuestObjectStatus ObjectStatus => Status;
    public QuestObjectStatus Status { get => ObjectStatus; set => Status = value; }

    [Header("AUDIO")]
    public AudioSource QuestObjectAudioSource;
    public AudioClip InteractAudioClip;

    private void OnValidate()
    {
        if (QuestObjectAudioSource == null) QuestObjectAudioSource = GetComponent<AudioSource>();
    }

    public float GetCurrentProgress()
    {
        return 1;
    }

    public void OnPlayerInteract()
    {
        QuestObjectAudioSource.PlayOneShot(InteractAudioClip);
        PlayerControlEventMananger.OnPlayerInteractAmmoBox?.Invoke();
    }

    public void UpdateStatus(QuestObjectStatus newStatus)
    {
        
    }
}