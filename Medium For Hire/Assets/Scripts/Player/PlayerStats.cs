using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("EXP")]
    public int currentEXP;

    [Header("Health")]
    public int currentHealth;
    public int maxHealth;

    [Header("Attack")]
    public int attackDamage;
    public int attackSpeed;
    public int critChance;

    [Header("Mobility")]
    public int moveSpeed;
    public int pickupRange;
}
