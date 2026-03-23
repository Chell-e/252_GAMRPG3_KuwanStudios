using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUpgradeData : ScriptableObject
{
    public Sprite icon;
    public string title;

    [TextArea(2, 6)]
    public string description;
    public Color cardOutlineColor;

            [Tooltip("How many times this can be picked. -1 for uncapped.")]
    public int maxPicks; // how many of these you can pick; -1 for indefinite
            [Tooltip("Whether Upgrade can show up as multiple cards.")]
    public bool canDuplicate; // whether this can be rolled multiple times per upgrade draw

            [Tooltip("All requirements must be met to show the card.")]
    public List<BaseUpgradeRequirement> requirements; // used to determine whether to include this upgrade in the roll

    //public abstract void Apply(PlayerStats player); // may not be needed because of UpgradeManager 

}
