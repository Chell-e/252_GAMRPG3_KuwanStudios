using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineSpawner : MonoBehaviour
{
    public static ShrineSpawner Instance;

    public float allShrineHealAmount = 5f;
    public int challengeCap = 3;
    public bool isChallengeActive = false;

    [Header("Shrine Spawn Times")]
    [SerializeField] private float spiritSpawnDelay = 10; // 10 sec
    [SerializeField] private float akasiSpawnDelay = 60f; // 1 min
    [SerializeField] private float apolakiSpawnDelay = 5f; // 5 min
    private float attemptedDelay = 5f;

    [Header("Respawn Times")]
    [SerializeField] public float spiritCooldown = 30f;
    [SerializeField] public float akasiCooldown = 360f;
    [SerializeField] public float apolakiCooldown = 5f; // 5 mins

    [Header("Map References")]
    [SerializeField] private List<BaseShrine> allShrineSpots;
    public List<BaseShrine> activeShrines = new List<BaseShrine>();

    [Header("Notification SO Shrines")]
    [SerializeField] private NotificationSO spiritNotif;
    [SerializeField] private NotificationSO akasiNotif;
    [SerializeField] private NotificationSO apolakiNotif;

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

        if (challengeCap > 0) StartCoroutine(InhabitEmptyShrine(ShrineType.Apolaki, apolakiSpawnDelay));
    }

    private IEnumerator InhabitEmptyShrine(ShrineType type, float delay)
    {
        yield return new WaitForSeconds(delay);

        // if challenge is active OR exhausted the shrine, dont spawn anymore
        if (type == ShrineType.Apolaki)
        {
            if (isChallengeActive || challengeCap <= 0)
                yield break;
        }

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
            //Debug.Log("{" + type + "} has inhabited an empty shrine!");

            SetUpNotification(type);
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
        else if (oldType == ShrineType.Akasi)
        {
            StartCoroutine(InhabitEmptyShrine(ShrineType.Akasi, akasiCooldown));
        }
        else if (oldType == ShrineType.Apolaki)
        {
            //StartCoroutine(InhabitEmptyShrine(ShrineType.Apolaki, apolakiCooldown));
        }
    }

    public void SetUpNotification(ShrineType type)
    {
        if (type == ShrineType.Spirit)
        {
            NotificationManager.Instance.ShowNotification(spiritNotif);
        }
        if (type == ShrineType.Akasi)
        {
            NotificationManager.Instance.ShowNotification(akasiNotif);
        }
        if (type == ShrineType.Apolaki)
        {
            NotificationManager.Instance.ShowNotification(apolakiNotif);
        }
    }

    public void CompleteApolakiChallenge()
    {
        isChallengeActive = false;

        if (challengeCap > 0) 
            StartCoroutine(InhabitEmptyShrine(ShrineType.Apolaki, apolakiCooldown));
    }
}