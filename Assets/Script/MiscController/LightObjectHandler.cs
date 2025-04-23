using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObjectHandler : MonoBehaviour
{
    #region PROPERTIES
    public Light light;
    public float LightEnableDuration;
    public Vector2 LightIntensityRange;
    Coroutine lightCoroutine;

    bool ready = true;
    #endregion

    #region UNITY CORE
    private void OnEnable()
    {
        ready = true;
        light.enabled = false;
    }
    private void OnDisable()
    {
        ready = true;
        light.enabled = false;
        if (lightCoroutine != null) StopCoroutine(lightCoroutine);
        lightCoroutine = null;
    }
    #endregion

    #region MAIN
    public void LightEnable()
    {
        if (!ready) return;
        if (lightCoroutine != null) StopCoroutine(lightCoroutine);
        float _Intensity = Random.Range(LightIntensityRange.x, LightIntensityRange.y);
        StartCoroutine(LightEnableCoroutine(_Intensity));
    }

    private IEnumerator LightEnableCoroutine(float intensity)
    {
        ready = false;
        light.intensity = intensity;
        light.enabled = true;
        yield return new WaitForSeconds(LightEnableDuration);
        light.enabled = false;
        ready = true;
    }

    public void LightFlashLoop(int flashCounts = 3)
    {
        StartCoroutine(LightEnableCoroutine(flashCounts));
    }

    private IEnumerator LightEnableCoroutine(int flashCounts = 3)
    {
        for (int i = 0; i < flashCounts; i++)
        {
            light.intensity = Random.Range(LightIntensityRange.x, LightIntensityRange.y);
            light.enabled = true;
            float duration = Random.Range(0, LightEnableDuration);
            yield return new WaitForSeconds(duration);
            light.enabled = false;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void DisableLight()
    {
        if (lightCoroutine != null) StopCoroutine(lightCoroutine);
        light.enabled = false;
    }
    #endregion
}
