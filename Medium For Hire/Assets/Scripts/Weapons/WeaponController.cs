using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EXCLUSIVELY responsible for SPAWNING/INSTANTIATING weapons, basing it off of stats from weapon data
public class WeaponController : MonoBehaviour
{
    public WeaponData weaponData;
    private float spawnCounter;
    public float cooldown;

    void Update()
    {
        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0)
        {
            spawnCounter = cooldown;
            Fire();
        }
    }
    public void Fire()
    {
        var projectileObj = Instantiate(weaponData.weaponPrefab, transform.position, transform.rotation);

        var projectileMovement = projectileObj.GetComponent<ProjectileMovement>();
        projectileMovement.weaponData = weaponData;

        var projectileDamage = projectileObj.GetComponent<ProjectileDamage>();
        projectileDamage.weaponData = weaponData;

        if (weaponData.isAimed)
        {
            projectileMovement.AimAtCursor();
        }
        else
        {
            projectileMovement.AimAtPlayerDirectionX(PlayerController.Instance.GetLastFacingDirectionX());
        }
    }
}
