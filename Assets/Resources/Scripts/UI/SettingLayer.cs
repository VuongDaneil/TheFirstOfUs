using static GameConstant;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.Windows;

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
            KeyBindingData.MoveBackward = (KeyCode)Enum.Parse(typeof(KeyCode), MoveBackwardInputField.text.ToUpper());
            KeyBindingData.MoveLeft = (KeyCode)Enum.Parse(typeof(KeyCode), MoveLeftInputField.text.ToUpper());
            KeyBindingData.MoveRight = (KeyCode)Enum.Parse(typeof(KeyCode), MoveRightInputField.text.ToUpper());
            KeyBindingData.LeanLeft = (KeyCode)Enum.Parse(typeof(KeyCode), LeanLeftInputField.text.ToUpper());
            KeyBindingData.LeanRight = (KeyCode)Enum.Parse(typeof(KeyCode), LeanRightInputField.text.ToUpper());
            KeyBindingData.Reload = (KeyCode)Enum.Parse(typeof(KeyCode), ReloadInputField.text.ToUpper());
            KeyBindingData.MainWeapon = (KeyCode)Enum.Parse(typeof(KeyCode), MainWeaponInputField.text.ToUpper());
            KeyBindingData.SubWeapon = (KeyCode)Enum.Parse(typeof(KeyCode), SubWeaponInputField.text.ToUpper());

            KeyBindingData.SaveToPlayerPref();
        }

        PlayerPrefs.SetFloat(SensitivityKey, SensitivitySlider.value);
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolumeSlider.value);
        PlayerPrefs.SetFloat(SFXVolumeKey, SFXVolumeSlider.value);


        float music = Mathf.Log10(MusicVolumeSlider.value) * 20f;
        float sfx = Mathf.Log10(SFXVolumeSlider.value) * 20f;

        AudioMixer.SetFloat(MusicVolumeKey, music);
        AudioMixer.SetFloat(SFXVolumeKey, sfx);
        PlayerPrefs.Save();
    }
    #endregion

    private void Initialize()
    {
        if (KeyBindingData != null)
        {
            MoveForwardInputField.text = ConvertKeyCodeToString(KeyBindingData.MoveForward);
            MoveBackwardInputField.text = ConvertKeyCodeToString(KeyBindingData.MoveBackward);
            MoveLeftInputField.text = ConvertKeyCodeToString(KeyBindingData.MoveLeft);
            MoveRightInputField.text = ConvertKeyCodeToString(KeyBindingData.MoveRight);
            LeanLeftInputField.text = ConvertKeyCodeToString(KeyBindingData.LeanLeft);
            LeanRightInputField.text = ConvertKeyCodeToString(KeyBindingData.LeanRight);
            ReloadInputField.text = ConvertKeyCodeToString(KeyBindingData.Reload);
            MainWeaponInputField.text = ConvertKeyCodeToString(KeyBindingData.MainWeapon);
            SubWeaponInputField.text = ConvertKeyCodeToString(KeyBindingData.SubWeapon);
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

    private string NormaizeInputCoontent(string content)
    {
        if (int.TryParse(content, out int number))
        {
            if (number >= 1 && number <= 9) return content;
            return "0" + content;
        }
        else return content.ToUpper();
    }

    private string ConvertKeyCodeToString(KeyCode keyCode)
    {
        string keyString = keyCode.ToString();
        if (keyString.Contains("Alpha", StringComparison.OrdinalIgnoreCase))
        {
            return keyString.Replace("Alpha", "");
        }
        return keyString;
    }
    #endregion
}
