using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteManananggal_LowerHalf : BaseEnemy
{
    [Header("Upper Half Reference")]
    public EliteManananggal_UpperHalf upperHalf;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (health != null)
        {
            health.OnDeath += HandleLowerHalfDeath;
        }
    }

    protected override void Update()
    {
        if (IsPlayerDead())
        {
            animator.enabled = false;
        }

        base.Update();
    }


    protected override void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= HandleLowerHalfDeath;
        }

        base.OnDisable();
    }

    private void HandleLowerHalfDeath()
    {
        if (upperHalf != null)
            upperHalf.IsLowerHalfDead();
    }

    public void LinkToUpperHalf(EliteManananggal_UpperHalf upper)
    {
        upperHalf = upper;
    }

    protected override void Move()
    {
        rb.velocity = Vector2.zero; 
    }
}