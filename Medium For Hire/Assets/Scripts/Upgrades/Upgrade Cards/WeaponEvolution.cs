using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponEvolutionType
{
    // OFFENSE
    OffenseEvolution = 1,

    // SURVIVAL
    SurvivalEvolution = 2,

    // UTILITY
    UtilityEvolution = 3
}

[CreateAssetMenu(menuName = "Upgrades/Upgrade Types/Weapon Evolution")]
public class WeaponEvolution : BaseUpgradeData
{
    //public string baseWeapon; // weapon to replace
    //public GameObject evolvedWeapon; // weapon prefab to replace with?

    public WeaponEvolutionType evolutionType;
    

    //public override void Apply(PlayerStats player) { }
}
