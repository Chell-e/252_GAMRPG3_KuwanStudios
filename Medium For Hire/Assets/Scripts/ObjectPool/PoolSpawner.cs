using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PoolSpawner : MonoBehaviour
{
    // ==== FROM LEVEL SELECT MANAGER
    [SerializeField] private EnemyPoolData currentNormalPool;
    [SerializeField] private EnemyPoolData currentElitePool;
    [SerializeField] private BossData currentBoss;

        [Header("Wave Progression")]
            [Tooltip("Initial time between spawns")]
    public float initialSpawnInterval = 1.8f;
            [Tooltip("Fastest possible spawn interval")]
    public float minimumSpawnInterval = 0.25f;
            [Tooltip("Modifies spawn rate per wave")]
    public float spawnRateMultiplier = 0.9f;

        [Header("Horde Size")]
            [Tooltip("Base number of enemies per wave")]
    public int baseHordeSize = 12;
            [Tooltip("Number of enemies added each wave")]
    public int hordeSizeIncrease = 7;

            [Tooltip("How often a new wave triggers (in seconds)")]
    public float timePerWave = 60f;

        [Header("Boss")]
            [Tooltip("Time when boss spawns")]
    public float bossSpawnTime = 600f;  // 10 minutes

        [Header("Elite")]
            [Tooltip("Time between elite spawns")]
    public float eliteSpawnInterval = 60f;

        [Header("Difficulty Scaling")]
            [Tooltip("Enemy stats multiplier per wave (HP, DMG)")]
    public float statMultiplierPerWave = 1.12f;

        [Header("Enemy Cap")]
            [Tooltip("Max enemies (normal + elite) on screen")]
    public int maxEnemiesOnScreen = 100;

        [Header("Spawn Position")]
            [Tooltip("How far off-screen to spawn (in viewport units)")]
    public float spawnEdgeOffset = 1.5f;

        [Header("Enemy Unlock Progression")]
            [Tooltip("Seconds required to unlock a new enemy type")]
    private float timeToUnlockNextEnemy = 60f;

    private float currentSpawnInterval;
    private float spawnTimer = 0f;
    private float eliteTimer = 0f;
    private float waveTimer = 0f;
    private float elapsedTime = 0f;

    private int currentWave = 0;
    private int enemiesToSpawnThisWave;
    private int enemiesSpawnedThisWave = 0;

    private bool bossSpawned = false;

    public int CurrentWave => currentWave;
    public float ElapsedTime => elapsedTime;
    public int CurrentEnemyCount => GetCurrentEnemyCount();
    public bool IsAtEnemyCap() => maxEnemiesOnScreen > 0 && GetCurrentEnemyCount() >= maxEnemiesOnScreen;

    private bool HasNormalEnemies() => currentNormalPool != null && currentNormalPool.enemyPool != null && currentNormalPool.enemyPool.Count > 0;
    private bool HasEliteEnemies() => currentElitePool != null && currentElitePool.enemyPool != null && currentElitePool.enemyPool.Count > 0;


    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        enemiesToSpawnThisWave = baseHordeSize;
    }

    public void SetUpLevelData(LevelData levelData)
    {
        if (levelData == null)
            return;

        currentNormalPool = levelData.normalEnemies;
        currentElitePool = levelData.eliteEnemies;
        currentBoss = levelData.boss;

        // clean up level
        ResetSpawnerState();
    }

    private void ResetSpawnerState()
    {
        elapsedTime = 0f;

        currentWave = 0;
        waveTimer = 0f;

        spawnTimer = 0f;
        eliteTimer = 0f;

        enemiesSpawnedThisWave = 0;
        bossSpawned = false;

        currentSpawnInterval = initialSpawnInterval;
        enemiesToSpawnThisWave = baseHordeSize;
    }

    private void Update()
    {
        if (PlayerController.Instance == null || !PlayerController.Instance.gameObject.activeSelf) return;

        elapsedTime += Time.deltaTime;
        waveTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;
        eliteTimer += Time.deltaTime;

        // --- normal enemy spawning
        if (spawnTimer >= currentSpawnInterval && HasNormalEnemies() && !IsAtEnemyCap())
        {
            spawnTimer -= currentSpawnInterval;
            SpawnNormalEnemy();
            enemiesSpawnedThisWave++;
            
        }

        // --- elite spawning
        if (eliteTimer >= eliteSpawnInterval && HasEliteEnemies() && !IsAtEnemyCap())
        {
            eliteTimer -= eliteSpawnInterval;
            SpawnElite();
            
        }

        // --- wave progression
        if (waveTimer >= timePerWave)
        {
            AdvanceWave();
            waveTimer = 0f;
        }

        HandleBossSpawn();
    }

    private void AdvanceWave()
    {
        currentWave++;

        // current spawn interval is multiplied by spawnRateMultiplier each wave, but never goes below minimumSpawnInterval
        currentSpawnInterval = Mathf.Max(minimumSpawnInterval, currentSpawnInterval * spawnRateMultiplier);

        // 10 + (0 * 7) = 10, 10 + (1 * 7) = 17, 10 + (2 * 7) = 24, etc.
        enemiesToSpawnThisWave = baseHordeSize + (currentWave * hordeSizeIncrease);

        enemiesSpawnedThisWave = 0; // reset count for new wave
    }

    private void SpawnNormalEnemy()
    {
        GameObject prefab = GetRandomPrefabFromPool(currentNormalPool);
        if (prefab == null)
            return;

        GameObject enemy = PoolManager.SpawnObject(prefab, GetRandomSpawnPosition(), Quaternion.identity, PoolManager.PoolType.Enemy);

        // apply scaling
        ApplyScaling(enemy);
    }

    private void SpawnElite()
    {
        GameObject prefab = GetRandomPrefabFromPool(currentElitePool);
        if (prefab == null)
            return;

        GameObject elite = PoolManager.SpawnObject(prefab, GetRandomSpawnPosition(), Quaternion.identity, PoolManager.PoolType.Enemy);

        // apply scaling
        ApplyScaling(elite);
    }

    // 0-60: first enemy only, 60-120: first enemy + second enemy
    private GameObject GetRandomPrefabFromPool(EnemyPoolData enemyPoolData)
    {
        if (enemyPoolData == null || enemyPoolData.enemyPool == null || enemyPoolData.enemyPool.Count == 0)
            return null;

        // calculate how many enemy types are currently unlocked
        int maxPossibleIndex = Mathf.FloorToInt(elapsedTime / timeToUnlockNextEnemy) + 1;

        // clamp it so it doesnt exceed list size
        int unlockedCount = Mathf.Min(maxPossibleIndex, enemyPoolData.enemyPool.Count);

        // get random enemy from unlocked enemies
        int randomIndex = Random.Range(0, unlockedCount);
        
        EnemyData data = enemyPoolData.enemyPool[randomIndex];

        // if data's not null, return prefab, otherwise return null
        return data != null ? data.enemyPrefab : null;
    }

    private void ApplyScaling(GameObject enemyObj)
    {
        if (enemyObj == null) return;

        BaseEnemy _enemy = enemyObj.GetComponent<BaseEnemy>();
        if (_enemy != null)
        {
            float multiplier = Mathf.Pow(statMultiplierPerWave, currentWave);
            _enemy.ScaleEnemyStat(multiplier);
        }
    }

    private void HandleBossSpawn()
    {
        if (bossSpawned || currentBoss == null)
            return;

        if (elapsedTime >= bossSpawnTime)
        {
            Vector2 pos = GetRandomSpawnPosition();
            PoolManager.SpawnObject(currentBoss.bossPrefab, pos, Quaternion.identity, PoolManager.PoolType.Enemy);
            bossSpawned = true;
        }
    }

    private int GetCurrentEnemyCount()
    {
        if (PoolManager._enemyPoolEmpty == null) return 0;

        int count = 0;
        foreach (Transform child in PoolManager._enemyPoolEmpty.transform)
        {
            if (child.gameObject.activeSelf)
                count++;
        }
        return count;
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 pos = Random.value > 0.5f
            ? new Vector2(Random.value > 0.5f ? 1 - spawnEdgeOffset : spawnEdgeOffset, Random.value)
            : new Vector2(Random.value, Random.value > 0.5f ? 1 - spawnEdgeOffset : spawnEdgeOffset);

        return Camera.main.ViewportToWorldPoint(pos);
    }
}
