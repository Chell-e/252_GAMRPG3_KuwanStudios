using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatUpgradeType
{
    // OFFENSE
    DamagePercent=1,
    AttackSpeedPercent=2,

    // SURVIVAL
    MaxHealthPercent=3,
    MoveSpeedPercent=4,

    // UTILITY
    ProjectileSpeedPercent=5,
    AreaPercent=6,
    PickupRangePercent=7,


    // MISC
    Heal=8,

    // DOMAIN STAT
    OffenseBonus=9,
    SurvivalBonus=10,
    UtilityBonus=11
}

[CreateAssetMenu(menuName = "Upgrades/Upgrade Types/Stat Upgrade")]
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
