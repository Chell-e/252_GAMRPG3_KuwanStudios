using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stat
{
    // LEVELS
    CurrentLevel,
    CurrentExp,
    ExpToLevel,

    // HP
    CurrentHealth,
    MaxHealth,
    HealthPercentLeft,

    // MOVESPEED
    BaseMoveSpeed,
    MoveSpeedPercent,
    FinalMoveSpeed,

    // DMG
    DamagePercent,

    // ATK SPD
    AttackSpeedPercent,

    // PROJECTILE SPD
    ProjectileSpeedPercent,

    // PICKUP RANGE
    PickupRange
}

public class PlayerStats : MonoBehaviour
{


    // ****** CHANGE ALL OF THE BELOW THESE TO PRIVATE LATER. ENCAPSULATION.
    // MAKE NOW, FIX LATER.
        [Header("Player Level")]
    //public int maxLevel;
    public int currentLevel;
    public int expToLevel;
    public int currentExp;
    //public List<int> expToLevelUp;

    public bool isAiming = false;

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

    [Tooltip("Percentage of movespeed set to when aiming.")]
    public int movespeedAimingPercent = 50;

    [Header("Other Upgrades")]
    public int pickupRangePercent = 100;



        [Header("Domain Stats")]
    public int offenseDomainStat = 0;
    public int survivalDomainStat = 0;
    public int utilityDomainStat = 0;


    public float GetFinalMovespeed()
    {
        return movespeedBase * (movespeedPercent / 100f);
    }


    public float GetFinalAimedMovespeed()
    {
        return movespeedBase * (movespeedAimingPercent / 100f);
    }

    // GETTER
    public float GetPlayerStat(Stat statToFinalize)
    {
        switch (statToFinalize)
        {
            // EXP/LEVELS
            case Stat.CurrentLevel:
                return currentLevel;
            case Stat.CurrentExp:
                return currentExp;
            case Stat.ExpToLevel:
                return expToLevel;

            //  HEALTH
            case Stat.CurrentHealth:
                return currentHealth;
            case Stat.MaxHealth:
                return maxHealth;

            // MOVESPEED
            case Stat.MoveSpeedPercent:
                return movespeedPercent;
            case Stat.FinalMoveSpeed:
                return movespeedBase * (movespeedPercent / 100f);

            // DAMAGE
            case Stat.DamagePercent:
                return dmgPercent;

            // ATK SPEED
            case Stat.AttackSpeedPercent:
                return dmgPercent;

            // PROJECTILE SPEED
            case Stat.ProjectileSpeedPercent:
                return dmgPercent;

            default:
                Debug.Log($"PlayerStats: unhandled stat type {statToFinalize}");
                return -1f;
        }
    }

    // ****** ENCAPSULATION: EXTERNAL CLASSES SHOULD CALL THIS INSTEAD. 
    public void UpdatePlayerStat(Stat statToUpdate)
    {
        switch (statToUpdate)
        {
        }

        UpdateDomainStats();
    }

    private void UpdateDomainStats()
    {
        // OFFENSE
        /*int offense_dmgValue = Mathf.Max(dmgPercent - 100, 0);
        int offense_atkSpeedValue = Mathf.Max(atkSpeedPercent - 100, 0);

        offenseDomainStat = offense_dmgValue + offense_atkSpeedValue;*/

        
        // SURVIVAL
        /*int survival_hpValue = Mathf.Max(maxHealthPercent - 100, 0);
        int survival_moveSpeedValue = Mathf.Max(movespeedPercent - 100, 0);

        survivalDomainStat = survival_hpValue + survival_moveSpeedValue;*/


        // UTILITY
        /*int utility_projectileSpeedValue = Mathf.Max(projectileSpeedPercent - 100, 0);
        int utility_pickupRangeValue = Mathf.Max(pickupRangePercent - 100, 0);

        utilityDomainStat = utility_projectileSpeedValue + utility_pickupRangeValue;*/
    }


    public void GainExperience(int amount)
    {
        currentExp += amount;
        //currentExp += 10;
        UIManager.Instance.UpdateExpUI();

        if (currentExp >= expToLevel)
        {
            if (UpgradeManager.Instance == null) return;

            currentExp = 0;
            currentLevel++;

            if ((currentLevel) % 5 == 0) UpgradeManager.Instance.ShowUpgradeOptions(true);
            else UpgradeManager.Instance.ShowUpgradeOptions(false);

            

            UIManager.Instance.UpdateExpUI();
        }

        /*int index = playerStats.currentLevel - 1;
        if (playerStats.currentEXP >= playerStats.expToLevelUp[index])
        {
            playerStats.expToLevelUp[index]++;
        }*/
    }
}
