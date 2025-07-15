using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectPool : MonoBehaviour
{
    public static HitEffectPool Instance;

    public List<ObjectHitEffectControl> UnUsedEffect = new List<ObjectHitEffectControl>();
    public HitEffectConfig HitEffectConfigs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetFromPool(HitEffectType hitEffectType)
    {
        if (UnUsedEffect.Count == 0)
        {
            return HitEffectConfigs.GetHitEffect(hitEffectType);
        }
        else
        {
            UnUsedEffect.Shuffle();
            ObjectHitEffectControl hitableObject = UnUsedEffect.Find(x => x.HitType == hitEffectType);
            if (hitableObject != null) UnUsedEffect.Remove(hitableObject);
            else hitableObject = HitEffectConfigs.GetHitEffect(hitEffectType).GetComponent<ObjectHitEffectControl>();
            return hitableObject.gameObject;
        }
    }

    public void PushToPool(ObjectHitEffectControl hitableEffect)
    {
        if (hitableEffect == null) return;
        UnUsedEffect.Add(hitableEffect);
    }
}