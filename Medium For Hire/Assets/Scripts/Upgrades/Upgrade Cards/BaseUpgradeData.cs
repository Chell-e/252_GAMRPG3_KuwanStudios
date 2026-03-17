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

    public int maxPicks; // how many of these you can pick; -1 for indefinite
    public bool canDuplicate; // whether this can be rolled multiple times per upgrade draw

    //public abstract void Apply(PlayerStats player); // may not be needed because of UpgradeManager 

}
