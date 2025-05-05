using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "SceneLightingPreset", menuName = "SceneSettingAsset/SceneLightingPreset")]
public class SceneLightingPreset : ScriptableObject
{
    #region PROPERTIES
    [Header("ENVIRONMENT")]
    public Material SkyBox;
    public Color RealtimeShadowColor;

    [HorizontalLine(2, EColor.Blue)]
    [Header("ENVIRONMENT LIGHTING")]
    [Label("Source")] public AmbientMode AmbientMode = AmbientMode.Skybox;

    // ___ SKYBOX MODE ___
    [ShowIf("AnimbientMode", AmbientMode.Skybox)][Label("Intensity Multiplier")] [Range(0f, 8f)] public float Intensity = 0.25f;

    // ___ GRADIENT MODE ___
    [ShowIf("AnimbientMode", AmbientMode.Trilight)][Label("Sky Color")] public Color SkyColor;
    [ShowIf("AnimbientMode", AmbientMode.Trilight)][ColorUsage(true, true)][Label("Equator Color")] public Color EquatorColor;
    [ShowIf("AnimbientMode", AmbientMode.Trilight)][ColorUsage(true, true)][Label("Ground Color")] public Color GroundColor;

    // ___ COLOR MODE ___
    [ShowIf("AnimbientMode", AmbientMode.Flat)][Label("Ambient Color")] public Color AmbientColor;

    [HorizontalLine(2, EColor.Blue)]
    [Header("FOG")]
    public bool FogEnabled;
    [ShowIf("FogEnabled")][Label("Color")] public Color FogColor;
    [ShowIf("FogEnabled")][Label("Mode")] public FogMode FogMode = FogMode.Linear;
    [ShowIf("FogEnabled")][Label("Start Distance")] public float StartDistance = 0f;
    [ShowIf("FogEnabled")][Label("End Distance")] public float EndDistance = 300f;
    #endregion

    #region METHODS
    [Button("APPLY")]
    public void Apply()
    {
        if (SkyBox != null) RenderSettings.skybox = SkyBox;
        RenderSettings.ambientMode = AmbientMode;
        switch (AmbientMode)
        {
            case AmbientMode.Skybox:
                RenderSettings.ambientIntensity = Intensity;
                break;
            case AmbientMode.Trilight:
                RenderSettings.ambientSkyColor = SkyColor;
                RenderSettings.ambientEquatorColor = EquatorColor;
                RenderSettings.ambientGroundColor = GroundColor;
                break;
            case AmbientMode.Flat:
                RenderSettings.ambientLight = AmbientColor;
                break;
        }
        if (FogEnabled)
        {
            RenderSettings.fog = FogEnabled;
            RenderSettings.fogColor = FogColor;
            RenderSettings.fogMode = FogMode;
            RenderSettings.fogStartDistance = StartDistance;
            RenderSettings.fogEndDistance = EndDistance;
        }
    }
    #endregion
}