using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GawigawenState
{
    Approach,
    Attack
}

public class BossGawigawen : BaseEnemy
{
    [Header("Boss Gawigawen Settings")]
    public GawigawenState currentState = GawigawenState.Approach;
    [SerializeField] private float baseHealth;
    [SerializeField] private float baseDamage;

    [Header("Superstition Buffs")]
    [SerializeField] private float healthBuffMultiplier = 1.1f;
    [SerializeField] private float damageBuffMultiplier = 1.1f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Attack Settings")]
    public float attackRange = 5f;
    public float attackThresholdY = -3.5f; // vertical threshold for attack

    private bool nextAttackIsAxe = true;

    public float bossWeaponAttackDamage = 15f;

    protected override void OnEnable()
    {
        base.OnEnable();

        baseHealth = health.GetMaxHealth();
        baseDamage = bossWeaponAttackDamage;

        currentState = GawigawenState.Approach;
        nextAttackIsAxe = true;

        if (animator != null)
        {
            animator.SetBool("isSwinging", false);
            animator.SetBool("isThrusting", false);
        }

        // listen to superstition manager
        SuperstitionManager.OnSuperstitionBroken += ApplyBuff;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        health.SetMaxHealth(baseHealth);
        bossWeaponAttackDamage = baseDamage;

        SuperstitionManager.OnSuperstitionBroken -= ApplyBuff;
    }

    protected override void Update()
    {
        if (IsPlayerDead())
        {
            animator.enabled = false;
        }

        base.Update();

        UpdateGawigawenState();
        ExecuteCurrentState();
    }

    private void UpdateGawigawenState()
    {
        if (playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        // check if player is within attack range and above the vertical threshold for attacking
        if (dist <= attackRange && IsPlayerInAttackablePosition())
        {
            currentState = GawigawenState.Attack;
        }
        else
        {
            currentState = GawigawenState.Approach;
        }
    }

    // prevents attacking if the player is below it
    private bool IsPlayerInAttackablePosition()
    {
        if (playerTransform == null) return false;

        float verticalDifference = playerTransform.position.y - transform.position.y;

        return verticalDifference > attackThresholdY;
    }

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case GawigawenState.Approach:
                animator.SetBool("isSwinging", false);
                animator.SetBool("isThrusting", false);
                break;

            case GawigawenState.Attack:
                if (nextAttackIsAxe && currentState != GawigawenState.Approach)
                {
                    animator.SetBool("isSwinging", true);
                    animator.SetBool("isThrusting", false);
                }
                else
                {
                    animator.SetBool("isSwinging", false);
                    animator.SetBool("isThrusting", true);
                }

                // alternate attack type for next attack
                nextAttackIsAxe = !nextAttackIsAxe;

                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == playerController.gameObject)
        {
            playerController.TakeDamage(bossWeaponAttackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // =========== SUPERSTITION BUFF
    public void BuffHealth(float multiplier)
    {
        // get base max health
        // multiply it by the multiplier
        // set it as the new max health

        health.SetMaxHealth(health.GetMaxHealth() * multiplier);
    }

    public void BuffDamage(float multiplier)
    {
        // multiply the boss's weapon attack damage by the multiplier
        bossWeaponAttackDamage *= multiplier;
    }
    // =========== SUPERSTITION BUFF

    private void ApplyBuff(int defyCount)
    {
        Debug.Log("buff applied");

        // multiplicative if within 5 defies ?
        if (defyCount <= 5)
        {
            BuffHealth(healthBuffMultiplier);
            BuffDamage(damageBuffMultiplier);
        }
        else
        {
            // additive after ? HSHS
            BuffHealth(1f + (0.05f * defyCount));
            BuffDamage(1f + (0.05f * defyCount));
        }

    }
}