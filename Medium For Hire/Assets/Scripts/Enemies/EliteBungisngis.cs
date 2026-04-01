using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum  BungisngisState
{
    Approach,
    Panic
}

public class EliteBungisngis : BaseEnemy
{
    [Header("Bungisngis Settings")]
    public BungisngisState currentState = BungisngisState.Approach;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Panic Settings")]
    public float lowHealthThreshold = 0.5f; // panics when health is below 50% 
    public float panicDuration = 5f;
    public float panicSpeedMultiplier = 5f;
    private float panicTimer = 0f;
    private bool hasPanicked = false;

    [Header("Attack Settings")]
    public float throwRange = 8f;

    [Header("Boulder")]
    [SerializeField] private GameObject boulderPrefab;
    public float boulderDamage = 5f;

    private float shootRate = 2f;
    private float shootTimer = 0f;

    protected override void OnEnable()
    {
        base.OnEnable();

        currentState = BungisngisState.Approach;
        hasPanicked = false;

        if (animator != null)
        {
            animator.SetBool("isPanicking", false);
        }
    }

    protected override void Update()
    {
        if (IsPlayerDead())
        {
            animator.enabled = false;
        }

        base.Update();

        UpdateBungisngisState();
        ExecuteCurrentState();
    }

    private void UpdateBungisngisState()
    {
        if (playerTransform == null) return;

        // if currently panicking, stay in panic until timer ends
        if (currentState == BungisngisState.Panic)
        {
            panicTimer -= Time.deltaTime;

            // if panic timer ends, return to approach state
            if (panicTimer <= 0f)
            {
                animator.SetBool("isPanicking", false);
                currentState = BungisngisState.Approach;
            }
            return; 
        }

        // trigger panic only ONCE when health drops below threshold
        if (!hasPanicked && health.GetCurrentHealth() <= health.GetMaxHealth() * lowHealthThreshold)
        {
            currentState = BungisngisState.Panic;
            hasPanicked = true;

            panicTimer = panicDuration;
            animator.SetBool("isPanicking", true);
            return;
        }

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist <= throwRange)
        {
            ThrowBoulder();
        }
    }


    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case BungisngisState.Approach:
                break;

            case BungisngisState.Panic:
                animator.SetBool("isPanicking", true);
                Panic();

                break;
        }
    }

    private void ThrowBoulder()
    {
        // boulder shooting 
        shootTimer -= Time.deltaTime;

        if (shootTimer < 0f)
        {
            shootTimer = shootRate;

            GameObject boulderObj = PoolManager.SpawnObject(boulderPrefab, transform.position, Quaternion.identity, PoolManager.PoolType.Projectile);

            Boulder boulder = boulderObj.GetComponent<Boulder>();
            if (boulder != null)
            {
                boulder.SetTarget(playerTransform);
                boulder.SetDamage(boulderDamage);
            }
        }

    }

    private void Panic()
    {
        if (panicTimer >= 0)
        {
            panicTimer -= Time.deltaTime;
            
            Vector2 fleeDir = (transform.position - playerTransform.position).normalized;
            rb.velocity = fleeDir * baseMoveSpeed * panicSpeedMultiplier;
        }
    }

    protected override void LookAtTarget()
    {
        if (currentState == BungisngisState.Panic)
        {
            bool facingAway = playerController.transform.position.x < transform.position.x;
            transform.rotation = facingAway
                ? Quaternion.Euler(0f, 180f, 0f)
                : Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            base.LookAtTarget();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, throwRange);
    }
}
