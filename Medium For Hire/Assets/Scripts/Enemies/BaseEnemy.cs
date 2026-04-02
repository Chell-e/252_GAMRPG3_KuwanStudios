using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
        [Header("Enemy Stats")]
    [SerializeField] public DropItem[] possibleDrops;

    [SerializeField] protected float attackDamage = 1f;
    [SerializeField] protected float baseMoveSpeed = 1f;

    // scaled damage when spawning
    [SerializeField] private float currentScaledDamage;

    public float AttackDamage { get; set; }
    public float BaseMoveSpeed { get; set; }

    // new
    [Header("Runtime Stats")]
    [SerializeField] private float moveSpeedMultiplier = 1.0f;
    [SerializeField] private float incomingDamageMultiplier = 1.0f;

    // new
    private StatusEffectHandlerComponent statusEffectHandler;

    protected HealthComponent health;
    protected Rigidbody2D rb;
    protected HitFlash hitFlash;

    public bool isKnockedBack = false;
    private Coroutine damageRoutine;
    public float damageTick = 0.7f;

    // player references
    protected PlayerController playerController;
    protected Transform playerTransform;
    protected HealthComponent playerHealth;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
        hitFlash = GetComponent<HitFlash>();

        // new
        statusEffectHandler = GetComponent<StatusEffectHandlerComponent>();

        // new!
        if (PlayerController.Instance != null)
        {
            playerController = PlayerController.Instance;
            playerTransform = PlayerController.Instance.transform;
            playerHealth = PlayerController.Instance.GetComponent<HealthComponent>();
        }
    }

    // IMPORTANT for pooling: reset these once they get spawned from pool
    protected virtual void OnEnable()
    {
        moveSpeedMultiplier = 1.0f;
        incomingDamageMultiplier = 1.0f;

        currentScaledDamage = attackDamage;

        if (health != null)
            health.ResetHealth();
    }

    public virtual void ScaleEnemyStat(float statMultiplier)
    {
        if (health != null)
        {
            health.ApplyHealthMultiplier(statMultiplier);
        }

        currentScaledDamage = attackDamage * statMultiplier;
    }

    protected virtual void Update()
    {
        // don't do anything if dead OR player is null
        if (health.IsDead || IsPlayerDead())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        LookAtTarget();
        Move();
    }

    public bool IsPlayerDead()
    {
        return playerHealth == null || playerHealth.IsDead;
    }

    protected virtual void LookAtTarget()
    {
        // don't look if player is dead or player tranform is null
        if (playerTransform == null) return;

        bool facingPlayer = playerTransform.position.x < transform.position.x;
        transform.rotation = facingPlayer
            ? Quaternion.Euler(0f, 0f, 0f) 
            : Quaternion.Euler(0f, 180f, 0f);
    }

    protected virtual void Move()
    {
        if (isKnockedBack || playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = direction * baseMoveSpeed * moveSpeedMultiplier;
    }

    protected virtual void OnCollisionEnter2D(Collision2D _collision)
    {
        if (health.IsDead) return;

        var player = _collision.gameObject.GetComponent<PlayerController>();
        if (player)
        {
            if (damageRoutine == null) 
                damageRoutine = StartCoroutine(DamageTick(player));
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D _collision)
    {
        if (_collision.gameObject.GetComponent<PlayerController>() && damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }

    private IEnumerator DamageTick(PlayerController _player)
    {
        while (true)
        {
            if (_player != null && !_player.GetComponent<HealthComponent>().IsDead)
            {
                _player.TakeDamage(currentScaledDamage, this);
            }
            
            yield return new WaitForSeconds(damageTick);
        }
    }

    // ====================== KNOCKBACK
    public void ApplyKnockback(Vector2 _direction, float _force, float _duration)
    {
        if (health.IsDead)
            return;

        StartCoroutine(KnockbackRoutine(_direction, _force, _duration));
    }

    private IEnumerator KnockbackRoutine(Vector2 _direction, float _force, float _duration)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(_direction * _force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.05f);

        yield return new WaitForSeconds(_duration);
        isKnockedBack = false;
    }
    // ====================== KNOCKBACK


    // ====================== DAMAGE
    public void TakeDamage(float _damage)
    {
        float finalDamage = _damage * incomingDamageMultiplier;
        
        //health.TakeDamage(finalDamage);

        health.ReduceHealth(finalDamage);

        if (hitFlash != null)
            hitFlash.TriggerHitFlash();
    }
    // ====================== DAMAGE


    // IMPORTANT for pooling: these reset BEFORE they return to pool
    protected virtual void OnDisable() 
    {
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }

        // stop movement immediately when disabled and reset knockback state
        rb.velocity = Vector2.zero;
        isKnockedBack = false;

        // reset hitflash BEFORE it returns to pool
        if (hitFlash != null)
        {
            hitFlash.ResetFlash();
        }
    }


    // ====================== STATUS EFFECTS
    public StatusEffectHandlerComponent GetStatusEffectHandler()
    {
        return statusEffectHandler;
    }

    public void SetIncomingDamageModifier(float _newIncomingDamageModifier)
    {
        this.incomingDamageMultiplier = _newIncomingDamageModifier;
    }

    public void SetMoveSpeedModifier(float _newMoveSpeedModifier)
    {
        this.moveSpeedMultiplier = _newMoveSpeedModifier;
    }
    // ====================== STATUS EFFECTS
}
