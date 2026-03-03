using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteSpawner : MonoBehaviour
{
    public GameObject elitePrefab;
    public float spawnInterval = 10f;
    public float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnElite();
            spawnTimer = 0f;
        }
    }

    private void SpawnElite()
    {
        Vector2 spawnPos = RandomSpawnOutsideCamera();
        Instantiate(elitePrefab, spawnPos, Quaternion.identity);
    }

    private Vector2 RandomSpawnOutsideCamera()
    {
        float edgeOffset = 1.1f;
        Vector2 spawnViewportPos;
        Vector2 spawnWorldPos;
        // coin flip (horizontal/vertical)
        if (Random.Range(0f, 1f) > 0.5f)
        {
            // horizontal
            spawnViewportPos = new Vector2(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f) > 0.5f ? edgeOffset : -edgeOffset
            );
        }
        else
        {
            // vertical
            spawnViewportPos = new Vector2(
                Random.Range(0f, 1f) > 0.5f ? edgeOffset : -edgeOffset,
                Random.Range(0f, 1f)
            );
        }
        spawnWorldPos = Camera.main.ViewportToWorldPoint(spawnViewportPos);
        return spawnWorldPos;
    }
}
