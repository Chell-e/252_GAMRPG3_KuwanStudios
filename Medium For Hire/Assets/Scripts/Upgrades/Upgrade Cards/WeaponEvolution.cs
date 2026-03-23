using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Upgrade Types/Weapon Evolution")]
public class WeaponEvolution : BaseUpgradeData
{
    public string baseWeapon; // weapon to replace
    public GameObject evolvedWeapon; // weapon prefab to replace with?

    //public override void Apply(PlayerStats player) { }
}
