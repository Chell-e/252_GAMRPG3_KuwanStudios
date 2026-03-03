using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum State
{
    Approach,
    Immune,
    Regenerate,
    Dead
}

public class UpperHalfAI : MonoBehaviour, IDamageable
{
    public State currentState;

    [Header("References")]
    public Rigidbody2D rb;
    public HealthComponent health;
    public GameObject lowerHalfPrefab;
    private GameObject lowerHalfInstance;
    private HitFlash hitFlash;
    public SpriteRenderer spriteRenderer;

    [Header("Settings")]
    [SerializeField] public float regenDuration;
    [SerializeField] private float regenTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
        hitFlash = GetComponent<HitFlash>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        SpawnLowerHalf();

        currentState = State.Approach;
    }

    void Update()
    {
        LookAtPlayer();

        if (lowerHalfIsDestroyed() && currentState != State.Dead)
        {
            ChangeState(State.Dead);
        }

        switch (currentState)
        {
            case State.Approach:
                ApproachPlayer();

                if (health.IsDead())
                {
                    ChangeState(State.Immune);
                }

                break;
            case State.Immune:
                MoveToLowerHalf();
                if (isNearLowerHalf())
                {
                    regenTimer = regenDuration;
                    ChangeState(State.Regenerate);
                }
                break;
            case State.Regenerate:
                regenTimer -= Time.deltaTime;
                // play animation here

                if (lowerHalfIsDestroyed())
                {
                    ChangeState(State.Dead);
                }
                else if (regenTimer <= 0)
                {
                    health.ResetHealth();
                    ChangeState(State.Approach);
                }

                break;
            case State.Dead:
                Die();
                break;
        }
    }

    private void LookAtPlayer()
    {
        if (PlayerController.Instance == null)
            return;

        // Flips sprite to always face the player
        var playerPosition = PlayerController.Instance.transform.position;
        spriteRenderer.flipX = playerPosition.x < transform.position.x;
    }
    private bool isNearLowerHalf()
    {
        if (lowerHalfInstance == null) 
            return false; 
        
        float distance = Vector3.Distance(transform.position, lowerHalfInstance.transform.position);
        return distance < 1.5f;
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
    }

    public void ApplyDamage(float damage)
    {
        if (currentState != State.Immune && currentState != State.Regenerate)
        {
            health.TakeDamage(damage);
            Debug.Log("hitflash");
            hitFlash?.TriggerHitFlash();
        }
    }

    private void ApproachPlayer()
    {
        if (PlayerController.Instance.gameObject.activeSelf)
        {
            Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
            rb.velocity = new Vector2(
                direction.x * 2f,
                direction.y * 2f
            );
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void SpawnLowerHalf()
    {
        Vector2 spawnPos = RandomSpawnOutsideCamera();
        lowerHalfInstance = Instantiate(lowerHalfPrefab, spawnPos, Quaternion.identity);
    }

    public void MoveToLowerHalf()
    {
        if (lowerHalfInstance != null)
        {
            Vector3 direction = (GetLowerHalfPosition() - transform.position).normalized;
            rb.velocity = direction * 3f;

        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public bool isNearPlayer()
    {
        if (PlayerController.Instance == null) 
            return false; float distance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position); 
        return distance < 1.5f; 
    }

    public bool lowerHalfIsDestroyed()
    {
        return lowerHalfInstance == null || lowerHalfInstance.GetComponent<HealthComponent>().IsDead();
    }

    public void Die()
    {
        Destroy(gameObject);
        if (lowerHalfInstance != null)
        {
            Destroy(lowerHalfInstance);
        }
    }

    private Vector2 RandomSpawnOutsideCamera()
    {
        float edgeOffset = 1.1f;

        Vector2 spawnViewportPos;
        Vector2 spawnWorldPos;

        // coin flip (horizontal/vertical)
        if (Random.Range(0f, 1f) > 0.5f)
        {
            // left/right
            if (Random.Range(0f, 1f) > 0.5f)
            {
                spawnViewportPos = new Vector3(1 - edgeOffset, Random.value);
            }
            else
            {
                spawnViewportPos = new Vector3(edgeOffset, Random.value);
            }
        }
        else
        {
            // top/bottom
            if (Random.Range(0f, 1f) > 0.5f)
            {
                spawnViewportPos = new Vector3(Random.value, 1 - edgeOffset);
            }
            else
            {
                spawnViewportPos = new Vector3(Random.value, edgeOffset);
            }
        }

        spawnWorldPos = Camera.main.ViewportToWorldPoint(spawnViewportPos);
        return spawnWorldPos;
    }


    public void OnCollisionStay2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>(); 
        if (player != null)
        {
            player.ApplyDamage(10); 
        }
    }

    public Vector3 GetLowerHalfPosition()
    {
        if (lowerHalfInstance != null)
        {
            return lowerHalfInstance.transform.position;
        }
        else
        {
            return transform.position; 
        }
    }
}
