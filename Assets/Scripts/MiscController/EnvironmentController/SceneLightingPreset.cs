using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using static SceneSharedAttributes;

[CreateAssetMenu(fileName = "SceneLightingPreset", menuName = "SceneSettingAsset/SceneLightingPreset")]
public class SceneLightingPreset : ScriptableObject
{
    #region PROPERTIES
    [Header("WEATHER OVERRIDES")]
    public SceneLightingSettingPreset ClearLightingSetting;
    public SceneLightingSettingPreset FoggyLightingSetting;
    public SceneLightingSettingPreset OvercastLightingSetting;
    public SceneLightingSettingPreset SnowLightingSetting;
    public SceneLightingSettingPreset LightRainLightingSetting;
    public SceneLightingSettingPreset MediumRainLightingSetting;
    public SceneLightingSettingPreset HeavyRainLightingSetting;
    public SceneLightingSettingPreset StormLightingSetting;

    private Light sun;
    #endregion

    #region METHODS
    [Button("APPLY")]
    public void ApplyDefault()
    {
        if (sun == null) sun = GameObject.FindGameObjectWithTag("SunAndMoon").GetComponent<Light>();
        SceneLightingSettingPreset targetPreset = ClearLightingSetting;

        sun.color = targetPreset.WorldLightColor;
        sun.intensity = targetPreset.WorldLightIntensity;

        if (targetPreset.SkyBox != null) RenderSettings.skybox = targetPreset.SkyBox;
        Material skyBoxMaterial = RenderSettings.skybox;
        skyBoxMaterial.SetColor("_Tint", targetPreset.SkyBoxColor);
        RenderSettings.ambientMode = targetPreset.AmbientMode;
        switch (targetPreset.AmbientMode)
        {
            case AmbientMode.Skybox:
                RenderSettings.ambientIntensity = targetPreset.Intensity;
                break;
            case AmbientMode.Trilight:
                RenderSettings.ambientSkyColor = targetPreset.SkyColor;
                RenderSettings.ambientEquatorColor = targetPreset.EquatorColor;
                RenderSettings.ambientGroundColor = targetPreset.GroundColor;
                break;
            case AmbientMode.Flat:
                RenderSettings.ambientLight = targetPreset.AmbientColor;
                break;
        }
        if (targetPreset.FogEnabled)
        {
            RenderSettings.fog = targetPreset.FogEnabled;
            RenderSettings.fogColor = targetPreset.FogColor;
            RenderSettings.fogMode = targetPreset.FogMode;
            RenderSettings.fogStartDistance = targetPreset.StartDistance;
            RenderSettings.fogEndDistance = targetPreset.EndDistance;
            RenderSettings.fogDensity = targetPreset.FogIntensity;
        }
    }

    [Button("APPLY SMOOTHLY")]
    public void ApplySmoothly()
    {
        if (sun == null) sun = GameObject.FindGameObjectWithTag("SunAndMoon").GetComponent<Light>();
        SceneLightingSettingPreset targetPreset = ClearLightingSetting;

        var currentLightIntensity = sun.intensity;
        DOTween.To(() => currentLightIntensity, x => sun.intensity = x, targetPreset.WorldLightIntensity, 2);
        var currentSunColor = sun.color;
        DOTween.To(() => currentSunColor, x => sun.color = x, targetPreset.WorldLightColor, 2);

        if (targetPreset.SkyBox != null) RenderSettings.skybox = targetPreset.SkyBox;
        Material skyBoxMaterial = RenderSettings.skybox;
        Color fromColor = skyBoxMaterial.GetColor("_Tint");
        DOTween.To(() => fromColor, x => skyBoxMaterial.SetColor("_Tint", x), targetPreset.SkyBoxColor, 2f);
        RenderSettings.ambientMode = targetPreset.AmbientMode;
        switch (targetPreset.AmbientMode)
        {
            case AmbientMode.Skybox:
                var currentAmbientIntensity = RenderSettings.ambientIntensity;
                DOTween.To(() => currentAmbientIntensity, x => RenderSettings.ambientIntensity = x, targetPreset.Intensity, 2);
                break;
            case AmbientMode.Trilight:
                var currentSkyColor = RenderSettings.ambientSkyColor;
                var currentEquatorColor = RenderSettings.ambientEquatorColor;
                var currentGroundColor = RenderSettings.ambientGroundColor;
                DOTween.To(() => currentSkyColor, x => RenderSettings.ambientSkyColor = x, targetPreset.SkyColor, 2);
                DOTween.To(() => currentEquatorColor, x => RenderSettings.ambientEquatorColor = x, targetPreset.EquatorColor, 2);
                DOTween.To(() => currentGroundColor, x => RenderSettings.ambientGroundColor = x, targetPreset.GroundColor, 2);
                break;
            case AmbientMode.Flat:
                var currentAmbientColor = RenderSettings.ambientLight;
                DOTween.To(() => currentAmbientColor, x => RenderSettings.ambientLight = x, targetPreset.AmbientColor, 2);
                break;
        }
        if (targetPreset.OverrideFog)
        {
            RenderSettings.fog = targetPreset.FogEnabled;
            RenderSettings.fogMode = targetPreset.FogMode;
            var currentFogColor = RenderSettings.fogColor;
            DOTween.To(() => currentFogColor, x => RenderSettings.fogColor = x, targetPreset.FogColor, 2);
            DOTween.To(() => RenderSettings.fogStartDistance, x => RenderSettings.fogStartDistance = x, targetPreset.StartDistance, 2);
            DOTween.To(() => RenderSettings.fogEndDistance, x => RenderSettings.fogEndDistance = x, targetPreset.EndDistance, 2);
            DOTween.To(() => RenderSettings.fogDensity, x => RenderSettings.fogDensity = x, targetPreset.FogIntensity, 2);
        }
    }

