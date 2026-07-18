using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SuperstitionManager : MonoBehaviour
{
    public static SuperstitionManager Instance;

    [Header("Active Superstition")]
    public SuperstitionData activeSuperstition;
    public int totalViolations = 0;

    public bool hasSuperstition => activeSuperstition != null;

    [Header("Timers")]
    private float milestoneTimer = 0f; // deserve mo ba reward ?
    private float nakedtimer = 0f; // no superstition equipped, thus, naked
    [SerializeField] private float milestoneDuration = 120f; // 2 mins
    [SerializeField] private float sitanGracePeriod = 30f; // 30s corruption brrr

    [Header("Sitan's Corruption")]
    private bool sitanCorruptionActive = false;
    public float sitanStatMultiplier = 1.0f;
    //public float sitanSpawnMultiplier = 1f; 

    [Header("Notifications SO")]
    [SerializeField] private NotificationSO pleasedSpiritsNotif;
    [SerializeField] private NotificationSO angeredSpiritsNotif;
    [SerializeField] private NotificationSO sitansCorruptionNotif;
    [SerializeField] private NotificationSO sitansEndNotif;

    // EVENT
    public static event Action<int> OnSuperstitionBroken;


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
    }

    private void Update()
    {
        HandleMilestoneTimer();
        HandleSitanLogic();
    }

    private void HandleMilestoneTimer()
    {
        milestoneTimer += Time.deltaTime;

        if (milestoneTimer >= milestoneDuration)
        {
            milestoneTimer = 0f;

            // DO THEY HAVE A SUPERSTITION? AND DID THEY FOLLOW IT?
            if (hasSuperstition && totalViolations == 0)
            {
                //ApplyReward();
                //Debug.Log("APLPYING REWARD");

                if (NotificationManager.Instance != null)
                {
                    NotificationManager.Instance.ShowNotification(pleasedSpiritsNotif);
                }
            }
        }

        // reset
        totalViolations = 0;
    }

    private void HandleSitanLogic()
    {
        if (!hasSuperstition)
        {
            nakedtimer += Time.deltaTime;

            if (nakedtimer >= sitanGracePeriod)
            {
                if (!sitanCorruptionActive)
                {
                    sitanCorruptionActive = true;
                    //Debug.Log("Sitan's Corruption starts!");

                    if (NotificationManager.Instance != null)
                    {
                        NotificationManager.Instance.ShowNotification(sitansCorruptionNotif);
                    }
                }

                sitanStatMultiplier += 0.01f * Time.deltaTime; // scales up enemy stat modifiers (+1% per second)
                //sitanSpawnMultiplier += 0.01f * Time.deltaTime; 
            }
        }
    }

    public void ActivateSuperstition(SuperstitionData _superstitionData)
    {
        if (_superstitionData == null)
        {
            return;
        }

        activeSuperstition = _superstitionData;
        activeSuperstition.Initialize(StageManager.Instance);

        UIManager.Instance.UpdateSuperstitionUI(activeSuperstition.superstitionName, activeSuperstition.description, activeSuperstition.rewardText, activeSuperstition.penaltyText);

        // reset whenever a spirit is appeased
        nakedtimer = 0f;
        sitanCorruptionActive = false;
        sitanStatMultiplier = 1.0f;

        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.ShowNotification(sitansEndNotif);
        }
    }

    public void NotifyRuleBroken(SuperstitionData rule, int amount)
    {
        totalViolations += amount;
        OnSuperstitionBroken?.Invoke(totalViolations);

        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.ShowNotification(angeredSpiritsNotif);
        }
        //Debug.Log("Total violations: " + totalViolations);

        BreakSuperstition();
    }

    public void BreakSuperstition()
    {
        if (activeSuperstition != null)
        {
            // ApplyPenalty();

            activeSuperstition.Deinitialize();
            activeSuperstition = null;
        }

        nakedtimer = 0f;
    }

    public void ResetTotalViolations()
    {
        totalViolations = 0;
    }

    private void OnDestroy()
    {
        if (activeSuperstition != null)
        {
            activeSuperstition.Deinitialize();
        }
    }
}
