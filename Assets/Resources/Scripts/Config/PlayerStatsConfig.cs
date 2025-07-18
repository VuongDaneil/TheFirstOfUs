using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttributesConfig", menuName = "Actor/PlayerAttributesConfig")]
public class PlayerStatsConfig : ScriptableObject
{
    public int MaxHealth = 500;
    public int MaxStamina = 500;
    public float StaminaConsumeRate = 5f;
    public float WalkSpeed = 5f;
    public float RunSpeed = 10f;
    public float JumpHeight = 1.5f;
}
