using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ManananggalState
{
    Approach,
    Immune,
    Regenerate
}
public class EliteManananggal_UpperHalf : BaseEnemy
{
    [Header("Manananggal State")]
    public ManananggalState currentState;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Lower Half")]
    public GameObject lowerHalfPrefab;

    private GameObject lowerHalfInstance;

    [Header("Stats")]
    public float immuneMoveSpeedMultiplier = 5f;

    [Header("Regeneration")]
    public float regenDuration = 5f;
    public float regenTimer;

    protected override void OnEnable()
    {
        base.OnEnable();

        currentState = ManananggalState.Approach;
        regenTimer = 0f;
        health.CanDie = false;

        SpawnLowerHalf();
    }

    private void SpawnLowerHalf()
    {
        if (lowerHalfPrefab == null) return;

        Vector2 spawnPos = RandomSpawnOutsideCamera();
        lowerHalfInstance = PoolManager.SpawnObject(lowerHalfPrefab, spawnPos, Quaternion.identity, PoolManager.PoolType.Enemy);

        var lowerHalf = lowerHalfInstance.GetComponent<EliteManananggal_LowerHalf>();
        if (lowerHalf != null)
        {
            lowerHalf.LinkToUpperHalf(this);
        }
    }

    protected override void Update()
    {
        if (IsPlayerDead())
        {
            animator.enabled = false;
        }

        base.Update();

        HandleState();
    }

    private void HandleState()
    {
        if (IsLowerHalfDead())
        {
            BecomeVulnerable();
            return;
        }

        switch (currentState)
        {
            case ManananggalState.Approach:
                if (health.GetCurrentHealth() <= 1f)
                {
                    EnterImmuneState();
                }
                break;

            case ManananggalState.Immune:

                MoveToLowerHalf();

                if (IsLowerHalfDead())
                {
                    BecomeVulnerable();
                    return;
                }

                if (IsNearLowerHalf())
                {
                    regenTimer = regenDuration;
                    EnterRegenerateState();
                }
                break;

            case ManananggalState.Regenerate:

                if (IsLowerHalfDead())
                {
                    BecomeVulnerable();
                    return;
                }

                rb.velocity = Vector2.zero;
                regenTimer -= Time.deltaTime;

                if (regenTimer <= 0f)
                {
                    health.ResetHealth();
                    SetUpperHalfColliderEnabled(true);
                    currentState = ManananggalState.Approach;
                }
                break;
        }
    }

    private void BecomeVulnerable()
    {
        currentState = ManananggalState.Approach;
        health.CanDie = true;
        SetUpperHalfColliderEnabled(true);
    }

    private void EnterImmuneState()
    {
        currentState = ManananggalState.Immune;
        SetUpperHalfColliderEnabled(false);
    }

    private void EnterRegenerateState()
    {
        currentState = ManananggalState.Regenerate;
        SetUpperHalfColliderEnabled(false);
    }

    public bool IsLowerHalfDead()
    {
        return lowerHalfInstance == null || lowerHalfInstance.GetComponent<HealthComponent>().IsDead;
    }

    private bool IsNearLowerHalf()
    {
        if (lowerHalfInstance == null) 
            return false;

        return Vector2.Distance(transform.position, lowerHalfInstance.transform.position) < 1.5f;
    }

    private void MoveToLowerHalf()
    {
        if (lowerHalfInstance == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector3 direction = (lowerHalfInstance.transform.position - transform.position).normalized;
        rb.velocity = direction * baseMoveSpeed * immuneMoveSpeedMultiplier;
    }

    protected override void LookAtTarget()
    {
        if (currentState == ManananggalState.Immune && lowerHalfInstance != null)
        {
            bool facingLowerHalf = lowerHalfInstance.transform.position.x < transform.position.x;
            transform.rotation = facingLowerHalf 
                ? Quaternion.Euler(0f, 0f, 0f) 
                : Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            base.LookAtTarget();
        }
    }

    private void SetUpperHalfColliderEnabled(bool enabled)
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = enabled;
        }
    }

    private void SetLowerHalfColliderEnabled(bool enabled)
    {
        Collider2D col = lowerHalfInstance.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = enabled;
        }
    }

    private Vector2 RandomSpawnOutsideCamera()
    {
        float edgeOffset = 1.1f;
        Vector2 pos = Random.value > 0.5f
            ? new Vector2(Random.value > 0.5f ? 1 - edgeOffset : edgeOffset, Random.value)
            : new Vector2(Random.value, Random.value > 0.5f ? 1 - edgeOffset : edgeOffset);

        return Camera.main.ViewportToWorldPoint(pos);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (lowerHalfInstance != null)
        {
            PoolManager.ReturnObjectToPool(lowerHalfInstance);
            lowerHalfInstance = null;
        }
    }
}
