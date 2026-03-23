using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Upgrade Requirements/Universal")]
public class UniversalRequirement : BaseUpgradeRequirement
{
    public override bool IsAvailable()
    {
        return true; // universal is always available to be rolled
    }
}
