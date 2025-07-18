using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHUDHandler : MonoBehaviour
{
    [Header("PROPERTIES")]
    public CanvasGroup HUDCanvasGroup;
    public CanvasGroup DeadLayerCanvasGroup;

    private void Start()
    {
        if (!DataPersistenceManager.Instance.IsNewGameProgress) ShowHud();
        else HideHud();

        PlayerControlEventMananger.OnPlayerDie.AddListener(OnPlayerDie);
        PlayerControlEventMananger.OnPlayerDoneIntro.AddListener(OnPlayerDoneIntro);
    }

    private void OnDestroy()
    {
        PlayerControlEventMananger.OnPlayerDie.RemoveListener(OnPlayerDie);
        PlayerControlEventMananger.OnPlayerDoneIntro.RemoveListener(OnPlayerDoneIntro);
    }

    private void OnPlayerDoneIntro()
    {
        HUDFadeIn();
    }

    private void OnPlayerDie()
    {
        HUDFadeOut();
        StartCoroutine(DelayDeadlAyerShowUp());
    }

    public void ShowHud() => HUDCanvasGroup.alpha = 1f;
    public void HideHud() => HUDCanvasGroup.alpha = 0f;

    public void HUDFadeIn()
    {
        HUDCanvasGroup.DOFade(1f, 1f);
    }

    public void HUDFadeOut()
    {
        HUDCanvasGroup.DOFade(0f, 1f);
    }

    IEnumerator DelayDeadlAyerShowUp()
    {
        yield return new WaitForSeconds(3f);
        DeadLayerCanvasGroup.DOFade(1, 2);
        yield return new WaitForSeconds(2f);
        GameplayEventManager.OnPlayerFallToGround?.Invoke();
        SceneManager.LoadScene(GameConstant.MainMenuScene);
    }
}
