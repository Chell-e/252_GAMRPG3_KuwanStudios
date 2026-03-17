using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Weapon Unlock")]
public class WeaponUnlock : BaseUpgradeData
{
    public GameObject weaponPrefab;

    /*public override void Apply(PlayerStats player)
    {
        // apply weapon to player

        // may need to add another abstract function to override in BaseUpgradeData
    }*/
}
