using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SuperstitionManager : MonoBehaviour
{
    // ===== 1ST SUPERSTITION: NO LEFT OVER =====

    [Header("Active Superstition")]
    public int orbsExpiredCount = 0;

    // EVENT
    public static event Action<int> OnSuperstionBroken;

    public static SuperstitionManager Instance;
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


    // OnEnable --> when the scene loads, this manager becomes active
    private void OnEnable()
    {
        // subscribe
        ExpOrb.OnExpOrbExpire += TrackExpiredExpOrbs;
    }

    private void TrackExpiredExpOrbs()
    {
        orbsExpiredCount++;
        OnSuperstionBroken?.Invoke(orbsExpiredCount); // hi, this is the current count of orbs expired

        Debug.Log("Expired Orbs: " + orbsExpiredCount);
    }

    // OnDisable --> when manager's inactive
    private void OnDisable()
    {
        // unsubscribe
        ExpOrb.OnExpOrbExpire -= TrackExpiredExpOrbs;
    }
}
