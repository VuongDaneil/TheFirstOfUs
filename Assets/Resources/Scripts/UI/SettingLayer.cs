using static GameConstant;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class SettingLayer : MonoBehaviour
{
    #region PROPERTIES
    [Header("DATA")]
    public CharacterControllerBinding KeyBindingData;
    public AudioMixer AudioMixer;

    [Header("UI ELEMENT(s)")]
    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;

    [Space]
    public Slider SensitivitySlider;
    public TMP_InputField MoveForwardInputField;
    public TMP_InputField MoveBackwardInputField;
    public TMP_InputField MoveLeftInputField;
    public TMP_InputField MoveRightInputField;
    public TMP_InputField LeanLeftInputField;
    public TMP_InputField LeanRightInputField;
    public TMP_InputField ReloadInputField;
    public TMP_InputField MainWeaponInputField;
    public TMP_InputField SubWeaponInputField;

    [Space]
    public Button SaveButton;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        RegisterAllEvents();
    }

    private void Start()
    {
        Initialize();
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
        SaveButton.onClick.AddListener(OnSaveSetting);
        UIEventManager.OnOpenSettingsMenu.AddListener(Initialize);
    }
    private void UnregisterAllEvents()
    {
        SaveButton.onClick.RemoveListener(OnSaveSetting);
        UIEventManager.OnOpenSettingsMenu.RemoveListener(Initialize);
    }

    private void OnSaveSetting()
    {
        if (KeyBindingData != null)
        {
            KeyBindingData.MoveForward = (KeyCode)Enum.Parse(typeof(KeyCode), MoveForwardInputField.text.ToUpper());
            KeyBindingData.MoveBackward = (KeyCode)Enum.Parse(typeof(KeyCode), MoveBackwardInputField.text);
            KeyBindingData.MoveLeft = (KeyCode)Enum.Parse(typeof(KeyCode), MoveLeftInputField.text);
            KeyBindingData.MoveRight = (KeyCode)Enum.Parse(typeof(KeyCode), MoveRightInputField.text);
            KeyBindingData.LeanLeft = (KeyCode)Enum.Parse(typeof(KeyCode), LeanLeftInputField.text);
            KeyBindingData.LeanRight = (KeyCode)Enum.Parse(typeof(KeyCode), LeanRightInputField.text);
            KeyBindingData.Reload = (KeyCode)Enum.Parse(typeof(KeyCode), ReloadInputField.text);
            KeyBindingData.MainWeapon = (KeyCode)Enum.Parse(typeof(KeyCode), MainWeaponInputField.text);
            KeyBindingData.SubWeapon = (KeyCode)Enum.Parse(typeof(KeyCode), SubWeaponInputField.text);

            KeyBindingData.SaveToPlayerPref();
        }

        PlayerPrefs.SetFloat(SensitivityKey, SensitivitySlider.value);
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolumeSlider.value);
        AudioMixer.SetFloat(MusicVolumeKey, Mathf.Log10(MusicVolumeSlider.value) * 20f);
        PlayerPrefs.SetFloat(SFXVolumeKey, SFXVolumeSlider.value);
        AudioMixer.SetFloat(SFXVolumeKey, Mathf.Log10(SFXVolumeSlider.value) * 20f);
        PlayerPrefs.Save();
    }
    #endregion

    private void Initialize()
    {
        if (KeyBindingData != null)
        {
            MoveForwardInputField.text = KeyBindingData.MoveForward.ToString();
            MoveBackwardInputField.text = KeyBindingData.MoveBackward.ToString();
            MoveLeftInputField.text = KeyBindingData.MoveLeft.ToString();
            MoveRightInputField.text = KeyBindingData.MoveRight.ToString();
            LeanLeftInputField.text = KeyBindingData.LeanLeft.ToString();
            LeanRightInputField.text = KeyBindingData.LeanRight.ToString();
            ReloadInputField.text = KeyBindingData.Reload.ToString();
            MainWeaponInputField.text = KeyBindingData.MainWeapon.ToString();
            SubWeaponInputField.text = KeyBindingData.SubWeapon.ToString();
        }

        float sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 50);
        SensitivitySlider.value = sensitivity;

        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
        MusicVolumeSlider.value = musicVolume;
        AudioMixer.SetFloat(MusicVolumeKey, Mathf.Log10(musicVolume) * 20f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 0.5f);
        SFXVolumeSlider.value = sfxVolume;
        AudioMixer.SetFloat(SFXVolumeKey, Mathf.Log10(sfxVolume) * 20f);
    }

    #endregion
}
