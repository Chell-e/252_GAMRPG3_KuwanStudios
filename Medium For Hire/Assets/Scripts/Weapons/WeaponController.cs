using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for firing projectiles based on weapon data
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
        //var projectileBehavior = projectileObj.GetComponent<ProjectileBehavior>();

        //projectileBehavior.GetComponent<ProjectileBehavior>().SetData(weaponData, PlayerController.Instance); 
        var movement = projectileObj.GetComponent<ProjectileMovement>();
        movement.weaponData = weaponData;

        var damage = projectileObj.GetComponent<ProjectileDamage>();
        damage.weaponData = weaponData;

        if (weaponData.isAimed)
        {
            movement.AimAtCursor();
        }
        else
        {
            movement.AimAtPlayerDirection(PlayerController.Instance.GetLastFacingDirectionX());
        }
    }
}
