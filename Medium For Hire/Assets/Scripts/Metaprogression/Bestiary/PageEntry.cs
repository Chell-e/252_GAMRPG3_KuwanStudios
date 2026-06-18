using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bestiary/Page Entry")]
public class PageEntry : ScriptableObject
{
    public string entryName;
    public string entryDescription;
    public string entryLore;
    public Sprite entrySprite;

    [Header("Unlock Threshold")]
    public int killsNeededToUnlockName = 30;
    public int killsNeededToUnlockImage = 50;
    public int killsNeededToUnlockLore = 100;

    // total kills needed for full unlock
    public int MaxKillsNeeded => killsNeededToUnlockLore;
}