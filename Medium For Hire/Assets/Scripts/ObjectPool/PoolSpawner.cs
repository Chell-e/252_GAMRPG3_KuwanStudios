using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PoolSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyEntry
    {
        public GameObject normalPrefab;
        public GameObject elitePrefab;
        public float unlockTime; // time in seconds when this enemy joins
    }

    [Header("Enemy Types")]
    public List<EnemyEntry> enemyEntries = new List<EnemyEntry>();

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

    [Header("Elite")]
    [Tooltip("Time between elite spawns")]
    public float eliteSpawnInterval = 60f;

    [Header("Difficulty Scaling")]
    [Tooltip("Enemy stats multiplier per wave (HP, DMG)")]
    public float statMultiplierPerWave = 1.12f;

    [Header("Enemy Cap")]
    [Tooltip("Max enemies (normal + elite) on screen")]
    public int maxEnemiesOnScreen = 100;

    [Header("Boss")]
    public GameObject bossPrefab;

    [Header("Spawn Position")]
    [Tooltip("How far off-screen to spawn (in viewport units)")]
    public float spawnEdgeOffset = 1.5f;

    private List<GameObject> unlockedNormals = new List<GameObject>();
    private List<GameObject> unlockedElites = new List<GameObject>();
    private int nextUnlockIndex = 0;

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
    public bool IsAtEnemyCap => maxEnemiesOnScreen > 0 && GetCurrentEnemyCount() >= maxEnemiesOnScreen;


    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        enemiesToSpawnThisWave = baseHordeSize;

        // sort enemy entries by unlock time to ensure correct unlocking order
        enemyEntries.Sort((a, b) => a.unlockTime.CompareTo(b.unlockTime));

        // unlock first enemy type immediately
        if (enemyEntries.Count > 0 && enemyEntries[0].unlockTime <= 0f)
            UnlockNextEnemyType();
    }

    private void Update()
    {
        if (PlayerController.Instance == null || !PlayerController.Instance.gameObject.activeSelf) return;

        elapsedTime += Time.deltaTime;
        waveTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;
        eliteTimer += Time.deltaTime;

        // unlock new enemy types over time
        UnlockNewEnemyTypes();

        // --- normal enemy spawning
        if (spawnTimer >= currentSpawnInterval && unlockedNormals.Count > 0)
        {
            // check if we can spawn more enemies based on cap
            if (GetCurrentEnemyCount() < maxEnemiesOnScreen || maxEnemiesOnScreen <= 0)
            {
                spawnTimer -= currentSpawnInterval;
                SpawnNormalEnemy();
                enemiesSpawnedThisWave++;
            }
        }

        // --- elite spawning
        if (eliteTimer >= eliteSpawnInterval && unlockedElites.Count > 0)
        {
            // same cap check for elites
            if (GetCurrentEnemyCount() < maxEnemiesOnScreen || maxEnemiesOnScreen <= 0)
            {
                eliteTimer -= eliteSpawnInterval;
                SpawnElite();
            }
        }

        // --- wave progression
        if (waveTimer >= timePerWave)
        {
            AdvanceWave();
            waveTimer = 0f;
        }

        HandleBossSpawn();


    }

    private void UnlockNewEnemyTypes()
    {
        // unlock any enemy types whose unlock time has passed
        while (nextUnlockIndex < enemyEntries.Count && elapsedTime >= enemyEntries[nextUnlockIndex].unlockTime)
        {
            UnlockNextEnemyType();
            nextUnlockIndex++;
        }
    }

    private void UnlockNextEnemyType()
    { 
        if (nextUnlockIndex >= enemyEntries.Count) return;

        // unlock the next enemy type and add to unlocked lists
        EnemyEntry entry = enemyEntries[nextUnlockIndex];
        
        // unlock normal enemy
        if (!unlockedNormals.Contains(entry.normalPrefab))
            unlockedNormals.Add(entry.normalPrefab);

        // unlock elite enemy if it exists
        if (entry.elitePrefab != null && !unlockedElites.Contains(entry.elitePrefab))
            unlockedElites.Add(entry.elitePrefab);

        nextUnlockIndex++; // increment index for next unlock
    }

    private void AdvanceWave()
    {
        currentWave++;

        // current spawn interval is multiplied by spawnRateMultiplier each wave, but never goes below minimumSpawnInterval
        currentSpawnInterval = Mathf.Max(minimumSpawnInterval, currentSpawnInterval * spawnRateMultiplier);

        // 10 + (0 * 7) = 10, 10 + (1 * 7) = 17, 10 + (2 * 7) = 24, etc.
        enemiesToSpawnThisWave = baseHordeSize + (currentWave * hordeSizeIncrease);

        enemiesSpawnedThisWave = 0; // reset count for new wave

        Debug.Log($"[Wave {currentWave}] Spawn Interval: {currentSpawnInterval:F2}s | Horde Goal: {enemiesToSpawnThisWave} | Cap: {maxEnemiesOnScreen} ");
    }

    private void SpawnNormalEnemy()
    {
        if (unlockedNormals.Count == 0) return;

        GameObject prefab = unlockedNormals[Random.Range(0, unlockedNormals.Count)];

        GameObject enemy = PoolManager.SpawnObject(prefab, GetRandomSpawnPosition(), Quaternion.identity, PoolManager.PoolType.Enemy);
        // ApplyScaling(enemy);
    }

    private void SpawnElite()
    {
        if (unlockedElites.Count == 0) return;

        GameObject prefab = unlockedElites[Random.Range(0, unlockedElites.Count)];

        GameObject elite = PoolManager.SpawnObject(prefab, GetRandomSpawnPosition(), Quaternion.identity, PoolManager.PoolType.Enemy);
        //ApplyScaling(elite);
    }

    private void ApplyScaling(GameObject enemyObj)
    {
        // enemy stats (health, damage) scale here
    }

    private void HandleBossSpawn()
    {
        if (bossSpawned || bossPrefab == null) return;

        // change to max level once implemented
        bool atMaxLevel = PlayerController.Instance.playerStats.currentLevel >= 5;

        if (atMaxLevel)
        {
            Vector2 pos = GetRandomSpawnPosition();
            PoolManager.SpawnObject(bossPrefab, pos, Quaternion.identity, PoolManager.PoolType.Enemy);
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
