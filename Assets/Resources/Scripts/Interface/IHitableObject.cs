using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitableObject
{
    public HitEffectType HitEffectType { get; }
    public void OnHit(RaycastHit hitPoint);
}

public enum HitEffectType
{
    None,
    Wood,
    Metal,
    Dirt,
    Water,
    Flesh,
    Rock,
    Brick,
    Concrete,
    Leaves,
}