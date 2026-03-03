using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piercing : MonoBehaviour
{
    public WeaponData weaponData;
    private ProjectileDamage projectileDamage;

    [SerializeField] private int piercingCount = 3; 
    [SerializeField] private int piercedEnemies = 0;
    private void Awake()
    {
        projectileDamage = GetComponent<ProjectileDamage>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        projectileDamage.ApplyDamage(collision.gameObject);

        piercedEnemies++;
        if (piercedEnemies >= piercingCount)
        {
            Destroy(gameObject); 
        }
    }
}
