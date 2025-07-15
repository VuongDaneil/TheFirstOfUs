using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GameplayEventManager
{
    public static UnityEvent OnPlayerIntialized = new UnityEvent();

    #region _quest
    public static UnityEvent<RadioTowerQuestObject> OnARadioTowerQuestCompleted = new UnityEvent<RadioTowerQuestObject>();
    public static UnityEvent<RadioCallingQuestObject> OnARadioCallingQuestCompleted = new UnityEvent<RadioCallingQuestObject>();
    #endregion

    #region _player
    /// <summary>
    /// Event triggered when the player make any noise.
    /// float parameter represents the range hearable of the noise.
    /// </summary>
    public static UnityEvent<float> OnPlayerMakeNoise = new UnityEvent<float>();
    #endregion

    #region _enemy
    public static UnityEvent<IActor> OnAnEnemyDead = new UnityEvent<IActor>();
    /// <summary>
    /// Event triggered when an enemy is attacked by the player.
    /// vector3 parameter represents the position of the enemy.
    /// </summary>
    public static UnityEvent<Vector3> OnAnEnemyAttackedByPlayer = new UnityEvent<Vector3>();
    #endregion
}