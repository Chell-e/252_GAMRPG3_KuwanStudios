using System.Collections.Generic;
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

    [System.Serializable]
    public class Wave
    {
        public float spawnTimer;
        public float spawnInterval;
        public int enemiesPerWave;
        public int spawnedEnemyCount;
    }

    [Header("Elite Settings")]
    private float eliteTimer = 0f;
    public float eliteSpawnInterval = 60f; // spawn elite every 60s

    [Header("Enemy Unlocks")]
    public List<EnemyEntry> enemyEntries;   
    private List<GameObject> activeNormals = new List<GameObject>();
    private List <GameObject> activeElites = new List<GameObject>();

    [Header("Wave Settings")]
    public int waveIndex;
    public List<Wave> waves;

    private float elapsedTime = 0f;

    void Update()
    {
        if (!PlayerController.Instance.gameObject.activeSelf) return;

        elapsedTime += Time.deltaTime;
        eliteTimer += Time.deltaTime;

        // unlock enemies based on time
        foreach (var entry in enemyEntries)
        {
            if (elapsedTime >= entry.unlockTime && !activeNormals.Contains(entry.normalPrefab))
            {
                activeNormals.Add(entry.normalPrefab);
                activeElites.Add(entry.elitePrefab); // when a normal enemy is unlocked, add its elite version too
            }
        }

        if (eliteTimer >= eliteSpawnInterval)
        {
            eliteTimer = 0f;
            SpawnElite();
        }

        HandleWaveSpawning();
    }

    private void HandleWaveSpawning()
    {
        Wave wave = waves[waveIndex];
        wave.spawnTimer += Time.deltaTime;

        if (wave.spawnTimer >= wave.spawnInterval)
        {
            wave.spawnTimer -= wave.spawnInterval;
            SpawnEnemy();
        }

        if (wave.spawnedEnemyCount >= wave.enemiesPerWave)
        {
            wave.spawnedEnemyCount = 0;

            if (wave.spawnInterval > 0.3f)
                wave.spawnInterval *= 0.9f; 

            waveIndex = (waveIndex + 1) % waves.Count; 
        }
    }

    private void SpawnElite()
    {
        if (activeElites.Count == 0) return;

        // pick a random unlocked elite
        GameObject prefab = activeElites[Random.Range(0, activeElites.Count)];
        GameObject elite = PoolManager.SpawnObject(prefab, RandomSpawnPosition(), Quaternion.identity, PoolManager.PoolType.Enemy);
    }

    private void SpawnEnemy()
    {
        if (activeNormals.Count == 0) return;

        // pick a random unlocked enemy
        GameObject prefab = activeNormals[Random.Range(0, activeNormals.Count)];
        GameObject enemy = PoolManager.SpawnObject(prefab, RandomSpawnPosition(), Quaternion.identity, PoolManager.PoolType.Enemy);

        waves[waveIndex].spawnedEnemyCount++;
    }

    private Vector2 RandomSpawnPosition()
    {
        float edgeOffset = 1.1f;
        Vector2 pos = Random.value > 0.5f
            ? new Vector2(Random.value > 0.5f ? 1 - edgeOffset : edgeOffset, Random.value)
            : new Vector2(Random.value, Random.value > 0.5f ? 1 - edgeOffset : edgeOffset);

        return Camera.main.ViewportToWorldPoint(pos);
    }
}
