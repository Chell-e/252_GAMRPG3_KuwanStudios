using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineSpawner : MonoBehaviour
{
    public static ShrineSpawner Instance;

    [Header("Shrines")]
    [SerializeField] private BaseShrine spiritShrineInstance;
    [SerializeField] private BaseShrine akasiShrineInstance;
    [SerializeField] private BaseShrine apolakiShrineInstance;

    [Header("Shrine Spawn Times")]
    [SerializeField] private float akasiSpawnDelay = 10f; // 1 min
    [SerializeField] private float apolakiSpawnDelay = 20f; // 5 min

    [Header("Map Spawn Points")]
    [SerializeField] private LayerMask obstructionLayers;
    [SerializeField] private float safetyRadius = 2.5f;
    [SerializeField] private List<Transform> availableSpawnPoints;
    private List<Transform> occupiedSpawnPoints = new List<Transform>();

    private void Awake()
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
    }

    private void Start()
    {
        // ---mamaya pa toh
        if (apolakiShrineInstance != null)
        {
            apolakiShrineInstance.gameObject.SetActive(false);
        }

        // ---isa ka pa
        if (akasiShrineInstance != null)
        {
            akasiShrineInstance.gameObject.SetActive(false);
        }

        if (spiritShrineInstance != null)
        {
            SpawnShrine(spiritShrineInstance);
        }

        // ---start timers for the other shrines
        StartCoroutine(SpawnShrineAfterDelay(akasiShrineInstance, akasiSpawnDelay));
        StartCoroutine(SpawnShrineAfterDelay(apolakiShrineInstance, apolakiSpawnDelay));
    }

    public bool SpawnShrine(BaseShrine shrine)
    {
        Transform spawnPoint = GetSpawnPoint();

        if (spawnPoint == null)
        {
            Debug.LogWarning("No available spawn points for shrine: " + shrine.name);
            return false;
        }

        availableSpawnPoints.Remove(spawnPoint);
        occupiedSpawnPoints.Add(spawnPoint);

        shrine.transform.position = spawnPoint.position;
        shrine.currentSpawnPoint = spawnPoint;
        shrine.gameObject.SetActive(true);

        return true;
    }

    private Transform GetSpawnPoint()
    {
        if (availableSpawnPoints.Count == 0)
        {
            return null;
        }

        // temp list 
        List<Transform> shuffledSpawnPoints = new List<Transform>(availableSpawnPoints);

        while (shuffledSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, shuffledSpawnPoints.Count);
            Transform safeSpawnPoint = shuffledSpawnPoints[randomIndex];

            Collider2D obstruction = Physics2D.OverlapCircle(safeSpawnPoint.position, safetyRadius, obstructionLayers);
            if (obstruction == null)
            {
                return safeSpawnPoint;
            }
            else
            {
                Debug.Log("Can't spawn. Blocked by: " + obstruction.name);
                shuffledSpawnPoints.RemoveAt(randomIndex);
            }
        }

        return null;
    }

    public void FreeSpawnPoint(Transform point)
    {
        if (occupiedSpawnPoints.Contains(point))
        {
            occupiedSpawnPoints.Remove(point);
            availableSpawnPoints.Add(point);
        }
    }

    public void RequestDelayedRespawn(BaseShrine shrine, float delay)
    {
        StartCoroutine(SpawnShrineAfterDelay(shrine, delay));
    }

    private IEnumerator SpawnShrineAfterDelay(BaseShrine shrine, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (shrine != null)
        {
            bool success = SpawnShrine(shrine);

            if (success)
            {
                Debug.Log("Spawned shrine: " + shrine.name + " after delay of " + delay + " seconds.");
            }
            else
            {
                Debug.Log("Failed to spawn shrine: " + shrine.name + " after delay of " + delay + " seconds.");
                RequestDelayedRespawn(shrine, 10f); // Retry after 10s if spawn failed
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (availableSpawnPoints == null) return;
        Gizmos.color = Color.green;
        foreach (Transform point in availableSpawnPoints)
        {
            if (point != null) Gizmos.DrawWireSphere(point.position, safetyRadius);
        }
    }
}