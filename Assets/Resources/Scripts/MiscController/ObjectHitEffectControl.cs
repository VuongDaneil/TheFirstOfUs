using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHitEffectControl : MonoBehaviour
{
    public HitEffectType HitType;
    public float LifeTime = 1f;
    private GameObject GO;

    [Header("AUDIO(s)")]
    public AudioSource ImpactAudioSource;
    public AudioClip[] ImpactAudioClip;

    private void OnValidate()
    {
        if (ImpactAudioSource == null)
        {
            ImpactAudioSource = GetComponent<AudioSource>();
        }
    }

    private void Awake()
    {
        GO = gameObject;
    }

    private void OnEnable()
    {
        if (ImpactAudioSource != null && ImpactAudioClip.Length > 0)
        {
            ImpactAudioSource.PlayOneShot(ImpactAudioClip.GetRandom());
        }
        StartCoroutine(AutoDisable());
    }

    private void OnDisable()
    {
        HitEffectPool.Instance.PushToPool(this);
    }

    private IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(LifeTime);
        GO.SetActive(false);
    }
}