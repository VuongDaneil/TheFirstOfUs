using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUIHandler : MonoBehaviour
{
    #region PROPERTIES
    [Header("UI ELEMENT - HP")]
    public Image HealthBar;
    public Color NormalHealthBarColor = Color.green;
    public Color DangerHealthBarColor = Color.red;

    [Header("UI ELEMENT - STAMINA")]
    public Image StaminaBar;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        ResetStatusBarOnAwake();
        RegisterAllEvents();
    }

    private void OnDestroy()
    {
        UnRegisterAllEvents();
    }
    #endregion

    #region MAIN

    #region _events
    private void RegisterAllEvents()
    {
        PlayerControlEventsMananger.OnPlayerHealthChanged?.AddListener(OnHealthChanged);
        PlayerControlEventsMananger.OnPlayerStaminaChanged?.AddListener(OnStaminaChanged);
    }
    private void UnRegisterAllEvents()
    {
        PlayerControlEventsMananger.OnPlayerHealthChanged?.RemoveListener(OnHealthChanged);
        PlayerControlEventsMananger.OnPlayerStaminaChanged?.RemoveListener(OnStaminaChanged);
    }
    private void OnHealthChanged(float currentHP, float maxHP)
    {
        float ratio = currentHP / maxHP;
        HealthBar.color = ratio >= 0.5f ? NormalHealthBarColor : DangerHealthBarColor;
        HealthBar.DOFillAmount(ratio, 0.5f);
    }
    private void OnStaminaChanged(float currentStamina, float maxStamina)
    {
        float ratio = currentStamina / maxStamina;
        StaminaBar.DOFillAmount(ratio, 0.5f);
    }
    #endregion

    #endregion

    #region SUPPORTIVE
    private void ResetStatusBarOnAwake()
    {
        HealthBar.fillAmount = 0f;
        StaminaBar.fillAmount = 0f;
    }
    #endregion
}
