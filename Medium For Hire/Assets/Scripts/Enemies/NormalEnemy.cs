using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : BaseEnemy
{
    protected override void Awake()
    {
        base.Awake(); // Executes base's awake -> Gets rb & health component
    }

    protected override void Move()
    {
        if (isKnockedBack) return;

        Transform player = PlayerController.Instance.transform;
        Vector3 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }
}