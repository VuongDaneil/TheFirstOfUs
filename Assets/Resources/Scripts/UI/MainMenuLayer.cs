using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLayer : MonoBehaviour
{
    #region PROPERTIES
    [Header("UI ELEMENT(s)")]
    public GameObject LoadingScene;
    public Button StartNewGameButton;
    public Button StartSavedGameButton;
    public Button SettingButton;
    public Button QuitGameButton;
    [Space]
    public CanvasGroup MainMenuCanvasGroup;
    public CanvasGroup SettingCanvasGroup;

    public AudioSource[] MainMenuAudioSource;

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
        LoadingScene.SetActive(false);
        StartSavedGameButton.interactable = DataPersistenceManager.Instance.HasGameData();
        StartSavedGameButton.onClick.AddListener(OnStartSavedPlaying);
        StartNewGameButton.onClick.AddListener(OnStartNewPlaying);
        SettingButton.onClick.AddListener(OnOpenSettingLayer);
        QuitGameButton.onClick.AddListener(OnQuitGame);
        UIEventManager.OnOpenMainMenu?.Invoke();
    }

    private void UnregisterAllEvents()
    {
        StartSavedGameButton.onClick.RemoveListener(OnStartSavedPlaying);
        StartNewGameButton.onClick.RemoveListener(OnStartNewPlaying);
        SettingButton.onClick.RemoveListener(OnOpenSettingLayer);
        QuitGameButton.onClick.RemoveListener(OnQuitGame);
        UIEventManager.OnCloseMainMenu?.Invoke();
    }
    #endregion

    private void OnStartNewPlaying()
    {
        LoadingScene.SetActive(true);
        MainMenuCanvasGroup.alpha = 0f;
        SettingCanvasGroup.alpha = 0f;
        DataPersistenceManager.Instance.NewGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameConstant.GameScene);
    }
    private void OnStartSavedPlaying()
    {
        LoadingScene.SetActive(true);
        MainMenuCanvasGroup.alpha = 0f;
        SettingCanvasGroup.alpha = 0f;
        foreach (var audioSource in MainMenuAudioSource) audioSource.Stop();
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
