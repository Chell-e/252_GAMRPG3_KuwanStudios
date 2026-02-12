using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    private HitFlash hitFlash;

    [Header("Physics")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Orb Drop")]
    public GameObject orbPrefab;

    private EnemyStats enemyStats;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        hitFlash = GetComponent<HitFlash>();
    }
    void Update()
    {
        LookAtPlayer();
    }

    private void FixedUpdate()
    {
        MoveTowardPlayer();       
    }

    private void LookAtPlayer()
    {
        if (PlayerController.Instance == null)
            return;

        // Flips sprite to always face the player
        spriteRenderer.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }
    private void MoveTowardPlayer()
    {
        if (PlayerController.Instance.gameObject.activeSelf)
        {
            // Get distance from player 
            Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
            rb.velocity = new Vector2(
                direction.x * enemyStats.moveSpeed,
                direction.y * enemyStats.moveSpeed
            );
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            PlayerController.Instance.GetComponent<PlayerController>().TakeDamage(enemyStats.damage);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        enemyStats.health -= damageAmount;
        if (enemyStats.health <= 0)
        {
            PoolManager.ReturnObjectToPool(gameObject);
            PoolManager.SpawnObject(orbPrefab, transform.position, transform.rotation, PoolManager.PoolType.ExpOrb);
        }

        // Hit flash fx
        hitFlash.TriggerHitFlash();
    }
}
