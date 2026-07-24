using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExpOrb : MonoBehaviour
{
    // ==== EVENTS ====

        // static --> accessible globally
        // Action --> delegate method returns void
            // can use generic Action<T> to pass data with the event

    public static event Action OnExpOrbExpire;    

    // ==== EVENTS ====

    public int expToGive;
    public float expireTime = 30f;
    public float decayStartsTime = 5f;

    private float spawnTimeElapsed = 0f;

    [Header("Being Sucked FX")]
    public float moveSpeed = 11f;
    public float acceleration = 20f;

    private bool isDecaying = false;
    private bool isCollected = false;
    private bool isBeingSucked = false;

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
        // track how much time has passed since it spawned
        // Time.timeSinceLevelLoad --> pauses when game is paused, so orbs don't expire when on upgrade screen

        spawnTimeElapsed = Time.timeSinceLevelLoad;
        isDecaying = false;
        isCollected = false;
        isBeingSucked= false;
        trailRenderer.Clear();

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
        // stop expiring when player's dead
        if (StageManager.Instance != null && StageManager.Instance.isGameOver) return;
        if (isCollected) return;

        if (!isBeingSucked)
        {
            float timePassed = Time.timeSinceLevelLoad - spawnTimeElapsed;

            if (!isDecaying && timePassed >= (expireTime - decayStartsTime))
            {
                StartDecay();
            }

            if (timePassed >= expireTime)
            {
                ExpireOrb();
            }
        }
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

    private void StartDecay()
    {
        isDecaying = true;

        if (anim != null)
        {
            anim.Play("Decaying");
        }
    }

    public void StartBeingSucked()
    {
        isBeingSucked = true;
        trailRenderer.emitting = true;
    }

    private void ExpireOrb()
    {
        // EVENT
        OnExpOrbExpire?.Invoke();

        if (!isCollected)
        {
            PoolManager.ReturnObjectToPool(gameObject);
        }
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

            PlayerController.Instance.playerStats.GainExperience(expToGive);
            PoolManager.ReturnObjectToPool(gameObject);
        }

    }

    public void Initialize(int xpValue)
    {
        expToGive = xpValue;
    }
}
