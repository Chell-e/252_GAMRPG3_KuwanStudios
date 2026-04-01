using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    [Header("Boulder Speed")]
    private Transform target;
    [SerializeField] private float moveSpeed = 20f;

    private float damage;

    private float currentLifetime = 0f;
    private float lifetime = 3f; // time before boulder is returned to pool

    private Vector3 move;

    private void OnEnable()
    {
        currentLifetime = lifetime;
    }

    private void Update()
    {
        if (target != null)
        {
            //Vector3 move = (target.position - transform.position).normalized;
            transform.position += move * moveSpeed * Time.deltaTime;
        }
        
        currentLifetime -= Time.deltaTime;

        if (currentLifetime <= 0f)
        {
            PoolManager.ReturnObjectToPool(gameObject);
        }
    }

    public void SetTarget(Transform targetTranform)
    {
        target = targetTranform;
        move = (target.position - transform.position).normalized;
    }

    public void SetDamage(float _damage)
    {
        damage = _damage;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        var player = col.gameObject.GetComponent<PlayerController>();
        if (player)
        {
            PoolManager.ReturnObjectToPool(gameObject);
            player.TakeDamage(damage);
        }
    }
}
