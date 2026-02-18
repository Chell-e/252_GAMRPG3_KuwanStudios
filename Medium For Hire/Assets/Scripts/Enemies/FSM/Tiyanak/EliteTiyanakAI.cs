using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EliteTiyanakState
{
    Approach,
    Lure,
    Transform,
    Dead
}
public class EliteTiyanakAI : MonoBehaviour, IDamageable
{
    public EliteTiyanakState currentState;

    [Header("References")]
    public Transform playerTransform;
    public Rigidbody2D rbPlayer;
    public Rigidbody2D rb;
    public HealthComponent health;
    private HitFlash hitFlash;

    [Header("Settings")]
    public float approachSpeed;
    public float transformSpeed;
    public bool hasTransformed = false;

    [Header("BlackHole Settings")] // aka lure
    public float influenceRange;
    public float intensity;
    public float distanceToPlayer;
    Vector2 pullForce;

    [Header("Colliders")]
    public Collider2D lureCollider;
    public Collider2D transformCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
        hitFlash = GetComponent<HitFlash>();
        rbPlayer = PlayerController.Instance.GetComponent<Rigidbody2D>();

        currentState = EliteTiyanakState.Approach;
    }

    void Update()
    {
        // check if player's alive grr, tas tama na pls >;(
        if (PlayerController.Instance == null || !PlayerController.Instance.gameObject.activeSelf)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        switch (currentState)
        {
            case EliteTiyanakState.Approach:
                ApproachPlayer();
                if (health.IsDead())
                {
                    ChangeState(EliteTiyanakState.Dead);
                }
                break;
            case EliteTiyanakState.Transform:
                TransformTiyanak();
                if (health.IsDead())
                {
                    ChangeState(EliteTiyanakState.Dead);
                }
                break;
            case EliteTiyanakState.Lure:
                LurePlayer();

                if (health.IsDead())
                {
                    ChangeState(EliteTiyanakState.Dead);
                }
                break;
            case EliteTiyanakState.Dead:
                Die();
                break;
        }
    }

    public void ChangeState(EliteTiyanakState newState)
    {
        currentState = newState;
    }

    public void ApproachPlayer()
    {
        Vector2 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
        rb.velocity = direction * approachSpeed;
    }

    public void LurePlayer()
    {
        // stop tiyanak movement
        rb.velocity = Vector2.zero;

        // black hole 
        var playerPosition = PlayerController.Instance.transform.position;

        distanceToPlayer = Vector2.Distance(transform.position, playerPosition);
        if (distanceToPlayer <= influenceRange)
        {
            pullForce = (transform.position - playerPosition).normalized / distanceToPlayer * intensity;
            rbPlayer.AddForce(pullForce, ForceMode2D.Force);
        }
    }

    public void TransformTiyanak()
    {
        var playerPosition = PlayerController.Instance.transform.position;

        Vector2 direction = (playerPosition - transform.position).normalized;
        rb.velocity = direction * transformSpeed;
    }

    public void ApplyDamage(float damage)
    {
        GetComponent<HealthComponent>().TakeDamage(damage);
        hitFlash?.TriggerHitFlash();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ApplyDamage(10);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        // if it has not transformed yet and the player enters the lure zone, LURE
        if (!hasTransformed && lureCollider.IsTouching(other))
        {
            ChangeState(EliteTiyanakState.Lure);
        }

        // if the player enters the transform zone, TRANSFORM permanently
        if (transformCollider.IsTouching(other))
        {
            hasTransformed = true;
            ChangeState(EliteTiyanakState.Transform);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        // if it has not transformed yet and the player exits the lure zone, APPROACH
        if (!hasTransformed && !lureCollider.IsTouching(other))
        {
            ChangeState(EliteTiyanakState.Approach);
        }
    }
}
