using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class EnemiesSpawningManager : MonoBehaviour
{
    public bool Ready = false;
    public bool Stop = false;

    [Header("NORMAL ZOMBIES - CONFIG")]
    public List<NormalZombieActor> NormalZombiePrefabs;
    public List<EnemyConfig> NormalZombieConfigs;

    [Header("NORMAL ZOMBIES - SPAWN")]
    public int MaxNormalZombiesSpawned = 100;
    public int MinDistanceSpawnToPlayer = 150;
    public int MaxDistanceSpawnToPlayer = 150;
    public Vector2 SpawnRate = new Vector2(3, 10);
    public List<Transform> SpawnPoints;
    private int frameCounterCheckDistance = 0;
    private float SpawnTimer = 0f;

    [Header("NORMAL ZOMBIES - DEBUG")]
    [ReadOnly] public int NumberOfNormalZombieAlive = 0;
    public List<NormalZombieActor> ActiveNormalZombies = new List<NormalZombieActor>();
    public List<NormalZombieActor> PooledNormalZombies = new List<NormalZombieActor>();
    private Transform PlayerTransform;


    #region UNITY CORE
    private void Awake()
    {
        Ready = false;
        RegisterAllEvents();
    }

    private void Update()
    {
        if (!Ready || Stop || !PlayerBrain.Instance.IsAlive) return;

        #region spawn automatically
        if (SpawnTimer <= 0)
        {
            SpawnTimer = Random.Range(SpawnRate.x, SpawnRate.y);
            if (NumberOfNormalZombieAlive < MaxNormalZombiesSpawned)
            {
                SpawnNormalZombie();
            }
        }
        else
        {
            SpawnTimer -= Time.deltaTime;
        }
        #endregion

        #region check enemy distance to player
        frameCounterCheckDistance++;
        if (frameCounterCheckDistance >= 15)
        {
            frameCounterCheckDistance = 0;
            if (NumberOfNormalZombieAlive > 0)
            {
                Vector3 playerPosition = PlayerTransform.position;
                var botToDespawn = new List<NormalZombieActor>();
                foreach (var zombie in ActiveNormalZombies)
                {
                    if (Vector3.SqrMagnitude(zombie.ActorTransform.position - playerPosition) > MaxDistanceSpawnToPlayer * MaxDistanceSpawnToPlayer)
                    {
                        botToDespawn.Add(zombie);
                    }
                }
                foreach (var zombie in botToDespawn)
                {
                    DespawnBot(zombie);
                    SpawnNormalZombie();
                }
            }
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.Y)) KillAllZombie();
        if (Input.GetKeyDown(KeyCode.U)) SpawnZombies();
    }

    private void OnDestroy()
    {
        UnRegisterAllEvents();
    }
    #endregion

    
    #region MAIN

    #region _events
    private void RegisterAllEvents()
    {
        GameplayEventManager.OnAnEnemyDead.AddListener(OnAnEnemyDead);
        GameplayEventManager.OnGameEnd.AddListener(DespawnAllEnemies);
        GameplayEventManager.OnPlayerIntialized?.AddListener(OnPlayerInitialized);
    }

    private void UnRegisterAllEvents()
    {
        GameplayEventManager.OnAnEnemyDead.RemoveListener(OnAnEnemyDead);
        GameplayEventManager.OnGameEnd.RemoveListener(DespawnAllEnemies);
        GameplayEventManager.OnPlayerIntialized.RemoveListener(OnPlayerInitialized);
    }

    private void OnPlayerInitialized()
    {
        PlayerTransform = PlayerBrain.Instance.transform;
        StartCoroutine(SpawnStartNormalZombies());
        Ready = true;
    }

    #endregion

    #region _normal zombies

    private void OnAnEnemyDead(IActor deadZombie)
    {
        NumberOfNormalZombieAlive--;
        PooledNormalZombies.Add(deadZombie as NormalZombieActor);
    }

    private IEnumerator SpawnStartNormalZombies()
    {
        for (int i = 0; i < MaxNormalZombiesSpawned / 2f; i++)
        {
            if (!Stop) SpawnNormalZombie();
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
        }
    }
    private void SpawnNormalZombie()
    {
        var newBornZombie = GetNormalZombiePrefab();
        if (newBornZombie == null) return;

        Vector3 spawnPoint = Vector3.zero;
        spawnPoint = ChooseSpawnPointNearPlayer();

        newBornZombie.Spawn(NormalZombieConfigs.GetRandom(), spawnPoint);
        ActiveNormalZombies.Add(newBornZombie);
        NumberOfNormalZombieAlive++;
    }
    #endregion

    #endregion

    #region SUPPORTIVE
    private void KillAllZombie()
    {
        var botToDespawn = new List<NormalZombieActor>();
        foreach (var zombie in ActiveNormalZombies)
        {
            botToDespawn.Add(zombie);
        }
        foreach (var zombie in botToDespawn)
        {
            DespawnBot(zombie);
        }
    }

    private void SpawnZombies()
    {
        for (int i = 0; i < 10; i++)
        {
            SpawnNormalZombie();
        }
    }
    private void DespawnBot(NormalZombieActor zombie)
    {
        if (zombie == null) return;
        zombie.gameObject.SetActive(false);
        PooledNormalZombies.Add(zombie);
        ActiveNormalZombies.Remove(zombie);
        NumberOfNormalZombieAlive--;
    }
    private NormalZombieActor GetNormalZombiePrefab()
    {
        NormalZombieActor result = null;
        if (PooledNormalZombies.Count <= 0)
        {
            result = Instantiate(ChooseZombieModel());
        }
        else
        {
            try
            {
                result = PooledNormalZombies.First(x => !x.gameObject.activeSelf);
                if (result != null) PooledNormalZombies.Remove(result);
            }
            catch { result = Instantiate(ChooseZombieModel()); }
        }
        return result;
    }

    private Vector3 ChooseSpawnPointNearPlayer()
    {
        Vector3 playerPosition = PlayerTransform.position;
        for (int i = 0; i < 10; i++)
        {
            float distanceToSpawn = Random.Range(MinDistanceSpawnToPlayer, MaxDistanceSpawnToPlayer);
            Vector3 randomPoint = playerPosition + Random.insideUnitSphere * distanceToSpawn;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return SpawnPoints.GetRandom().position; ;
    }

    private NormalZombieActor ChooseZombieModel()
    {
        int modelTypesCount = NormalZombiePrefabs.Count;
        return NormalZombiePrefabs[(NumberOfNormalZombieAlive % modelTypesCount)];
    }

    private void DespawnAllEnemies()
    {
        foreach (var enemies in ActiveNormalZombies) enemies.gameObject.SetActive(false);
    }
    #endregion
}
