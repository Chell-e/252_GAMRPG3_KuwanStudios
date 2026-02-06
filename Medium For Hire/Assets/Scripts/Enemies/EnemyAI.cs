using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Physics")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Orb Drop")]
    public GameObject orbPrefab;

    private EnemyStats enemyStats;

    void Start()
    {
        // get reference to enemy stats
        enemyStats = GetComponent<EnemyStats>();
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
        // safety check for when player is destroyed later (killed emz)
        if (PlayerController.Instance == null)
            return;

        // flips sprite to always face the player
        spriteRenderer.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }
    private void MoveTowardPlayer()
    {
        // gets distance from player 
        Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(
            direction.x * enemyStats.moveSpeed,
            direction.y * enemyStats.moveSpeed
        );
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // contact damage to player
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            // returns enemy back
            PoolManager.ReturnObjectToPool(gameObject);

            // drop exp when killed
            PoolManager.SpawnObject(orbPrefab, transform.position, transform.rotation, PoolManager.PoolType.ExpOrb);

            // Damage player here
        }

    }
}
