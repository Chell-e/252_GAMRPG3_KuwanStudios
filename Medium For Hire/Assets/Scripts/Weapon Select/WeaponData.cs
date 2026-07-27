using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponSelect/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("REFERENCES")]
    public GameObject weaponPrefab;
    public WeaponUnlock mainWeaponUIData;
    // sprite?

    [Header("UI")]
    [TextArea(2, 2)]
    public string weaponTitle;
    [Space]
    [TextArea(5,7)]
    public string weaponDescription;
    [Space]
    [TextArea(5, 7)]
    public string normalBehavior;
    [Space]
    [TextArea(5, 7)]
    public string aimedBehavior;
    // auto, aimed, evolutions
}
