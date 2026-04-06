using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stat
{
    // LEVELS
    CurrentLevel,
    CurrentExp,
    ExpToLevel,
    RemainingLevels, // decrement this

    // HP
    CurrentHealth,
    MaxHealth,
    HealthPercentLeft,

    // MOVESPEED
    BaseMoveSpeed,
    MoveSpeedPercent,
    FinalMoveSpeed,
    FinalAimedMoveSpeed,

    // DMG
    DamagePercent,

    // ATK SPD
    AttackSpeedPercent,

    // PROJECTILE SPD
    ProjectileSpeedPercent,

    // PICKUP RANGE
    PickupRange,



    // DOMAIN STUFF
    DomainOffense,
    DomainSurvival,
    DomainUtility,
}

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
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

    // ========== METAPROGRESSION (TWEAKABLE)
        [Header("Metaprogression Stat Bonus Per Level")]
    public static int healthBonusPerLevel = 20;
    public static int dmgBonusPerLevel = 10;
    public static int atkSpeedBonusPerLevel = 10;
    public static int moveSpeedBonusPerLevel = 5;
    public static int projSpeedBonusPerLevel = 15;
    public static int pickupRangeBonusPerLevel = 25;


    // ****** CHANGE ALL OF THE BELOW THESE TO PRIVATE LATER. ENCAPSULATION.
    // MAKE NOW, FIX LATER.
        [Header("Player Level")]
    //public int maxLevel;
    public int currentLevel;
    public int expToLevel;
    public int currentExp;
    public int remainingLevels; // relative to max
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
            case Stat.RemainingLevels:
                return remainingLevels;

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
            case Stat.FinalAimedMoveSpeed:
                return movespeedBase * (movespeedAimingPercent / 100f);

            // DAMAGE
            case Stat.DamagePercent:
                return dmgPercent;

            // ATK SPEED
            case Stat.AttackSpeedPercent:
                return dmgPercent;

            // PROJECTILE SPEED
            case Stat.ProjectileSpeedPercent:
                return dmgPercent;


            // DOMAIN STUFF
            case Stat.DomainOffense:
                return offenseDomainStat;
            case Stat.DomainSurvival:
                return survivalDomainStat;
            case Stat.DomainUtility:
                return utilityDomainStat;


            default:
                Debug.Log($"PlayerStats: unhandled stat type {statToFinalize}");
                return -1f;
        }
    }

    // ****** ENCAPSULATION: EXTERNAL CLASSES SHOULD CALL THIS INSTEAD. 
    public void UpdatePlayerStat(Stat statToUpdate)
    {
        /*switch (statToUpdate)
        {
        }*/

    }

    // debug
    [ContextMenu("Force Level Up")]
    void ForceLevelUp()
    {
        Debug.Log("Forced level up");
        GainExperience(999999);
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
            // find a cleaner way for this
            remainingLevels--;


            if ((currentLevel+1) % 3 == 0) UpgradeManager.Instance.ShowUpgradeOptions(true);
            else UpgradeManager.Instance.ShowUpgradeOptions(false);

            

            UIManager.Instance.UpdateExpUI();
        }

        /*int index = playerStats.currentLevel - 1;
        if (playerStats.currentEXP >= playerStats.expToLevelUp[index])
        {
            playerStats.expToLevelUp[index]++;
        }*/
    }

    public void ApplyShopPurchases()
    {
        PlayerData data = PlayerData.Instance;

        maxHealth = 100 + (data.healthLevel * healthBonusPerLevel);
        dmgPercent = 100 + (data.damageLevel * dmgBonusPerLevel);
        atkSpeedPercent = 100 + (data.attackSpeedLevel * atkSpeedBonusPerLevel);
        movespeedBase = 100 + (data.moveSpeedLevel * moveSpeedBonusPerLevel);
        projectileSpeedPercent = 100 + (data.projectileSpeedLevel * projSpeedBonusPerLevel);
        pickupRangeBase = 100 + (data.pickupRangeLevel * pickupRangeBonusPerLevel);
    }
}
