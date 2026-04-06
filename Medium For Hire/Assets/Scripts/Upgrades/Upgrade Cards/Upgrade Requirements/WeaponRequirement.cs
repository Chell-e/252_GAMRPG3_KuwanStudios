using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Scripting;

public enum WeaponSource
{
    JuruPakal=1,
    TboliBells=2
}

[CreateAssetMenu(menuName = "Upgrades/Upgrade Requirements/Weapon Requirement")]
public class WeaponRequirement : BaseUpgradeRequirement
{
    public WeaponSource requiredWeapon;

    public override bool IsAvailable()
    {
        BaseWeapon mainWeapon = PlayerController.Instance.weaponManager.mainWeapon
            .GetComponent<BaseWeapon>();

        switch (requiredWeapon)
        {
            case (WeaponSource.JuruPakal):
                return mainWeapon is JuruPakalController;

            case (WeaponSource.TboliBells):
                return mainWeapon is MainWeapon_TboliBells;
        }

        return false;
        //return PlayerController.Instance.weaponManager.mainWeapon 

    }
}
