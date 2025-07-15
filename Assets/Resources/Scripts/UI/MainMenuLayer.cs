using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLayer : MonoBehaviour
{
    #region PROPERTIES
    [Header("UI ELEMENT(s)")]
    public Button StartGameButton;
    public Button SettingButton;
    public Button QuitGameButton;
    [Space]
    public CanvasGroup MainMenuCanvasGroup;
    public CanvasGroup SettingCanvasGroup;

    private string GameSceneName = "GameScene";
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        RegisterAllEvents();
        
        MainMenuCanvasGroup.alpha = 1f;
        MainMenuCanvasGroup.interactable = true;

        SettingCanvasGroup.alpha = 0f;
        SettingCanvasGroup.interactable = false;
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
        StartGameButton.onClick.AddListener(OnStartPlaying);
        SettingButton.onClick.AddListener(OnOpenSettingLayer);
        QuitGameButton.onClick.AddListener(OnQuitGame);
        UIEventManager.OnOpenMainMenu?.Invoke();
    }

    private void UnregisterAllEvents()
    {
        StartGameButton.onClick.RemoveListener(OnStartPlaying);
        SettingButton.onClick.RemoveListener(OnOpenSettingLayer);
        QuitGameButton.onClick.RemoveListener(OnQuitGame);
        UIEventManager.OnCloseMainMenu?.Invoke();
    }
    #endregion

    private void OnStartPlaying()
    {
        MainMenuCanvasGroup.alpha = 0f;
        SettingCanvasGroup.alpha = 0f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameConstant.GameScene);
    }

    private void OnOpenSettingLayer()
    {
        UIEventManager.OnOpenSettingsMenu?.Invoke();
        SettingCanvasGroup.DOFade(1, 1).OnComplete(() =>
        {
            SettingCanvasGroup.interactable = true;
        });
    }

    private void OnQuitGame()
    {
        Application.Quit();
    }

    #endregion
}
