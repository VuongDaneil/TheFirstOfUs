using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseLayerHandler : MonoBehaviour
{
    #region PROPERTIES
    [ReadOnly] public GameState CurrentGameState = GameState.Playing;

    [Header("LAYER")]
    public CanvasGroup SettingLayer;
    public CanvasGroup PauseLayer;
    public GameObject HUDGameObject;
    public GameObject LoadingScreen;

    [Header("UI ELEMENT")]
    public Button ResumeBtn;
    public Button SettingBtn;
    public Button ExitBtn;
    public enum GameState
    {
        Playing,
        Paused
    }
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        CurrentGameState = GameState.Playing;
        RegisterAllEvents();
        ResumeGame();
    }

    private void Update()
    {
        HandlePlayerInput();
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
        LoadingScreen.SetActive(false);
        ResumeBtn.onClick.AddListener(ResumeGame);
        SettingBtn.onClick.AddListener(OnOpenSettingLayer);
        ExitBtn.onClick.AddListener(OnExitGame);
    }

    private void UnregisterAllEvents()
    {
        ResumeBtn.onClick.RemoveListener(ResumeGame);
        SettingBtn.onClick.RemoveListener(OnOpenSettingLayer);
        ExitBtn.onClick.RemoveListener(OnExitGame);
    }
    #endregion

    private void HandlePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentGameState == GameState.Playing)
            {
                PauseGame();
            }
            else if (CurrentGameState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        if (PauseLayer != null)
        {
            PauseLayer.alpha = 1f;
            PauseLayer.interactable = true;
            PauseLayer.blocksRaycasts = true;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        CurrentGameState = GameState.Paused;
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        if (PauseLayer != null)
        {
            PauseLayer.alpha = 0f;
            PauseLayer.interactable = false;
            PauseLayer.blocksRaycasts = false;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        CurrentGameState = GameState.Playing;
        OnCloseSettingLayer();
        Time.timeScale = 1f;
    }

    private void OnOpenSettingLayer()
    {
        SettingLayer.alpha = 1f;
        UIEventManager.OnOpenSettingsMenu?.Invoke();
    }

    private void OnCloseSettingLayer()
    {
        SettingLayer.alpha = 0f;
        UIEventManager.OnCloseSettingsMenu?.Invoke();
    }

    private void OnExitGame()
    {
        DataPersistenceManager.Instance.SaveGame();
        UIEventManager.OnQuitToMainMenu?.Invoke();
        HUDGameObject.SetActive(false);
        LoadingScreen.SetActive(true);
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameConstant.MainMenuScene);
    }
    #endregion
}