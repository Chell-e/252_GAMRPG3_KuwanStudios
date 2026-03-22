 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeath : MonoBehaviour
{
    private HealthComponent health;

    private void Awake()
    {
        health = GetComponent<HealthComponent>();
        if (health != null )
        {
            health.OnDeath += HandleDeath; // Subscribe
        }
    }

    public void HandleDeath()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        BaseEnemy baseEnemy = GetComponent<BaseEnemy>();

        if (playerController != null)
        {
            gameObject.SetActive(false);
        }
        else if (baseEnemy != null)
        {
            DropLoot(baseEnemy);
            PoolManager.ReturnObjectToPool(gameObject);
        }
    }

    private void DropLoot(BaseEnemy enemy)
    {
        foreach (var drop in enemy.possibleDrops)
        {
            var random = Random.value;
            Debug.Log(random);
            if (random <= drop.dropChance)
            {
                PoolManager.SpawnObject(drop.itemPrefab, transform.position, Quaternion.identity, PoolManager.PoolType.ExpOrb);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        if ( health != null )
        {
            health.OnDeath -= HandleDeath; // Unsubscribe
        }
    }
}
