using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;

    [Header("Weapon Stats")]
    public int damage;
    public int attackSpeed;
    public int attackInterval;
    public float projectileSpeed;
    public float duration;
    //public float projectileSize;
    //public float critChance;

    [Header("Attack Pattern")]
    public bool isAimed;
}
