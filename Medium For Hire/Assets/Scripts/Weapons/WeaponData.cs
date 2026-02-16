using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;

    [Header("Weapon Stats")]

    [Tooltip("test")]
    public int damage;
    public int attackSpeed;
    public int attackInterval;
    public float projectileSpeed;
    public float duration;
    //public float projectileSize;
    //public float critChance;


    [Tooltip("Base DMG of the weapon projectile." +
        "\nMultiplied by Player DMG% stat.")]
    public float weaponDamage;

    [Tooltip("Base cooldown between \"firing states\"." +
        "\nDivided by Player CooldownReduction% stat.")]
    public float weaponCooldown;

    [Tooltip("Base duration of \"firing state\", assuming it exists." +
        "\nSet to 0 if none or instant firing state." +
        "\nDivided by Player Duration% stat.")]
    public float weaponDuration;

    [Tooltip("Base size of weapon projectiles, etc." +
        "\nMultiplied by Player Size% stat.")]
    public float weaponSize;

    [Tooltip("Base travel speed of GENERATED PROJECTILES." +
        "\nMultiplied by Player .")]
    public float weaponSpeed;

    [Tooltip("Internal CD of weapons that CAN HIT THE SAME TARGET MULTIPLE TIMES." +
        "\nSet to 0 if disabled.")]
    public float weaponAttackInterval = 0;









    [Header("Attack Pattern")]
    public bool isAimed;
}
