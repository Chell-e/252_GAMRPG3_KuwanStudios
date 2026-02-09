using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPoolSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public float spawnTimer;
        public float spawnInterval;
        public int enemiesPerWave;
        public int spawnedEnemyCount;
    }

    public int waveIndex;
    public List<Wave> waves;


    void Update()
    {
        if (PlayerController.Instance.gameObject.activeSelf)
        {
            waves[waveIndex].spawnTimer += Time.deltaTime;

            if (waves[waveIndex].spawnTimer >= waves[waveIndex].spawnInterval)
            {
                // more accurate than resetting to 0
                waves[waveIndex].spawnTimer = waves[waveIndex].spawnTimer - waves[waveIndex].spawnInterval;
                SpawnEnemy();
            }
            // once all enemies in wave have been spawned, move to next wave
            if (waves[waveIndex].spawnedEnemyCount >= waves[waveIndex].enemiesPerWave)
            {
                // reset spawned enemy count for current wave
                waves[waveIndex].spawnedEnemyCount = 0;

                // doesnt go below 0.3 seconds spawn interval
                if (waves[waveIndex].spawnInterval > 0.3f)
                {
                    waves[waveIndex].spawnInterval *= 0.9f; // reduce spawn interval by 10%, increase difficulty
                }

                waveIndex++;
            }

            //// cycles back to first wave
            //if (waveIndex >= waves.Count)
            //{
            //    waveIndex = 0;
            //}
        }

    }

    private void SpawnEnemy()
    {
        PoolManager.SpawnObject(waves[waveIndex].enemyPrefab, RandomSpawnPosition(), transform.rotation, PoolManager.PoolType.Enemy);
        waves[waveIndex].spawnedEnemyCount++;
    }

    private Vector2 RandomSpawnPosition()
    {
        float edgeOffset = 1.1f;

        Vector2 spawnViewportPos;
        Vector2 spawnWorldPos;

        // coin flip (horizontal/vertical)
        if (Random.Range(0f, 1f) > 0.5f)
        {
            // left/right
            if (Random.Range(0f, 1f) > 0.5f)
            {
                spawnViewportPos = new Vector3(1 - edgeOffset, Random.value);
            }
            else
            {
                spawnViewportPos = new Vector3(edgeOffset, Random.value);
            }
        }
        else
        {
            // top/bottom
            if (Random.Range(0f, 1f) > 0.5f)
            {
                spawnViewportPos = new Vector3(Random.value, 1 - edgeOffset);
            }
            else
            {
                spawnViewportPos = new Vector3(Random.value, edgeOffset);
            }
        }

        spawnWorldPos = Camera.main.ViewportToWorldPoint(spawnViewportPos);
        return spawnWorldPos;
    }
}
