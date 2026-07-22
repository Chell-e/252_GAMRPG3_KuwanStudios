using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Upgrade Tier")]
public class UpgradeTier : ScriptableObject
{
    public string rarityName;
    public float rarityWeight;

    public List<BaseUpgradeData> upgradePool;
}