using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    Heal,
    MaxHealthIncrease,
    MoveSpeedIncrease,
    DamageIncrease
}

[CreateAssetMenu(fileName = "UpgradeDefinition", menuName = "Upgrades/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    public Sprite icon;

    public string title;
    [TextArea(2,6)]
    public string description;

    public UpgradeType upgradeType;
    public int intValue;
    public float floatValue;

    public int duplicates; // how many times you can get this upgrade; set to -1 for infinite.
}