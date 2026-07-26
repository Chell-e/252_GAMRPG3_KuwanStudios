using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornPage : MonoBehaviour
{
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
    }

    [Header("Being Sucked FX")]
    public float moveSpeed = 11f;
    public float acceleration = 20f;

    private bool isBeingSucked = false;
    private bool isCollected = false;

    //public Animator anim;
    private Rigidbody2D rb;
    private TrailRenderer trailRenderer;

    // * DRIVER CODE
    // mainly Start() and Update()
    private void FixedUpdate()
    {
        if (isBeingSucked && !isCollected && PlayerController.Instance != null)
        {
            Vector2 targetPos = PlayerController.Instance.transform.position;
            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;

            rb.velocity = Vector2.MoveTowards(rb.velocity, dir * moveSpeed, acceleration * Time.fixedDeltaTime);
        }
    }
    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below

    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions
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

            StageManager.Instance.RegisterTornPages();
            PoolManager.ReturnObjectToPool(gameObject);
        }
    }

    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)

    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here

    // EVENTS & LISTENERS
}
