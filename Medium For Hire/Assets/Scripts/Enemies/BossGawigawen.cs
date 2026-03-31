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

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Attack Settings")]
    public float attackRange = 5f;          
    public float attackThresholdY = -3.5f; // vertical threshold for attack

    private bool nextAttackIsAxe = true;

    public float bossWeaponAttackDamage = 15f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        currentState = GawigawenState.Approach;
        nextAttackIsAxe = true;

        if (animator != null)
        {
            animator.SetBool("isSwinging", false);
            animator.SetBool("isThrusting", false);
        }
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

        return verticalDifference > attackThresholdY ;
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
}
