using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stat
{
    Movespeed,
    PickupRange
}

public class PlayerStats : MonoBehaviour
{
    [Header("Player Level")]
    //public int maxLevel;
    public int currentLevel;
    public int expToLevel;
    public int currentExp;
    //public List<int> expToLevelUp;

    [Header("Health")]
    public int maxHealth;
    public int currentHealth;

    [Space(5)]
    [Header("Base Stats")]
    [Space(5)]
    [Header("Mobility")]
    public float movespeedBase;
    public float pickupRangeBase;

    [Space(5)]
    [Header("Upgrade Stats")]
    [Space(5)]

    [Header("Attack")]
    public int dmgPercent;
    public int cooldownPercent;

    [Header("Mobility")]
    public int movespeedPercent;

    [Header("Other")]
    public int pickupRangePercent;

    public float GetFinalMovespeed()
    {
        return movespeedBase * (movespeedPercent / 100f);
    }

    public float GetFinalStat(Stat statToFinalize)
    {
        switch (statToFinalize)
        {
            case Stat.Movespeed:
                return movespeedBase * (movespeedPercent / 100f);
            case Stat.PickupRange:
                return pickupRangeBase * (pickupRangePercent * 100f);
            default:
                Debug.Log($"PlayerStats: unhandled stat type {statToFinalize}");
                return -1f;
        }
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;
        UIManager.Instance.UpdateExpSlider();

        if (currentExp >= expToLevel)
        {
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.ShowUpgradeOptions();
            }

            currentExp = 0;
            currentLevel++;

            UIManager.Instance.UpdateExpSlider();
        }

        /*int index = playerStats.currentLevel - 1;
        if (playerStats.currentEXP >= playerStats.expToLevelUp[index])
        {
            playerStats.expToLevelUp[index]++;
        }*/
    }
}
