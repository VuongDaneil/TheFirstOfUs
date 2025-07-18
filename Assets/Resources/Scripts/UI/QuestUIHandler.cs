using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIHandler : MonoBehaviour
{
    #region PROPERTIES
    [Header("UI ELEMENT(s)")]
    public CanvasGroup QuestProgressCanvasGroup;
    public TMP_Text QuestStatusProgressTxt;
    public Image QuestProgressBar;

    [Space]
    public CanvasGroup QuestAnnouncementCanvasGroup;
    public TMP_Text QuestStatusAnnouncementTxt;

    private Coroutine ProgressQuest;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        ResetAllElement();
        RegisterAllEvents();
    }

    private void OnDestroy()
    {
        UnregisterAllEvents();
        ResetAllElement();
    }
    #endregion

    #region MAIN

    #region _events
    private void RegisterAllEvents()
    {
        GameplayEventManager.OnAQuestFailed.AddListener(OnAQuestFailed);

        GameplayEventManager.OnStartARadioTowerQuest.AddListener(OnStartARadioTowerQuest);
        GameplayEventManager.OnARadioTowerQuestCompleted.AddListener(OnARadioTowerQuestCompleted);
        GameplayEventManager.OnAllRadioTowerQuestsCompleted.AddListener(OnAllRadioTowerQuestsDone);

        GameplayEventManager.OnStartARadioCallQuest.AddListener(OnStartARadioCallingQuest);
        GameplayEventManager.OnRadioCallingQuestCompleted.AddListener(OnRadioCallingQuestDone);

        GameplayEventManager.OnStartSupportComingQuest.AddListener(OnStartSupportComingQuest);
        GameplayEventManager.OnSupportComingQuestCompleted.AddListener(OnSupportComingQuestCompleted);

        PlayerControlEventMananger.OnPlayerDoneIntro.AddListener(QuestInstruction);
    }

    private void UnregisterAllEvents()
    {
        GameplayEventManager.OnAQuestFailed.RemoveListener(OnAQuestFailed);

        GameplayEventManager.OnStartARadioTowerQuest.RemoveListener(OnStartARadioTowerQuest);
        GameplayEventManager.OnARadioTowerQuestCompleted.RemoveListener(OnARadioTowerQuestCompleted);
        GameplayEventManager.OnAllRadioTowerQuestsCompleted.RemoveListener(OnAllRadioTowerQuestsDone);

        GameplayEventManager.OnStartARadioCallQuest.RemoveListener(OnStartARadioCallingQuest);
        GameplayEventManager.OnRadioCallingQuestCompleted.RemoveListener(OnRadioCallingQuestDone);

        GameplayEventManager.OnStartSupportComingQuest.RemoveListener(OnStartSupportComingQuest);
        GameplayEventManager.OnSupportComingQuestCompleted.RemoveListener(OnSupportComingQuestCompleted);

        PlayerControlEventMananger.OnPlayerDoneIntro.RemoveListener(QuestInstruction);
    }

    private void OnAQuestFailed()
    {
        QuestProgressCanvasGroup.DOFade(0, 0.5f);
        QuestStatusProgressTxt.text = "Failed";
        QuestProgressBar.DOFillAmount(0, 0.25f);
    }
    #endregion

    #region _radio tower quest
    private void OnStartARadioTowerQuest(RadioTowerQuestObject quest)
    {
        QuestProgressCanvasGroup.alpha = 1f;
        QuestStatusProgressTxt.text = "Fixing the radio tower in progress...";
        QuestProgressBar.fillAmount = 0f;

        if (ProgressQuest != null)
        {
            StopCoroutine(ProgressQuest);
        }
        ProgressQuest = StartCoroutine(StartProgressMission(quest));
    }
    private void OnARadioTowerQuestCompleted(RadioTowerQuestObject arg0)
    {
        QuestProgressCanvasGroup.DOFade(0, 0.5f);
        QuestStatusProgressTxt.text = "Done";
        QuestProgressBar.DOFillAmount(0, 0.25f);

        int radioTowerFixed = QuestManager.Instance.RadioTowerQuestDone + 1;
        int radioTowerNeedToFixed = QuestManager.Instance.RadioTowerQuestObjects.Count;

        if (radioTowerFixed >= radioTowerNeedToFixed) return;
        QuestAnnouncementCanvasGroup.alpha = 1;
        QuestStatusAnnouncementTxt.text = "Radio tower is fixed! (" + radioTowerFixed.ToString() + "/" + radioTowerNeedToFixed.ToString() + ")";

        StartCoroutine(FadeAnnouncement());
    }
    private IEnumerator StartProgressMission(IQuestObject quest)
    {
        while (quest.Status != QuestObjectStatus.Done)
        {
            QuestProgressBar.fillAmount = quest.GetCurrentProgress();
            yield return null;
        }
    }

    private void OnAllRadioTowerQuestsDone()
    {
        QuestAnnouncementCanvasGroup.alpha = 1;
        QuestStatusAnnouncementTxt.text = "All the radio tower are fixed, now head to the end of the pier to make the distress call!";

        StartCoroutine(FadeAnnouncement(10f));
    }
    #endregion

    #region _radio tower quest
    private void OnStartARadioCallingQuest(RadioCallingQuestObject quest)
    {
        QuestProgressCanvasGroup.alpha = 1f;
        QuestStatusProgressTxt.text = "Calling for support...";
        QuestProgressBar.fillAmount = 0f;

        if (ProgressQuest != null)
        {
            StopCoroutine(ProgressQuest);
        }
        ProgressQuest = StartCoroutine(StartProgressMission(quest));
    }

    private void OnRadioCallingQuestDone(RadioCallingQuestObject quest)
    {
        QuestProgressBar.DOFillAmount(0, 0.25f);

        QuestAnnouncementCanvasGroup.alpha = 1;
        QuestStatusAnnouncementTxt.text = "Support are on their way, try to survive, good luck!";

        StartCoroutine(FadeAnnouncement(10f));
    }
    #endregion

    #region _support coming quest
    private void OnStartSupportComingQuest(SupportOnTheWayQuestObject quest)
    {
        DOTween.Kill(QuestProgressCanvasGroup);
        DOTween.Kill(QuestProgressBar);
        QuestProgressCanvasGroup.alpha = 1f;
        QuestStatusProgressTxt.text = "Calling for support...";
        QuestProgressBar.fillAmount = 0f;

        if (ProgressQuest != null)
        {
            StopCoroutine(ProgressQuest);
        }
        ProgressQuest = StartCoroutine(StartProgressMission(quest));
    }

    private void OnSupportComingQuestCompleted(SupportOnTheWayQuestObject quest)
    {
        QuestAnnouncementCanvasGroup.alpha = 1;
        QuestStatusAnnouncementTxt.text = "AMBATUBLOU!";

        QuestProgressCanvasGroup.DOFade(0, 0.5f);

        StartCoroutine(FadeAnnouncement(10f));
    }
    #endregion

    #region First Cutscene
    private void QuestInstruction()
    {
        QuestProgressBar.DOFillAmount(0, 0.25f);

        QuestAnnouncementCanvasGroup.alpha = 1;
        QuestStatusAnnouncementTxt.text = "Fix all the radio towers then call for support";

        StartCoroutine(FadeAnnouncement(5f));
    }
    #endregion

    #endregion

    #region SUPPORTIVE
    private void ResetAllElement()
    {
        QuestProgressCanvasGroup.alpha = 0f;
        QuestAnnouncementCanvasGroup.alpha = 0f;
    }

    private IEnumerator FadeAnnouncement(float delay = 2f)
    {
        yield return new WaitForSeconds(delay);
        QuestAnnouncementCanvasGroup.DOFade(0, 0.5f);
    }
    #endregion
}
