using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class PoolSpawner : MonoBehaviour
{
    public static PoolSpawner Instance;

    // FOR TUTORIAL
    [SerializeField] private bool isSpawningEnabled = true;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] normalEnemyPrefabs;
    [SerializeField] private GameObject[] eliteEnemyPrefabs;
    [SerializeField] private GameObject[] bossPrefabs;

    [Header("Enemy Wave Progression")]
    public float waveDuration = 60f; // how long a wave lasts
    public float bossSpawnTime = 900f;

    [Header("Spawn Rate")]
    public float baseSpawnInterval = 1.5f; // time before spawning next enemy
    public float minimumSpawnInterval = 0.5f; // cap to the fastest enemies can spawn
    public float spawnIntervalDecrease = 0.05f; // spawn interval decreases by this, makes spawn rate faster

    [Header("Enemy Caps")]
    public int baseMaxEnemies = 30; // max enemies active at wave 0
    public int maxEnemiesIncrease = 15; // increases enemies capped on screen w/ this num.
    public int absoluteEnemyCap = 300; // allowed num. of active enemies

    [Header("Difficulty Scaling")]
    public float statMultiplierPerWave = 1.1f; // stat multiplier applied PER WAVE 1.05 = +5% health/dmg per wave
    public float statMultiplierPerPlayerLevel = 1.05f; // stat multiplier applied PER PLAYER LVL 
    public int maxPlayerLevel = 30;

    [Header("Spawn System")]
    public float spawnEdgeOffset = 1.1f; // just enuf out of the player's sight
    public float timeToUnlockNextEnemy = 90f; // 1:30 mins = new enemy unlocked!
    public float eliteSpawnInterval = 60f; // elites spawn every 1 min
    public float maxDistanceFromPlayer = 15f; // how far an enemy is BEFORE they get respawned 
    public float recycleCheckInterval = 1.5f; // every 1.3s, respawn a far away enemy nearer

    [Header("BANNED ELITE IN CHALLENGE SHRINE")]
    public GameObject bannedElite;

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
        // FOR TUTORIAL
        if (!isSpawningEnabled) return;
        // FOR TUTORIAL

        CalculateWaveSettings();

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

        // SITAN CORRUPTION
        float sitanScale = Mathf.Max(0.001f, SuperstitionManager.Instance.CurrentSitanMultiplier); // prevents division by 0

        float targetInterval = baseSpawnInterval - (currentWave * spawnIntervalDecrease);
        currentSpawnInterval = Mathf.Max(minimumSpawnInterval, targetInterval/sitanScale);

        float targetCap = baseMaxEnemies + (currentWave * maxEnemiesIncrease);
        currentMaxEnemies = Mathf.Min(absoluteEnemyCap, Mathf.RoundToInt(targetCap * sitanScale));
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

    public List<BaseEnemy> SpawnChallengeEliteEnemies(int clampedPlayerLevel)
    {
        int elitesToKill = 3;

        List<BaseEnemy> challengeElites = new List<BaseEnemy>();
        List<GameObject> includedChallengeElites = new List<GameObject>();

        int maxIndexAllowed = Mathf.FloorToInt(elapsedTime / timeToUnlockNextEnemy) + 1;
        int currentRangeMax = Mathf.Min(maxIndexAllowed, shuffledElites.Length);

        for (int j = 0; j < currentRangeMax; j++)
        {
            GameObject includedElite = shuffledElites[j];

            if (includedElite != null && includedElite != bannedElite)
            {
                includedChallengeElites.Add(includedElite);
            }
        }

        if (includedChallengeElites.Count == 0)
        {
            foreach (GameObject elite in shuffledElites)
            {
                if (elite != null && elite != bannedElite)
                {
                    includedChallengeElites.Add(elite);
                }
            }
        }

        if (includedChallengeElites.Count == 0) return challengeElites;

        for (int i = 0; i < elitesToKill; i++)
        {
            GameObject prefab = includedChallengeElites[Random.Range(0, includedChallengeElites.Count)];
            //if (prefab == null) return;

            Vector3 playerPos = PlayerController.Instance.transform.position;

            var radians = 2 * Mathf.PI / elitesToKill * i;

            var horizontal = Mathf.Sin(radians);
            var vertical = Mathf.Cos(radians);

            var spawnDir = new Vector3(horizontal, 0, vertical);
            float radius = 5f;

            var spawnPos = playerPos + spawnDir * radius;

            GameObject challengeElite = PoolManager.SpawnObject(prefab, spawnPos, Quaternion.identity, PoolManager.PoolType.Enemy);
            Color tmpColor = challengeElite.GetComponentInChildren<SpriteRenderer>().color = new Color(255f/255f, 146f/255f, 146f/255f, 255f/255f);

            challengeElites.Add(challengeElite.GetComponent<BaseEnemy>());

            if (challengeElite != null)
            {
                BaseEnemy enemy = challengeElite.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    challengeElites.Add(enemy);
                }

                ApplyScaling(challengeElite, clampedPlayerLevel);
                activeEnemyCount++;
            }
        }

        return challengeElites;
    }

    public void TriggerBatchSpawn(int enemyCount)
    {
        int playerLevel = 1;

        if (PlayerController.Instance != null)
        {
            playerLevel = PlayerController.Instance.playerStats.currentLevel;
        }
        playerLevel = Mathf.Clamp(playerLevel, 1, maxPlayerLevel);

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnNormalEnemy(playerLevel);
        }
    }

    public void SetSpawningEnabled(bool enable)
    {
        isSpawningEnabled = enable;
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
            //SetBossHPBar(boss);

            HealthComponent bossHealth = boss.GetComponent<HealthComponent>();
            if (bossHealth != null) UIManager.Instance.SetupBossHpBar(bossHealth, bossPrefab.name);

            if (boss != null)
            {
                ApplyScaling(boss, clampedPlayerLevel);
                activeEnemyCount++;
            }

            bossSpawned = true;
            StageManager.Instance.OnBossSpawned();
        }
    }

    //public void SetBossHPBar(GameObject boss)
    //{
    //    currBoss = boss;
    //}

    //public GameObject GetBossHPBar()
    //{
    //    return currBoss;
    //}

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
                //Debug.Log("RESPAWNED (" + respawnedEnemy.name + ")");
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

    public void ApplyScaling(GameObject enemyObj, int clampedPlayerLevel)
    {
        BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();
        if (enemy == null) return;

        float waveScale = Mathf.Pow(statMultiplierPerWave, currentWave);
        float playerScale = Mathf.Pow(statMultiplierPerPlayerLevel, clampedPlayerLevel);

        // SITAN CORRUPTION
        float sitanScale = SuperstitionManager.Instance.CurrentSitanMultiplier;

        float finalMultiplier = waveScale * playerScale * sitanScale;

        enemy.ScaleEnemyStat(finalMultiplier);
        Debug.Log($"enemy stat: {finalMultiplier}. sitanScale: {sitanScale}");
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

    public void RecalculateActiveEnemiesStats()
    {
        if (PoolManager._enemyPoolEmpty == null) return;

        int playerLevel = 1;
        if (PlayerController.Instance != null)
        {
            playerLevel = Mathf.Clamp(PlayerController.Instance.GetComponent<PlayerStats>().currentLevel, 1, maxPlayerLevel);
        }

        foreach (Transform child in PoolManager._enemyPoolEmpty.transform)
        {
            if (child.gameObject.activeSelf)
            {
                ApplyScaling(child.gameObject, playerLevel);
            }
        }
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
