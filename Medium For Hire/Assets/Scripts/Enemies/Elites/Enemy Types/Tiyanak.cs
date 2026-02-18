using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiyanak : MonoBehaviour
{
    private enum TiyanakState
    {
        Approach,
        Lure,
        Transform
    }

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private HealthComponent healthComponent;

    [Header("Tiyanak Stats")]
    [SerializeField] private float pullStrength = 3f;
    Vector2 pullForce;

    [Header("Lure Radius")]
    [SerializeField] private CircleCollider2D lureTrigger;

    private TiyanakState currentState = TiyanakState.Approach;
    private HitFlash hitFlash;  

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        healthComponent = GetComponent<HealthComponent>();
        hitFlash = GetComponent<HitFlash>();
        lureTrigger = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
    }

    private void Update()
    {
        LookAtPlayer();

        switch (currentState)
        {
            case TiyanakState.Approach:
                MoveTowardPlayer();
                break;
            case TiyanakState.Lure:
                PullPlayerIn();
                break;
            case TiyanakState.Transform:
                // to be implemented 
                break;
        }
    }

    private void PullPlayerIn()
    {
        var player = PlayerController.Instance;
        var playerRb = player.GetComponent<Rigidbody2D>();

        if (player == null || !player.gameObject.activeSelf)
            return;
        
        var playerPos = player.transform.position;
        float distanceToPlayer = Vector2.Distance(playerPos, transform.position);
        if (distanceToPlayer <= lureTrigger.radius)
        {
            pullForce = (transform.position - playerPos).normalized / distanceToPlayer * pullStrength;
            playerRb.AddForce(pullForce, ForceMode2D.Force);

        }

        Debug.Log($"Pulling player in with force {pullForce} at distance {distanceToPlayer}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState != TiyanakState.Approach)
            return;

        if (collision.gameObject.GetComponent<PlayerController>())
        {
            currentState = TiyanakState.Lure;
            rb.velocity = Vector2.zero;
            Debug.Log("Player entered lure radius, switching to Lure state.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentState != TiyanakState.Lure)
            return;

        if (collision.gameObject.GetComponent<PlayerController>())
        {
            currentState = TiyanakState.Approach;
            Debug.Log("Player exited lure radius, switching back to Approach state.");
        }
    }
    private void LookAtPlayer()
    {
        if (PlayerController.Instance == null)
            return;

        var playerPosition = PlayerController.Instance.transform.position;
        spriteRenderer.flipX = playerPosition.x < transform.position.x;
    }

    private void MoveTowardPlayer()
    {
        if (PlayerController.Instance == null || !PlayerController.Instance.gameObject.activeSelf)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(
            direction.x * enemyStats.moveSpeed,
            direction.y * enemyStats.moveSpeed
        );
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            PlayerController.Instance.ApplyDamage(enemyStats.damage);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        var health = GetComponent<HealthComponent>();
        if (health != null)
        {
            health.TakeDamage(damageAmount);
        }
        if (hitFlash != null)
        {
            hitFlash.TriggerHitFlash();
        }
    }
}