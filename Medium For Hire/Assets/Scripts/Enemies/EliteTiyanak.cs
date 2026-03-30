using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TiyanakState
{
    Approach,
    Lure,
    Transform
}
public class EliteTiyanak : BaseEnemy
{
    [Header("Tiyanak State")]
    public TiyanakState currentState;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Transformed Settings")]
    public float transformSpeed = 2f;
    public float healthHealOnTransform = 5f;
    public bool hasTransformed;

    [Header("Lure Settings")]
    public float influenceRange = 7f;
    public float intensity = 50f;
    private float distanceToPlayer;

    [Header("Trigger Distances")]
    public float lureDistance = 7f;
    public float transformDistance = 3f;

    // tiyanak speed cache
    private float originalBaseMoveSpeed;

    // player cache
    private Rigidbody2D playerRb;

    protected override void Awake()
    {
        base.Awake(); // Executes base's awake -> Gets rb, health component, player transform & hit flash

        originalBaseMoveSpeed = baseMoveSpeed;

        if (PlayerController.Instance != null)
        {
            playerRb = PlayerController.Instance.GetComponent<Rigidbody2D>();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable(); 
        
        hasTransformed = false;
        currentState = TiyanakState.Approach;
        rb.velocity = Vector2.zero;
        baseMoveSpeed = originalBaseMoveSpeed;

        if (animator != null)
            animator.SetBool("isCrying", false);   
    }

    protected override void Update()
    {
        if (health.IsDead) return;

        base.Update(); 

        // if transformed, stays in that state
        if (hasTransformed) return;

        UpdateTiyanakState();
        ExecuteCurrentState();
        //CheckDistanceToPlayer();
        //CheckState(currentState);
    }

    private void UpdateTiyanakState()
    {
        if (playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (dist <= transformDistance)  
        {
            currentState = TiyanakState.Transform;
        }
        else if (dist <= lureDistance) 
        {
            currentState = TiyanakState.Lure;
        }
        else
        {
            currentState = TiyanakState.Approach;
        }
    }

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case TiyanakState.Approach:
                animator.SetBool("isCrying", false);
                break;

            case TiyanakState.Lure:
                LurePlayer();
                animator.SetBool("isCrying", true);
                break;

            case TiyanakState.Transform:
                TransformTiyanak();
                animator.SetBool("isCrying", false);
                break;
        }
    }

    private void LurePlayer()
    {
        rb.velocity = Vector2.zero;

        if (playerRb == null || playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist <= influenceRange)
        {
            Vector2 pullForce = (transform.position - playerTransform.position).normalized / dist * intensity;
            playerRb.AddForce(pullForce, ForceMode2D.Force);
        }
    }

    private void TransformTiyanak()
    {
        health.Heal(healthHealOnTransform);
        baseMoveSpeed = transformSpeed;
        hasTransformed = true;
    }

    // ============ this visually checks the range
    private void OnDrawGizmosSelected()
    {
        // draws transform range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transformDistance);

        // draws lure range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lureDistance);
        Gizmos.DrawWireSphere(transform.position, transformDistance + 0.1f);
    }
}