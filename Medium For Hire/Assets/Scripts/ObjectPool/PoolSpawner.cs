using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PoolSpawner : MonoBehaviour
{
    public static PoolSpawner Instance;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] normalEnemyPrefabs;
    [SerializeField] private GameObject[] eliteEnemyPrefabs;
    [SerializeField] private GameObject[] bossPrefabs;

    [Header("Enemy Wave Progression")]
    public float waveDuration = 60f; // how long a wave lasts
    public float bossSpawnTime = 900f;

    [Header("Spawn Rate")]
    public float baseSpawnInterval = 1.3f; // time before spawning next enemy
    public float minimumSpawnInterval = 0.15f; // cap to the fastest enemies can spawn
    public float spawnIntervalDecrease = 0.08f; // spawn interval decreases by this, makes spawn rate faster

    [Header("Enemy Caps")]
    public int baseMaxEnemies = 30; // max enemies active at wave 0
    public int maxEnemiesIncrease = 15; // increases enemies capped on screen w/ this num.
    public int absoluteEnemyCap = 300; // allowed num. of active enemies

    [Header("Difficulty Scaling")]
    public float statMultiplierPerWave = 1.05f; // stat multiplier applied PER WAVE 1.05 = +5% health/dmg per wave
    public float statMultiplierPerPlayerLevel = 1.03f; // stat multiplier applied PER PLAYER LVL 
    public int maxPlayerLevel = 30;

    [Header("Spawn System")]
    public float spawnEdgeOffset = 1.1f; // just enuf out of the player's sight
    public float timeToUnlockNextEnemy = 90f; // 1:30 mins = new enemy unlocked!
    public float eliteSpawnInterval = 60f; // elites spawn every 1 min
    public float maxDistanceFromPlayer = 15f; // how far an enemy is BEFORE they get respawned 
    public float recycleCheckInterval = 1.5f; // every 1.3s, respawn a far away enemy nearer


    // runtime 
    [Header("--- THIS RUN'S ORDER OF ENEMIES")]
    private GameObject[] shuffledNormals;
    private GameObject[] shuffledElites;

    [Header("--- TIMERS")]
    [SerializeField] private float currentSpawnInterval;
    [SerializeField] private float spawnTimer = 0f;
    [SerializeField] private float eliteTimer = 0f;
    [SerializeField] private float waveTimer = 0f;
    [SerializeField] private float elapsedTime = 0f;
    [SerializeField] private float recycleTimer = 0f;

    [Header("--- ENEMY WAVE")]
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int activeEnemyCount = 0;
    [SerializeField] int currentMaxEnemies;
    [SerializeField] private bool bossSpawned = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ResetSpawnerState();
    }

    private void ResetSpawnerState()
    {
        // clean slate 
        elapsedTime = 0f;
        currentWave = 0;
        waveTimer = 0f;
        spawnTimer = 0f;
        eliteTimer = 0f;
        recycleTimer = 0f;
        activeEnemyCount = 0;
        bossSpawned = false;

        InitializeShuffledEnemies();
        CalculateWaveSettings();
    }

    private void InitializeShuffledEnemies()
    {
        if (normalEnemyPrefabs == null || normalEnemyPrefabs.Length == 0) return;

        int totalEnemies = normalEnemyPrefabs.Length;
        shuffledNormals = new GameObject[totalEnemies];
        shuffledElites = new GameObject[totalEnemies];

        // pick the first random enemy
        int firstChosenIndex = Random.Range(0, totalEnemies);
        shuffledNormals[0] = normalEnemyPrefabs[firstChosenIndex];

        // match its elite version
        if (eliteEnemyPrefabs != null && firstChosenIndex < eliteEnemyPrefabs.Length)
        {
            shuffledElites[0] = eliteEnemyPrefabs[firstChosenIndex];
        }

        // remaining enemies are shuffled
        List<int> remainingIndexes = new List<int>();
        for (int i = 0; i < totalEnemies; i++)
        {
            if (i != firstChosenIndex)
            {
                remainingIndexes.Add(i);
            }
        }

        // populate the rest of array slots w/ a random sequence
        int currentSlot = 1;
        while (remainingIndexes.Count > 0)
        {
            int randomListIndex = Random.Range(0, remainingIndexes.Count);
            int actualEnemyIndex = remainingIndexes[randomListIndex];

            shuffledNormals[currentSlot] = normalEnemyPrefabs[actualEnemyIndex];
            if (eliteEnemyPrefabs != null && actualEnemyIndex < eliteEnemyPrefabs.Length)
            {
                shuffledElites[currentSlot] = eliteEnemyPrefabs[actualEnemyIndex];
            }
            remainingIndexes.RemoveAt(randomListIndex);
            currentSlot++;
        }
    }

    private void Update()
    {
        if (PlayerController.Instance == null || !PlayerController.Instance.gameObject.activeSelf)
            return;

        elapsedTime += Time.deltaTime;
        waveTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;
        eliteTimer += Time.deltaTime;
        recycleTimer += Time.deltaTime;

        int playerLevel = 1;
        if (PlayerController.Instance != null)
        {
            playerLevel = PlayerController.Instance.GetComponent<PlayerStats>().currentLevel;
        }
        playerLevel = Mathf.Clamp(playerLevel, 1, maxPlayerLevel);

        float speedLimit = 0.5f;
        float speedUp = 0.02f;

        float playerLevelModifier = Mathf.Max(speedLimit, 1f - (playerLevel * speedUp));
        float adjustedSpawnInterval = Mathf.Max(minimumSpawnInterval, currentSpawnInterval * playerLevelModifier);

        // SPAWNING
        if (spawnTimer >= adjustedSpawnInterval && !IsAtEnemyCap())
        {
            spawnTimer = 0f;
            SpawnNormalEnemy(playerLevel);
        }

        if (eliteTimer >= eliteSpawnInterval && !IsAtEnemyCap())
        {
            eliteTimer = 0f;
            SpawnEliteEnemy(playerLevel);
        }

        if (recycleTimer >= recycleCheckInterval)
        {
            recycleTimer = 0f;
            RecycleFarEnemies(playerLevel);
        }
        // SPAWNING

        // WAVE PROGRESSION
        if (waveTimer >= waveDuration)
        {
            waveTimer = 0f;
            AdvanceWave();
        }

        // BOSS 
        if (!bossSpawned && elapsedTime >= bossSpawnTime)
        {
            SpawnBoss(playerLevel);
        }
    }

    private void AdvanceWave()
    {
        currentWave++;
        CalculateWaveSettings();
    }

    private void CalculateWaveSettings()
    {
        currentSpawnInterval = Mathf.Max(minimumSpawnInterval, baseSpawnInterval - (currentWave * spawnIntervalDecrease));
        currentMaxEnemies = Mathf.Max(absoluteEnemyCap, baseMaxEnemies + (currentWave * maxEnemiesIncrease));
    }

    private bool IsAtEnemyCap()
    {
        return activeEnemyCount >= currentMaxEnemies;
    }

    private void SpawnNormalEnemy(int clampedPlayerLevel)
    {
        GameObject prefab = GetUnlockedPrefabFromList(shuffledNormals);
        if (prefab == null) return;

        GameObject enemy = PoolManager.SpawnObject(prefab, GetRandomSpawnPosition(), Quaternion.identity, PoolManager.PoolType.Enemy);
        if (enemy != null)
        {
            ApplyScaling(enemy, clampedPlayerLevel);
            activeEnemyCount++;
        }

    }

    private void SpawnEliteEnemy(int clampedPlayerLevel)
    {
        GameObject prefab = GetUnlockedPrefabFromList(shuffledElites);
        if (prefab == null) return;

        GameObject elite = PoolManager.SpawnObject(prefab, GetRandomSpawnPosition(), Quaternion.identity, PoolManager.PoolType.Enemy);
        if (elite != null)
        {
            ApplyScaling(elite, clampedPlayerLevel);
            activeEnemyCount++;
        }
    }

    private void SpawnBoss(int clampedPlayerLevel)
    {
        if (bossPrefabs == null || bossPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, bossPrefabs.Length);
        GameObject bossPrefab = bossPrefabs[randomIndex];

        if (bossPrefab != null)
        {
            Vector2 spawnPos = GetRandomSpawnPosition();
            GameObject boss = PoolManager.SpawnObject(bossPrefab, spawnPos, Quaternion.identity, PoolManager.PoolType.Enemy);

            if (boss != null)
            {
                ApplyScaling(boss, clampedPlayerLevel);
                activeEnemyCount++;
            }

            bossSpawned = true;
        }
    }

    private void RecycleFarEnemies(int clampedPlayerLevel)
    {
        if (PoolManager._enemyPoolEmpty == null || PlayerController.Instance == null) return;

        Vector3 playerPos = PlayerController.Instance.transform.position;
        float maxDistSqr = maxDistanceFromPlayer * maxDistanceFromPlayer;

        // temp list
        List<GameObject> enemiesToRecycle = new List<GameObject>();

        foreach (Transform child in PoolManager._enemyPoolEmpty.transform)
        {
            if (child.gameObject.activeSelf)
            {
                // if boss, don't recycle
                if (IsBoss(child.gameObject)) continue;

                float sqrDistance = (child.position - playerPos).sqrMagnitude;
                if (sqrDistance > maxDistSqr)
                {
                    enemiesToRecycle.Add(child.gameObject);
                }
            }
        }

        // RECYCLE
        for (int i = 0; i < enemiesToRecycle.Count; i++)
        {
            GameObject enemy = enemiesToRecycle[i];

            PoolMember member = enemy.GetComponent<PoolMember>();
            if (member != null && member.prefab != null)
            {
                // return far-away enemy to pool
                PoolManager.ReturnObjectToPool(enemy);
                NotifyEnemyDespawned();

                GameObject respawnedEnemy = PoolManager.SpawnObject(member.prefab, GetRandomSpawnPosition(), Quaternion.identity, PoolManager.PoolType.Enemy);
                Debug.Log("RESPAWNED (" + respawnedEnemy.name + ")");
                if (respawnedEnemy != null)
                {
                    ApplyScaling(respawnedEnemy, clampedPlayerLevel);
                    activeEnemyCount++;
                }
            }
        }
    }

    private bool IsBoss(GameObject obj)
    {
        if (bossPrefabs == null) return false;

        PoolMember member = obj.GetComponent<PoolMember>();
        if (member == null) return false;

        for (int i = 0; i < bossPrefabs.Length; i++)
        {
            if (bossPrefabs[i] == member.prefab) return true;
        }
        return false;
    }

    private GameObject GetUnlockedPrefabFromList(GameObject[] prefabArray)
    {
        if (prefabArray == null || prefabArray.Length == 0) return null;

        int maxIndexAllowed = Mathf.FloorToInt(elapsedTime / timeToUnlockNextEnemy) + 1;
        int currentRangeMax = Mathf.Min(maxIndexAllowed, prefabArray.Length);

        int randomIndex = Random.Range(0, currentRangeMax);
        return prefabArray[randomIndex];
    }

    private void ApplyScaling(GameObject enemyObj, int clampedPlayerLevel)
    {
        BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();
        if (enemy == null) return;

        float waveScale = Mathf.Pow(statMultiplierPerWave, currentWave);
        float playerScale = Mathf.Pow(statMultiplierPerPlayerLevel, clampedPlayerLevel);
        float finalMultiplier = waveScale * playerScale;

        enemy.ScaleEnemyStat(finalMultiplier);
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 edgeCoordinate = Random.value > 0.5f
                    ? new Vector2(Random.value > 0.5f ? 1f + (spawnEdgeOffset - 1f) : -(spawnEdgeOffset - 1f), Random.value)
                    : new Vector2(Random.value, Random.value > 0.5f ? 1f + (spawnEdgeOffset - 1f) : -(spawnEdgeOffset - 1f));

        return Camera.main.ViewportToWorldPoint(edgeCoordinate);
    }

    public void NotifyEnemyDespawned()
    {
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
    }

    private void OnDrawGizmosSelected()
    {
        if (PlayerController.Instance != null)
        {
            Gizmos.color = Color.red;
            // anyone outside the circle respawns
            Gizmos.DrawWireSphere(PlayerController.Instance.transform.position, maxDistanceFromPlayer);
        }
    }
}