    public void ApplySmoothly(Weather weather)
    {
        if (sun == null) sun = GameObject.FindGameObjectWithTag("SunAndMoon").GetComponent<Light>();
        SceneLightingSettingPreset targetPreset = null;

        switch (weather)
        {
            case Weather.Clear:         targetPreset = ClearLightingSetting;        break;
            case Weather.Foggy:         targetPreset = FoggyLightingSetting;        break;
            case Weather.Overcast:      targetPreset = OvercastLightingSetting;     break;
            case Weather.Snowing:       targetPreset = SnowLightingSetting;         break;
            case Weather.LightRain:     targetPreset = LightRainLightingSetting;    break;
            case Weather.MediumRain:    targetPreset = MediumRainLightingSetting;   break;
            case Weather.HeavyRain:     targetPreset = HeavyRainLightingSetting;    break;
            case Weather.Storm:         targetPreset = StormLightingSetting;        break;
        }

        var currentLightIntensity = sun.intensity;
        DOTween.To(() => currentLightIntensity, x => sun.intensity = x, targetPreset.WorldLightIntensity, 2);
        var currentSunColor = sun.color;
        DOTween.To(() => currentSunColor, x => sun.color = x, targetPreset.WorldLightColor, 2);

        if (targetPreset.SkyBox != null) RenderSettings.skybox = targetPreset.SkyBox;
        Material skyBoxMaterial = RenderSettings.skybox;
        Color fromColor = skyBoxMaterial.GetColor("_Tint");
        DOTween.To(() => fromColor, x => skyBoxMaterial.SetColor("_Tint", x), targetPreset.SkyBoxColor, 2f);
        RenderSettings.ambientMode = targetPreset.AmbientMode;
        switch (targetPreset.AmbientMode)
        {
            case AmbientMode.Skybox:
                var currentAmbientIntensity = RenderSettings.ambientIntensity;
                DOTween.To(() => currentAmbientIntensity, x => RenderSettings.ambientIntensity = x, targetPreset.Intensity, 2);
                break;
            case AmbientMode.Trilight:
                var currentSkyColor = RenderSettings.ambientSkyColor;
                var currentEquatorColor = RenderSettings.ambientEquatorColor;
                var currentGroundColor = RenderSettings.ambientGroundColor;
                DOTween.To(() => currentSkyColor, x => RenderSettings.ambientSkyColor = x, targetPreset.SkyColor, 2);
                DOTween.To(() => currentEquatorColor, x => RenderSettings.ambientEquatorColor = x, targetPreset.EquatorColor, 2);
                DOTween.To(() => currentGroundColor, x => RenderSettings.ambientGroundColor = x, targetPreset.GroundColor, 2);
                break;
            case AmbientMode.Flat:
                var currentAmbientColor = RenderSettings.ambientLight;
                DOTween.To(() => currentAmbientColor, x => RenderSettings.ambientLight = x, targetPreset.AmbientColor, 2);
                break;
        }
        if (targetPreset.OverrideFog)
        {
            RenderSettings.fog = targetPreset.FogEnabled;
            RenderSettings.fogMode = targetPreset.FogMode;
            var currentFogColor = RenderSettings.fogColor;
            DOTween.To(() => currentFogColor, x => RenderSettings.fogColor = x, targetPreset.FogColor, 2);
            DOTween.To(() => RenderSettings.fogStartDistance, x => RenderSettings.fogStartDistance = x, targetPreset.StartDistance, 2);
            DOTween.To(() => RenderSettings.fogEndDistance, x => RenderSettings.fogEndDistance = x, targetPreset.EndDistance, 2);
            DOTween.To(() => RenderSettings.fogDensity, x => RenderSettings.fogDensity = x, targetPreset.FogIntensity, 2);
        }
    }

    #endregion
}