using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    [Header("Projectile Prefab")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Projectile Stats")]
    public int damage;
    public float projectileSpeed;
    public float cooldown;
    public float lifespan;
    private float spawnCounter;

    void Start()
    {

    }

    void Update()
    {
        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0)
        {
            spawnCounter = cooldown;
            Instantiate(projectilePrefab, transform.position, transform.rotation, transform);
        }
    }
}
