using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OnDeath : MonoBehaviour
{
    [SerializeField] public PlayerEvents Events;
    private HealthComponent health;

    public static event System.Action OnBossDeath;

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
        if (baseEnemy.enemyType != EnemyType.Boss)
        {
            DropLoot(baseEnemy);
        }

        baseEnemy.SpawnDeathAnimation();

        PoolManager.ReturnObjectToPool(gameObject);
        PoolSpawner.Instance.NotifyEnemyDespawned();

        StageManager.Instance.RegisterKill(baseEnemy.enemyID, baseEnemy.enemyType.ToString());

        if (baseEnemy.enemyType == EnemyType.Boss)
        {
            OnBossDeath?.Invoke();
        }
    }

    private void DropLoot(BaseEnemy enemy)
    {
        if (enemy.possibleDrops == null) return;

        foreach (var drop in enemy.possibleDrops)
        {
            if (drop == null) continue;

            float roll = Random.Range(0f, 100f);
            if (roll <= drop.dropChance)
            {
                int orbCount = (enemy.enemyType == EnemyType.Elite) ? 5 : 1;
                
                for (int i = 0; i < orbCount; i++)
                {
                    Vector3 spawnPos = transform.position;

                    if (orbCount > 1)
                    {
                        Vector2 offset = UnityEngine.Random.insideUnitCircle * 0.5f;
                        spawnPos += new Vector3(offset.x, offset.y, 0f);
                    }

                    // first target pool is xp orbs
                    PoolManager.PoolType targetPool = PoolManager.PoolType.ExpOrb;
                    // if there's an hp orb, its pool will be targeted 
                    if (drop.itemPrefab.GetComponent<HealthOrb>() != null)
                    {
                        targetPool = PoolManager.PoolType.HpOrb;
                    }

                    GameObject spawnedDrop = PoolManager.SpawnObject(
                        drop.itemPrefab, transform.position, Quaternion.identity, targetPool
                    );

                    if (spawnedDrop == null) continue;

                    // if xp orb
                    if (spawnedDrop.TryGetComponent<ExpOrb>(out var expOrb))
                    {
                        int finalXp = enemy.expReward;
                        expOrb.Initialize(finalXp);
                    }
                    else if (PlayerController.Instance != null && spawnedDrop.TryGetComponent<HealthOrb>(out var healthOrb))
                    {
                        float healAmnt = healthOrb.hpToGive;
                        PlayerController.Instance.GetComponent<HealthComponent>().Heal(healAmnt);
                    }
                }

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