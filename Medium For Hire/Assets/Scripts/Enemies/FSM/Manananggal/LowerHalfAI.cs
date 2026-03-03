using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerHalfAI : MonoBehaviour, IDamageable
{
    [Header("References")]
    public Rigidbody2D rb;
    public HealthComponent health;
    private HitFlash hitFlash;

    void Awake()
    {
        rb.velocity = Vector2.zero;
        health = GetComponent<HealthComponent>();
        hitFlash = GetComponent<HitFlash>();
    }

    void Update()
    {
        if (health.IsDead())
        {
            Destroy(gameObject);
        }
    }

    public void ApplyDamage(float damage)
    {
        health.TakeDamage(damage);
        hitFlash?.TriggerHitFlash();
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ApplyDamage(5); 
        }
    }
}
