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
    [SerializeField] private EnemyType enemyType = EnemyType.Normal;
    [SerializeField] public DropItem[] possibleDrops;

    [SerializeField] protected float attackDamage;
    [SerializeField] protected float moveSpeed;

    protected HealthComponent health;
    protected Rigidbody2D rb;
    private HitFlash hitFlash;

    private Coroutine damageRoutine;
    public bool isKnockedBack = false;
    public EnemyType EnemyType => enemyType;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
        hitFlash = GetComponent<HitFlash>();
    }

    protected virtual void Update()
    {
        LookAtPlayer();
        Move();
    }

    protected virtual void LookAtPlayer()
    {
        var playerPosition = PlayerController.Instance.transform.position;
        transform.rotation = playerPosition.x < transform.position.x ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 180f, 0f);
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameObject.activeInHierarchy || health == null || health.IsDead)
            return;

        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player)
        {
            damageRoutine = StartCoroutine(DamageTick(player));
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }

    private IEnumerator DamageTick(PlayerController player)
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
            if (player != null && !player.GetComponent<HealthComponent>().IsDead)
            {
                player.TakeDamage(attackDamage, this);
            }
            yield return new WaitForSeconds(0.7f); // -------> Adjust if necessary!
        }
    }

    // ====================== KNOCKBACK
    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        if (!gameObject.activeInHierarchy || health == null || health.IsDead)
            return;

        StartCoroutine(KnockbackRoutine(direction, force, duration));
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.05f);

        yield return new WaitForSeconds(duration);
        isKnockedBack = false;
    }
    // ====================== KNOCKBACK


    // ====================== DAMAGE
    public void TakeDamage(float damage)
    {
        health.TakeDamage(damage);
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
}
