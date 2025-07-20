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

        GameplayEventManager.OnGameEnd.AddListener(OnGameEnd);
        PlayerControlEventMananger.OnPlayerDie.AddListener(OnGameEnd);
        PlayerControlEventMananger.OnPlayerDoneIntro.AddListener(OnPlayerDoneIntro);
    }

    private void OnDestroy()
    {
        GameplayEventManager.OnGameEnd.RemoveListener(OnGameEnd);
        PlayerControlEventMananger.OnPlayerDie.RemoveListener(OnGameEnd);
        PlayerControlEventMananger.OnPlayerDoneIntro.RemoveListener(OnPlayerDoneIntro);
    }

    private void OnPlayerDoneIntro()
    {
        HUDFadeIn();
    }

    private void OnGameEnd()
    {
        HUDFadeOut();
        if (PlayerBrain.Instance.IsAlive) StartCoroutine(DelayAfterEndgame());
        else StartCoroutine(DelayDeadLayerShowUp());
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

    IEnumerator DelayDeadLayerShowUp()
    {
        DataPersistenceManager.Instance.DeleteSavedGamedata();
        DeadLayerCanvasGroup.DOFade(1, 2);
        yield return new WaitForSeconds(3f);
        yield return new WaitForSeconds(2f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameplayEventManager.OnPlayerFallToGround?.Invoke();
        SceneManager.LoadScene(GameConstant.MainMenuScene);
    }

    IEnumerator DelayAfterEndgame()
    {
        DataPersistenceManager.Instance.DeleteSavedGamedata();
        yield return new WaitForSeconds(10f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(GameConstant.MainMenuScene);
    }
}
