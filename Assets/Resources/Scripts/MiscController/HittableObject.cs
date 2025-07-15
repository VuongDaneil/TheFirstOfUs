using UnityEngine;

public class HittableObject : MonoBehaviour, IHitableObject
{
    public HitEffectType ObjectHitEffectType;
    public HitEffectType HitEffectType => ObjectHitEffectType;

    public void OnHit(RaycastHit hitPoint)
    {
        var effectGO = HitEffectPool.Instance.GetFromPool(ObjectHitEffectType);
        if (effectGO != null)
        {
            Transform hitEffectTransform = effectGO.transform;
            hitEffectTransform.position = hitPoint.point;
            hitEffectTransform.LookAt(hitPoint.point + hitPoint.normal);
            effectGO.SetActive(true);
        }
    }

    public void TakeDamage(float damage)
    {
        return;
    }
}
