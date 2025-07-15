using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public interface IActor
{
    ActorType ActorRole { get; }
    bool IsPlayer { get; }
    int Health { get; set; }
    int MaxHealth { get; }
    int AttackPower { get; }

    void TakeDamage(int amount, IActor source = null);
    void Heal(int amount);
    void Attack(IActor target, float damageMultiplier);
    bool IsAlive { get; }
}

public enum ActorType
{
    Player,
    Ally,
    Enemy,
    NPC
}
