using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OnDeath : MonoBehaviour
{
    [SerializeField] public PlayerEvents Events;
    private HealthComponent health;

    private void Awake()
    {
        health = GetComponent<HealthComponent>();
        if (health != null)
        {
            health.OnDeath += HandleDeath; // Subscribe
        }
    }

    public void HandleDeath()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        BaseEnemy baseEnemy = GetComponent<BaseEnemy>();

        if (playerController)
        {
            HandlePlayerDeath();
        }
        else if (baseEnemy != null)
        {
            HandleEnemyDeath(baseEnemy);
        }
    }

    public void HandlePlayerDeath()
    {
        if (Events == null)
        {
            Events = GetComponent<PlayerEvents>();
        }

        if (Events != null)
        {
            Events.OnPlayerDeath?.Invoke();
        }

        gameObject.SetActive(false);
    }

    public void HandleEnemyDeath(BaseEnemy baseEnemy)
    {
        DropLoot(baseEnemy);

        baseEnemy.SpawnDeathAnimation();

        PoolManager.ReturnObjectToPool(gameObject);
        PoolSpawner.Instance.NotifyEnemyDespawned();

        StageManager.Instance.RegisterKill(baseEnemy.enemyID, baseEnemy.enemyType.ToString());
    }

    private void DropLoot(BaseEnemy enemy)
    {

        foreach (var drop in enemy.possibleDrops)
        {
            if (drop == null)
                return;

            var random = Random.value;
            if (random <= drop.dropChance)
            {
                PoolManager.SpawnObject(drop.itemPrefab, transform.position, Quaternion.identity, PoolManager.PoolType.ExpOrb);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath; // Unsubscribe
        }
    }
}