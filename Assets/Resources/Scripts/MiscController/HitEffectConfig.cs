using UnityEngine;

[CreateAssetMenu(fileName = "HitEffectPrefab", menuName = "EffectSetup/HitEffectSetup")]
public class HitEffectConfig : ScriptableObject
{
    public GameObject[] FleshHitEffect;
    public GameObject WoodHitEffect;
    public GameObject MetalHitEffect;
    public GameObject DirtHitEffect;
    public GameObject WaterHitEffect;
    public GameObject RockHitEffect;
    public GameObject BrickHitEffect;
    public GameObject ConcreteHitEffect;
    public GameObject LeavesHitEffect;

    public GameObject GetHitEffect(HitEffectType effectType)
    {
        switch (effectType)
        {
            case HitEffectType.Flesh:
                return Instantiate(FleshHitEffect.GetRandom());
            case HitEffectType.Wood:
                return Instantiate(WoodHitEffect);
            case HitEffectType.Metal:
                return Instantiate(MetalHitEffect);
            case HitEffectType.Dirt:
                return Instantiate(DirtHitEffect);
            case HitEffectType.Water:
                return Instantiate(WaterHitEffect);
            case HitEffectType.Rock:
                return Instantiate(RockHitEffect);
            case HitEffectType.Brick:
                return Instantiate(BrickHitEffect);
            case HitEffectType.Concrete:
                return Instantiate(ConcreteHitEffect);
            case HitEffectType.Leaves:
                return Instantiate(LeavesHitEffect);
            default:
                return null;
        }
    }
}
