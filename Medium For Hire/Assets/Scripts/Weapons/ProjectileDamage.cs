using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public WeaponData weaponData;

    public void ApplyDamage(GameObject target)
    {
        EnemyAI enemy = target.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            // kills enemy
            enemy.TakeDamage(weaponData.damage);
        }
    }
}
