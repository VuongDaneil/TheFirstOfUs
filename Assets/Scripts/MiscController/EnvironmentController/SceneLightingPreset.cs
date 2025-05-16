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
    public SceneLightingSettingPreset ClearLightingSetting;
    public SceneLightingSettingPreset FoggyLightingSetting;
    public SceneLightingSettingPreset SunnyLightingSetting;
    public SceneLightingSettingPreset OvercastLightingSetting;
    public SceneLightingSettingPreset SnowLightingSetting;
    public SceneLightingSettingPreset SnowStormLightingSetting;
    public SceneLightingSettingPreset LightRainLightingSetting;
    public SceneLightingSettingPreset MediumRainLightingSetting;
    public SceneLightingSettingPreset HeavyRainLightingSetting;
    public SceneLightingSettingPreset StormLightingSetting;

    [Header("ENVIRONMENT")]
    public Material SkyBox;
    public Color SkyBoxColor;
    public Color RealtimeShadowColor;

    [HorizontalLine(2, EColor.Blue)]
    [Header("SUN/MOON LIGHT SOURCE")]
    [Label("Color")] public Color WorldLightColor = Color.white;
    [Label("Intensity")] public float WorldLightIntensity = 1f;

    [HorizontalLine(2, EColor.Blue)]
    [Header("ENVIRONMENT LIGHTING")]
    [Label("Source")] public AmbientMode AmbientMode = AmbientMode.Skybox;
    private bool isColorMode => AmbientMode == AmbientMode.Flat || AmbientMode == AmbientMode.Trilight;

    // ___ SKYBOX MODE ___
    [ShowIf("AmbientMode", AmbientMode.Skybox)][Label("Intensity Multiplier")] [Range(0f, 8f)] public float Intensity = 0.25f;

    // ___ COLOR MODE ___
    [ShowIf("AmbientMode", AmbientMode.Flat)][Label("Ambient Color")] public Color AmbientColor;

    // ___ GRADIENT MODE ___
    [ShowIf("AmbientMode", AmbientMode.Trilight)][ColorUsage(true, true)][Label("Sky Color")] public Color SkyColor;
    [ShowIf("AmbientMode", AmbientMode.Trilight)][ColorUsage(true, true)][Label("Equator Color")] public Color EquatorColor;
    [ShowIf("AmbientMode", AmbientMode.Trilight)][ColorUsage(true, true)][Label("Ground Color")] public Color GroundColor;

    [HorizontalLine(2, EColor.Blue)]
    [Header("FOG")]
    public bool FogEnabled;
    [ShowIf("FogEnabled")][Label("Color")] public Color FogColor;
    [ShowIf("FogEnabled")][Label("Mode")] public FogMode FogMode = FogMode.Linear;
    [ShowIf("FogEnabled")][Label("Start Distance")] public float StartDistance = 0f;
    [ShowIf("FogEnabled")][Label("End Distance")] public float EndDistance = 300f;
    [ShowIf("FogEnabled")][Label("Intensity")] public float FogIntensity;

    private Light sun;
    #endregion

    #region METHODS
    [Button("APPLY")]
    public void Apply()
    {
        if (sun == null) sun = GameObject.FindGameObjectWithTag("SunAndMoon").GetComponent<Light>();
        SceneLightingSettingPreset targetPreset = ClearLightingSetting;

        sun.color = targetPreset.WorldLightColor;
        sun.intensity = targetPreset.WorldLightIntensity;

        if (SkyBox != null) RenderSettings.skybox = targetPreset.SkyBox;
        Material skyBoxMaterial = RenderSettings.skybox;
        skyBoxMaterial.SetColor("_Tint", SkyBoxColor);
        RenderSettings.ambientMode = targetPreset.AmbientMode;
        switch (AmbientMode)
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
        if (FogEnabled)
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

        if (SkyBox != null) RenderSettings.skybox = targetPreset.SkyBox;
        Material skyBoxMaterial = RenderSettings.skybox;
        Color fromColor = skyBoxMaterial.GetColor("_Tint");
        DOTween.To(() => fromColor, x => skyBoxMaterial.SetColor("_Tint", x), targetPreset.SkyBoxColor, 2f);
        RenderSettings.ambientMode = targetPreset.AmbientMode;
        switch (AmbientMode)
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
        if (FogEnabled)
        {
            RenderSettings.fog = FogEnabled;
            RenderSettings.fogMode = FogMode;
            var currentFogColor = RenderSettings.fogColor;
            DOTween.To(() => currentFogColor, x => RenderSettings.fogColor = x, targetPreset.FogColor, 2);
            DOTween.To(() => RenderSettings.fogStartDistance, x => RenderSettings.fogStartDistance = x, targetPreset.StartDistance, 2);
            DOTween.To(() => RenderSettings.fogEndDistance, x => RenderSettings.fogEndDistance = x, targetPreset.EndDistance, 2);
            DOTween.To(() => RenderSettings.fogDensity, x => RenderSettings.fogDensity = x, targetPreset.FogIntensity, 2);
        }
    }

    [Button("GET CURRENT SCENE SETTING")]
    public void GetCurrentSceneSetting()
    {
        SkyBox = RenderSettings.skybox;
        AmbientMode = RenderSettings.ambientMode;
        switch (AmbientMode)
        {
            case AmbientMode.Skybox:
                Intensity = RenderSettings.ambientIntensity;
                break;
            case AmbientMode.Trilight:
                SkyColor = RenderSettings.ambientSkyColor;
                EquatorColor = RenderSettings.ambientEquatorColor;
                GroundColor = RenderSettings.ambientGroundColor;
                break;
            case AmbientMode.Flat:
                AmbientColor = RenderSettings.ambientLight;
                break;
        }
        FogEnabled = RenderSettings.fog;
        FogColor = RenderSettings.fogColor;
        FogMode = RenderSettings.fogMode;
        StartDistance = RenderSettings.fogStartDistance;
        EndDistance = RenderSettings.fogEndDistance;
        FogIntensity = RenderSettings.fogDensity;
    }

    #endregion
}