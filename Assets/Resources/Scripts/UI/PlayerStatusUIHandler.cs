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

    [Header("DAMAGE VFX")]
    public CanvasGroup FullScreenVfxCanavs;
    public List<Animator> DamageFxAnimator = new List<Animator>();

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
        PlayerControlEventMananger.OnPlayerHealthChanged?.AddListener(OnHealthChanged);
        PlayerControlEventMananger.OnPlayerStaminaChanged?.AddListener(OnStaminaChanged);
    }
    private void UnRegisterAllEvents()
    {
        PlayerControlEventMananger.OnPlayerHealthChanged?.RemoveListener(OnHealthChanged);
        PlayerControlEventMananger.OnPlayerStaminaChanged?.RemoveListener(OnStaminaChanged);
    }
    private void OnHealthChanged(float currentHP, float maxHP, bool healing = false)
    {
        float ratio = currentHP / maxHP;
        HealthBar.color = ratio >= 0.5f ? NormalHealthBarColor : DangerHealthBarColor;
        HealthBar.DOFillAmount(ratio, 0.5f);

        if (!healing)
        {
            FullScreenVfxCanavs.alpha = 1;
            var vfx = DamageFxAnimator.GetRandom();
            vfx.Rebind();
        }
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
        FullScreenVfxCanavs.alpha = 0;
        HealthBar.fillAmount = 0f;
        StaminaBar.fillAmount = 0f;
    }
    #endregion
}
