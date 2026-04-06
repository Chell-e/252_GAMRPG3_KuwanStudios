using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
//using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MiniWeapon_HolyWater : BaseWeapon
{
        [Header("Base Weapon Stats")]
            [Tooltip("Base cooldown between each splash.")]
    public float baseCooldownTime = 5f;
            [Tooltip("StatusEffect applied.")]
    [SerializeField] BaseStatusEffect holyWaterStatus; // vulnerable
    // add: dmg mult

        [Header("Effective Weapon Stats")]
            [Tooltip("Effective CD between each splash.")]
    [SerializeField] private float cooldownTime;


        [Header("Runtime Stats")]
            [Tooltip("CD timer.")]
    [SerializeField] private float cooldownTimer;


        [Header("References")]
            [Tooltip("Holy Water melee splash object.")]
    [SerializeField] SpriteRenderer holyWaterVfx;
    [SerializeField] CircleCollider2D holyWaterHitbox;


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


    private void AimHolyWater()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = transform.position.z;

        Vector3 direction = mouseWorld - transform.position;

        // Use LookAt, but constrain it to 2D
        this.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    private void SplashHolyWater()
    {
        // ***** make this an animation instead
        holyWaterVfx.GetComponent<SpriteRenderer>().enabled = true;
        // *****

        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(holyWaterVfx.transform.position, this.GetComponent<CircleCollider2D>().radius);

        foreach (var enemyHit in targetsHit)
        {
            if (enemyHit.GetComponent<BaseEnemy>() != null)
            {
                BaseEnemy enemy = enemyHit.GetComponent<BaseEnemy>();
                enemy.GetStatusEffectHandler().ApplyEffect(holyWaterStatus, 0f);
                PlayerController.Instance.DealDamage(1, enemy);

                Debug.Log("holyWater hit " + enemy);
            }
        }
    }

    private void HolyWaterRechargeLogic()
    {
        if (cooldownTimer >= cooldownTime)
        {
            Debug.Log("HolyWater swing");
            AimHolyWater();
            SplashHolyWater();

            cooldownTimer = 0;
        }

        cooldownTimer += Time.deltaTime;

    }




    public override float GetFillProgress()
    {
        return cooldownTimer / cooldownTime;
    }

    public override string GetTooltipText()
    {
        string name = "Holy Water";

        string description =
            $"Name: \"{name}\"" +
            $"\nStatus Duration: {holyWaterStatus.lifetime}s" +
            $"\nCooldown: {(int)cooldownTimer}/{cooldownTime}s"
            ;

        return description;

    }

    public override string GetName()
    {
        return "Holy Water";
    }

    public override string GetDescription()
    {
        return "Sprinkles holy water towards the cursor. Holy Water makes Enemies take more Damage.";
    }
}
