using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] public PlayerEvents Events;
    [SerializeField] private PoolSpawner poolSpawner;

    [SerializeField] private Timer timer;

    public static StageManager Instance;
    public bool isGameOver = false;

    public static LevelData CurrentLevel { get; set; }

    private void Awake() // for SINGLETON
    {
        // singleton 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (CurrentLevel == null)
        {
            Debug.Log("No level...");
            return;
        }

        AssembleStage();
    }

    private void OnEnable()
    {
        isGameOver = false;

        Events.OnPlayerDeath += CompleteLevel;
    }

    private void OnDisable()
    {
        Events.OnPlayerDeath -= CompleteLevel;
    }

    private void AssembleStage()
    {
        // SET UP MAP
        if (CurrentLevel.map.mapPrefab != null)
        {
            Instantiate(CurrentLevel.map.mapPrefab, Vector3.zero, Quaternion.identity);
        }

        // SET UP POOL SPAWNER
        if (poolSpawner != null)
        {
            poolSpawner.SetUpLevelData(CurrentLevel);
        }
        else
        {
            Debug.Log("Missing pool spawner reference.");
        }

        // SET UP SUPERSTITION
        if (CurrentLevel.superstition != null)
        {
            Debug.Log("CURRENT SUPERSTITION: " + CurrentLevel.superstition.name);
        }
    }

    public void CompleteLevel()
    {
        isGameOver = true;

        // display end run screen
        UIManager.Instance.DisplayEndRunScreen();
    }
}
