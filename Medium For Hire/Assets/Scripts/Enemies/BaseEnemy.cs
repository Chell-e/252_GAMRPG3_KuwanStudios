using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public enum EnemyType
{
    Normal,
    Elite,
    Boss
}

public abstract class BaseEnemy : MonoBehaviour
{
        [Header("Enemy Stats")]
    [SerializeField] private EnemyType enemyType = EnemyType.Normal;
    [SerializeField] public DropItem[] possibleDrops;

    [SerializeField] protected float attackDamage;
    [SerializeField] protected float moveSpeed;


        [Header("Runtime Stats")]
    //
    [SerializeField] List<BaseStatusEffect> activeStatusEffects = new List<BaseStatusEffect>();
    [SerializeField] private float moveSpeedMultiplier = 1.0f;
    [SerializeField] private float incomingDamageMultiplier = 1.0f;
    //

    public bool isKnockedBack = false;
    private Coroutine damageRoutine;


    protected HealthComponent health;
    protected Rigidbody2D rb;
    private HitFlash hitFlash;
    
    public EnemyType EnemyType => enemyType;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
        hitFlash = GetComponent<HitFlash>();
    }

    protected virtual void Update()
    {
        UpdateStatusEffects(Time.deltaTime);

        LookAtPlayer();
        Move();
    }

    protected virtual void LookAtPlayer()
    {
        var playerPosition = PlayerController.Instance.transform.position;
        transform.rotation = playerPosition.x < transform.position.x ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 180f, 0f);
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D _collision)
    {
        if (!gameObject.activeInHierarchy || health == null || health.IsDead)
            return;

        var player = _collision.gameObject.GetComponent<PlayerController>();
        if (player)
        {
            damageRoutine = StartCoroutine(DamageTick(player));
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D _collision)
    {
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }

    private IEnumerator DamageTick(PlayerController _player)
    {
        // Don't access
        /*var playerHealth = player.GetComponent<HealthComponent>();
        while (true)
        {
            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.TakeDamage(attackDamage);
            }
            yield return new WaitForSeconds(0.7f); // -------> Adjust if necessary!
        }*/

        while (true)
        {
            if (_player != null && !_player.GetComponent<HealthComponent>().IsDead)
            {
                _player.TakeDamage(attackDamage, this);
            }
            yield return new WaitForSeconds(0.7f); // -------> Adjust if necessary!
        }
    }

    // ====================== KNOCKBACK
    public void ApplyKnockback(Vector2 _direction, float _force, float _duration)
    {
        if (!gameObject.activeInHierarchy || health == null || health.IsDead)
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
        
        health.TakeDamage(finalDamage);
    }
    // ====================== DAMAGE


    protected virtual void Move()
    {
        if (isKnockedBack) return; 
    }

    protected virtual void OnDisable()
    {
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
        StopAllCoroutines();
        isKnockedBack = false;
        rb.velocity = Vector2.zero;
    }


    // ====================== STATUS EFFECTS
    public void SetMoveSpeedMultiplier(float _moveSpeedMultiplier)
    {
        moveSpeedMultiplier = _moveSpeedMultiplier; 
    }
    public void SetIncomingDamageMultiplier(float _incomingDamageMultiplier)
    {
        incomingDamageMultiplier = _incomingDamageMultiplier;
    }

    public void ApplyStatusEffect(BaseStatusEffect _newEffect)
    {
        foreach (var activeEffect in activeStatusEffects)
        {
            if (_newEffect.GetType() == activeEffect.GetType())  // check if status effect already exists
            {
                activeEffect.Refresh();
                return;
            }
        }

        // otherwise, initialize the new effect and register it for the enemy
        _newEffect.OnApply(this);
        activeStatusEffects.Add(_newEffect);
    }
    private void UpdateStatusEffects(float _timeElapsed)
    {
        // ENEMY is responsible for driving status effect logic

        foreach (var activeEffect in activeStatusEffects) // loop thru each active status effect
        {
            activeEffect.TickEffect(this, _timeElapsed); 

            if (activeEffect.IsFinished())
            {
                activeEffect.OnExpire(this);
                activeStatusEffects.Remove(activeEffect);
            }
        }
    }

    // ====================== STATUS EFFECTS


}
