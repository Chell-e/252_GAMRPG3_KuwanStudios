using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEditor.PackageManager;
using UnityEngine;

public class MiniWeapon_HolyWater : BaseWeapon
{
        [Header("Base Weapon Stats")]
            [Tooltip("Base cooldown between each splash.")]
    public float baseCooldownTime = 5f;
    // add: dmg mult

        [Header("Effective Weapon Stats")]
            [Tooltip("Effective recharge CD per Charge.")]
    [SerializeField] private float cooldownTime;


        [Header("Runtime Stats")]
            [Tooltip("Recharge timer.")]
    [SerializeField] private float cooldownTimer;


        [Header("References")]
            [Tooltip("Holy Water melee hitbox.")]
    [SerializeField] GameObject holyWaterSplashHitbox;


    protected override void Subscribe()
    {
    }

    protected override void Unsubscribe()
    {
    }

    private void Start()
    {
        Subscribe();

        cooldownTime = baseCooldownTime;
        cooldownTimer = 0;

    }

    private void Update()
    {

        // for now, let's put the stat scaling in Update()


        HolyWaterRechargeLogic();
    }


    private void HolyWaterRechargeLogic()
    {
        if (cooldownTimer >= cooldownTime)
        {
            Debug.Log("HolyWater swing");

            cooldownTimer = 0;
        }

        cooldownTimer += Time.deltaTime;


    }

    private void SplashHolyWater()
    {
        CircleCollider2D[] hitboxes;
        hitboxes = holyWaterSplashHitbox.GetComponentsInChildren<CircleCollider2D>(); // dont use this?
    }

    private void AimHolyWater()
    {
        
    }

}
