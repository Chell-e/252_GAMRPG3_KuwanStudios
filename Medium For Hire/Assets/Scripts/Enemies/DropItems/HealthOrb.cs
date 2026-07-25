using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    public float hpToGive = 5f;

    [Header("Being Sucked FX")]
    public float moveSpeed = 11f;
    public float acceleration = 20f;

    private bool isBeingSucked = false;
    private bool isCollected = false;

    public Animator anim;
    private Rigidbody2D rb;
    private TrailRenderer trailRenderer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = rb.GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable()
    {
        isCollected = false;
        isBeingSucked = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
            anim.Play("Idle", 0, 0f);
        }
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (isBeingSucked && !isCollected && PlayerController.Instance != null)
        {
            Vector2 targetPos = PlayerController.Instance.transform.position;
            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;

            rb.velocity = Vector2.MoveTowards(rb.velocity, dir * moveSpeed, acceleration * Time.fixedDeltaTime);
        }
    }
    public void StartBeingSucked()
    {
        isBeingSucked = true;
        trailRenderer.emitting = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        // pickup range 
        if (!isBeingSucked && collision.GetComponent<PickupRange>())
        {
            StartBeingSucked();
            return;
        }

        // player 
        if (collision.GetComponent<PlayerController>())
        {
            isCollected = true;
            trailRenderer.emitting = false;

            PlayerController.Instance.GetComponent<HealthComponent>().Heal(hpToGive);
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHpUI();
            }

            PoolManager.ReturnObjectToPool(gameObject);
        }
    }

}
