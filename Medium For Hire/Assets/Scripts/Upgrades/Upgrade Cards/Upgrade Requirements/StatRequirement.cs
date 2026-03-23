using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Upgrade Requirements/Stat Requirement")]
public class StatRequirement : BaseUpgradeRequirement
{
    public int requiredStatValue;
    public Stat statType; // should primarily be domain


    public override bool IsAvailable()
    {
        return PlayerStats.Instance.GetPlayerStat(statType) >= requiredStatValue;
    }
}
