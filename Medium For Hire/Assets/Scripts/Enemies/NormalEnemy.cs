using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : BaseEnemy
{
        [Header("FOR ANIMATIONS")]
    [SerializeField] private Animator animator;

    protected override void Awake()
    {
        base.Awake(); 
    }

    protected override void OnEnable()
    {
        base.OnEnable(); 
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
}