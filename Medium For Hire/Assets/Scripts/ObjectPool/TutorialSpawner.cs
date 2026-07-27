using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialSpawner : MonoBehaviour
{
    [SerializeField] private int enemiesToSpawn = 5;

    private void Start()
    {
        // standby spawner
        PoolSpawner.Instance.SetSpawningEnabled(false);
    }

    public void TriggerEnemySpawning()
    {
        if (PoolSpawner.Instance == null) return;

        PoolSpawner.Instance.TriggerBatchSpawn(enemiesToSpawn);
    }
}
