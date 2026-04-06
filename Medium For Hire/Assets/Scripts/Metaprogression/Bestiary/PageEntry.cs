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
    public int killsNeededToUnlockName = 100;
    public int killsNeededToUnlockImage = 250;
    public int killsNeededToUnlockLore = 400;

    // total kills needed for full unlock
    public int MaxKillsNeeded => killsNeededToUnlockLore;
}
