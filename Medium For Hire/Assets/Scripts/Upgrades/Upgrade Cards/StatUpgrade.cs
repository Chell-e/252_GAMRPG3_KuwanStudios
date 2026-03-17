using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatUpgradeType
{
    // OFFENSE
    DamagePercent,
    AttackSpeedPercent,

    // SURVIVAL
    MaxHealthPercent,
    MoveSpeedPercent,

    // UTILITY
    ProjectileSpeedPercent,
    PickupRangePercent,


    // MISC
    Heal,

    // DOMAIN STAT
    OffenseBonus,
    SurvivalBonus,
    UtilityBonus
}

[CreateAssetMenu(menuName = "Upgrades/Stat Upgrade")]
public class StatUpgrade : BaseUpgradeData
{
    [System.Serializable]
    public class StatUpgradeData
    {
        public StatUpgradeType statToUpgrade;
        public int value;
    }

    [SerializeField] public List<StatUpgradeData> statsUpgraded = new List<StatUpgradeData>();

    //public override void Apply(PlayerStats player) { }
}
