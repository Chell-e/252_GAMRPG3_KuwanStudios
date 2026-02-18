using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Level")]
    //public int maxLevel;
    public int currentLevel;
    public int expToLevel;
    //public List<int> expToLevelUp;

    [Header("EXP")]
    public int currentEXP;

    [Header("Attack")]
    public int attackDamage;
    public int attackSpeed;
    public int critChance;

    [Header("Mobility")]
    public int moveSpeed;
    public int pickupRange;
}
