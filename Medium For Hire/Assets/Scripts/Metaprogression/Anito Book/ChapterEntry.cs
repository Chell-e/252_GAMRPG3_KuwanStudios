using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bestiary/Chapter Entry")]
public class ChapterEntry : ScriptableObject
{
    public Sprite chapterSprite;
    public string chapterName;
    public string chapterDescription;
    public bool chapterUnlocked;

    [Header("Unlock Threshold")]
    public int killsNeededToUnlockName;
    public int killsNeededToUnlockImage;
    public int killsNeededToUnlockDesc;
}