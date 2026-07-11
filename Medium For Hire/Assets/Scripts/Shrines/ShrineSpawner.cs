using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineSpawner : MonoBehaviour
{
    public static ShrineSpawner Instance;

    [Header("Shrine Spawn Times")]
    [SerializeField] private float spiritSpawnDelay = 5f; // 10 sec
    [SerializeField] private float akasiSpawnDelay = 10f; // 1 min
    [SerializeField] private float apolakiSpawnDelay = 15f; // 5 min
    private float attemptedDelay = 5f;

    [Header("Respawn Times")]
    [SerializeField] private float spiritCooldown = 30f;
    [SerializeField] private float apolakiCooldown = 30f; // 5 mins

    [Header("Map References")]
    [SerializeField] private List<BaseShrine> allShrineSpots;

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
        StartCoroutine(InhabitEmptyShrine(ShrineType.Spirit, spiritSpawnDelay));
        StartCoroutine(InhabitEmptyShrine(ShrineType.Akasi, akasiSpawnDelay));
        StartCoroutine(InhabitEmptyShrine(ShrineType.Apolaki, apolakiSpawnDelay));
    }

    private IEnumerator InhabitEmptyShrine(ShrineType type, float delay)
    {
        yield return new WaitForSeconds(delay);

        // create list of empty spots
        List<BaseShrine> emptySpots = new List<BaseShrine>();

        // get all empty spots in the map
        foreach (BaseShrine shrine in allShrineSpots)
        {
            if (shrine != null && shrine.CurrentType == ShrineType.Empty)
            {
                emptySpots.Add(shrine);
            }
        }

        if (emptySpots.Count > 0)
        {
            // pick a random spot for a pirit/deity to inhabit
            int randomIndex = Random.Range(0, emptySpots.Count);
            emptySpots[randomIndex].SetShrineType(type);
            Debug.Log("{" + type + "} has inhabited an empty shrine!");
        }
        else
        {
            Debug.Log("No empty spots detected. Attempting to respawn again...");
            StartCoroutine(InhabitEmptyShrine(type, attemptedDelay));
        }
    }

    public void FreeActiveShrineSpot(BaseShrine spot, ShrineType oldType)
    {
        if (oldType == ShrineType.Spirit)
        {
            StartCoroutine(InhabitEmptyShrine(ShrineType.Spirit, spiritCooldown));
        }
        else if (oldType == ShrineType.Apolaki)
        {
            StartCoroutine(InhabitEmptyShrine(ShrineType.Apolaki, apolakiCooldown));
        }
    }
}