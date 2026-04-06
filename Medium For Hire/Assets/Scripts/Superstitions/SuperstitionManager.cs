using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SuperstitionManager : MonoBehaviour
{
    [Header("Active Superstition")]
    private SuperstitionData activeSuperstition;
    public int totalViolations = 0;

        // EVENT
    public static event Action<int> OnSuperstitionBroken;


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

    public void ActivateSuperstition(SuperstitionData _superstitionData)
    {
        if (_superstitionData == null)
        {
            UIManager.Instance.SetSuperstitionText("None", "No Active Superstition.", "No flavor text.");
            return;
        }

        activeSuperstition = _superstitionData;
        activeSuperstition.Initialize(StageManager.Instance);

        UIManager.Instance.SetSuperstitionText(activeSuperstition.superstitionName, activeSuperstition.description, activeSuperstition.flavorText);
    }    

    public void NotifyRuleBroken(SuperstitionData rule, int amount)
    {
        totalViolations += amount;
        OnSuperstitionBroken?.Invoke(totalViolations);

        Debug.Log("Total violations: " + totalViolations);
    }

    private void OnDestroy()
    {
        if (activeSuperstition != null)
        {
            activeSuperstition.Deinitialize();
        }
    }
}
