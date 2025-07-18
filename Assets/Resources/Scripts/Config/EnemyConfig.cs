using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "EnemyAttributesConfig", menuName = "Actor/EnemyAttributesConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("PERSONALITY ATTRIBUTES")]
    [Range(1f, 5f)] public float Sensitive = 1f;
    [Range(1f, 5f)] public float Aggression = 1f;
    [Range(1f, 5f)] public float Brutality = 1f;
    [Range(1f, 5f)] public float Cowardice = 1f;
    [Range(1f, 5f)] public float Courage = 1f;
    [Range(1f, 5f)] public float Laziness = 1f;

    [Header("STAT ATTRIBUTES")]
    public int Health = 100;
    public float Speed = 2f;
    public float AttackPower = 10f;
    public float AttackRange = 2f;
    public float AttackRate = 2f;
    [Range(0, 100f)] public float HealthPercentageCauseStun = 10f;
    public float StunDuration = 2f;

    [Header("VISION ATTRIBUTES")]
    public float VisionRange = 20f;
    public float VisionAngle = 45f;

    [Header("HEARING ATTRIBUTES")]
    public float HearingRange = 40f;
    public float SurroundingSenseRange = 5f;

    [Header("MOVEMENT ATTRIBUTES")]
    public float RotationSpeed = 5f;
    public float RotateSpeedAcceleration = 5f;
    public float DistanceCheckDestinationStop = 5f;

    [HorizontalLine]
    [Header("BEHAVIOUR ATTRIBUTE(s)")]
    public Vector2 IdleDuration = new Vector2(5, 10);
    public Vector2 DistanceEachMoveWandering = new Vector2(4, 8);
    public float DistanceStopChasing = 50;

    [Header("ANIMATION(s)")]
    public List<string> IdleAnim = new List<string>();
    public List<string> MoveAnim = new List<string>();
    public List<string> ChaseAnim = new List<string>();
    public List<string> ChaseSlowAnim = new List<string>();
    public List<string> AttackAnim = new List<string>();
    public List<string> StunAnim = new List<string>();
    public List<string> DeadAnim = new List<string>();

    [Header("AUDIO CLIP(s)")]
    public AudioClip[] IdleAudioClips;
    public AudioClip[] MoveAudioClips;
    public AudioClip[] ChaseAudioClips;
    public AudioClip[] AttackAudioClips;
    public AudioClip[] HurtAudioClips;
    public AudioClip[] DeadAudioClips;

    #region MAIN
    public float GetIdleDuration()
    {
        return Random.Range(IdleDuration.x, IdleDuration.y);
    }

    public bool GetRandomPointOnNavMesh(Vector3 center, out Vector3 result, float distanceMultiplier = 1)
    {
        float range = Random.Range(DistanceEachMoveWandering.x, DistanceEachMoveWandering.y);
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * (range * distanceMultiplier);
            randomPoint.y += 4;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    #region _checking condition
    public bool IsTargetInChaseRange(Vector3 targetPosition, Transform actor, float rangeMultiplier)
    {
        Vector3 actorPosition = actor.position;
        float distance = Vector3.SqrMagnitude(targetPosition - actorPosition);
        float maxRangeChase = DistanceStopChasing * rangeMultiplier;
        return distance <= maxRangeChase * maxRangeChase;
    }

    public bool IsTargetInDetectRange(Vector3 targetPosition, Transform actor)
    {
        Vector3 actorPosition = actor.position;
        float distance = Vector3.SqrMagnitude(targetPosition - actorPosition);
        return distance <= VisionRange * VisionRange;
    }

    public bool IsTargetInVisionRange(Vector3 targetPosition, Transform actor)
    {
        Vector3 actorPosition = actor.position;
        float distance = Vector3.Distance(targetPosition, actor.position);
        if (distance > VisionRange) return false;

        Vector3 directionToTarget = (targetPosition - actorPosition).normalized;
        float angle = Vector3.Angle(actor.forward, directionToTarget);
        return angle <= VisionAngle / 2f;
    }

    public bool IsTargetInAttackRange(Vector3 targetPosition, Transform actor)
    {
        return Vector3.Distance(targetPosition, actor.position) <= AttackRange;
    }

    public bool IsTargetInHearingRange(Vector3 targetPosition, Transform actor, float noiseRange)
    {
        Vector3 actorPosition = actor.position;
        float distance = Vector3.SqrMagnitude(targetPosition - actorPosition);
        return (noiseRange * noiseRange) + (HearingRange * HearingRange) >= distance;
    }

    public bool IsTargetInVisionAngle(Vector3 targetPosition, Transform actor)
    {
        Vector3 directionToTarget = (targetPosition - actor.position).normalized;
        float angle = Vector3.Angle(actor.forward, directionToTarget);
        return angle <= VisionAngle / 2f;
    }

    public bool IsTargetInSurroundingSenseRange(Vector3 targetPosition, Transform actor)
    {
        Vector3 actorPosition = actor.position;
        float distance = Vector3.SqrMagnitude(targetPosition - actorPosition);
        return distance <= SurroundingSenseRange * SurroundingSenseRange;
    }

    public bool CanDealDamage(Vector3 targetPosition, Transform actor)
    {
        Vector3 myPos = actor.position;
        Vector3 directionToTarget = (targetPosition - myPos).normalized;
        float angle = Vector3.Angle(actor.forward, directionToTarget);
        if (angle <= VisionAngle / 2f)
        {
            return IsTargetInAttackRange(targetPosition, actor);
        }
        return false;
    }
    #endregion

    #region _checking personality
    public bool ChooseCourage()
    {
        float courageValue = Random.Range(0f, Courage);
        float cowardiceValue = Random.Range(0f, Cowardice);
        return courageValue > cowardiceValue;
    }

    public bool ChooseAggression()
    {
        return Random.Range(0, 5f) < Aggression;
    }

    public bool ChooseBrutality()
    {
        return Random.Range(0, 5f) < Brutality;
    }

    public bool ChooseLaziness()
    {
        return Random.Range(0, 5f) < Laziness;
    }
    #endregion

    #endregion
}