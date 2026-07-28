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

    public float CurrentSitanMultiplier => sitanCorruptionActive ? sitanMultiplier : 1.0f;
    public bool hasSuperstition => activeSuperstition != null;

    [Header("Timers")]
    private float milestoneTimer = 0f; // deserve mo ba reward ?
    private float nakedtimer = 0f; // no superstition equipped, thus, naked
    [SerializeField] private float milestoneDuration = 180f; // 3 mins
    [SerializeField] private float sitanGracePeriod = 40f; // 30s corruption brrr

    [Header("Sitan's Corruption")]
    public bool sitanCorruptionActive = false;
    public float sitanMultiplier = 1.0f; // base.. then scales up
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
                // REWARD & VANISH
                activeSuperstition.ApplyReward();
                EraseSuperstition();

                if (NotificationManager.Instance != null)
                {
                    NotificationManager.Instance.ShowNotification(pleasedSpiritsNotif);

                    if (SoundManager.Instance != null)
                    {
                        SoundManager.Instance.PlaySFX(7);
                    }
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

                        if (SoundManager.Instance != null)
                        {
                            //SoundManager.Instance.PlaySFX(5);
                            SoundManager.Instance.PlaySFX(6);
                        }
                    }
                }

                sitanMultiplier += 0.01f * Time.deltaTime; // scales up enemy stat modifiers (+1% per second)
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

        UIManager.Instance.UpdateSuperstitionUI(activeSuperstition.superstitionName, activeSuperstition.description,
            activeSuperstition.rewardText, activeSuperstition.penaltyText);

        if (NotificationManager.Instance != null && sitanCorruptionActive)
        {
            NotificationManager.Instance.ShowNotification(sitansEndNotif);
        }

        // reset whenever a spirit is appeased
        nakedtimer = 0f;
        sitanCorruptionActive = false;
        sitanMultiplier = 1.0f;

        // revert enemies back
        if (PoolSpawner.Instance != null)
        {
            PoolSpawner.Instance.RecalculateActiveEnemiesStats();
            Debug.Log("enemy stats back 2 normal");
        }
    }

    public void NotifyRuleBroken(SuperstitionData rule, int amount)
    {
        totalViolations += amount;
        OnSuperstitionBroken?.Invoke(totalViolations);

        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.ShowNotification(angeredSpiritsNotif);
            
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(4);
            }
        }
        //Debug.Log("Total violations: " + totalViolations);

        BreakSuperstition();
    }

    public void BreakSuperstition()
    {
        if (activeSuperstition != null)
        {
            // PENALTY
            activeSuperstition.ApplyPenalty();
            EraseSuperstition();
        }

        nakedtimer = 0f;
    }

    public void EraseSuperstition()
    {
        if (activeSuperstition != null)
        {
            activeSuperstition.Deinitialize();
            activeSuperstition = null;

            UIManager.Instance.UpdateSuperstitionUI("None", "...", null, null);
        }
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
