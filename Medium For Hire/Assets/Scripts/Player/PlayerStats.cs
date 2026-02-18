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
    [Tooltip("Base, flat movement speed. Normally unupgradeable.")]
    public float movespeedBase;
    public float pickupRangeBase;

    [Space(5)]
    [Header("Upgrade Stats")]
    [Space(5)]

    [Header("Health Upgrades")]
    [Tooltip("Percentage multiplying max health.")]
    public int maxHealthPercent = 100;

    [Header("Attack Upgrades")]
    [Tooltip("Percentage multiplying weapon damage.")]
    public int dmgPercent = 100;
    [Tooltip("Percentage dividing weapon cooldown.")]
    public int atkSpeedPercent = 100;
    [Tooltip("Percentage multiplying weapon velocity/speed.")]
    public int projectileSpeedPercent = 100;


    [Header("Mobility Upgrades")]
    [Tooltip("Percentage multiplying player movement speed.")]
    public int movespeedPercent = 100;

    [Header("Other Upgrades")]
    public int pickupRangePercent = 100;

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
