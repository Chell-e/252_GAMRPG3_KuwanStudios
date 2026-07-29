using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;

public class WhipAttack : MonoBehaviour
{
    public enum WhipAttackType
    {
        Sweep,
        Strike
    }

    [Header("DEBUG")]
    private float statusEffectPotency;
    private WhipAttackType attackType;
    private float facingDirectionX;

    [Header("REFERENCES")]
    private MainWeapon_BuntotPagi weapon; // get stats from this
    [SerializeField] private BaseStatusEffect statusEffect;
    

    //
    // * DRIVER CODE
    public void Initialize(MainWeapon_BuntotPagi _weapon, WhipAttackType _attackType, float _facingDirectionX = 0f)
    {
        this.weapon = _weapon;
        this.attackType = _attackType;

        facingDirectionX = _facingDirectionX;

        DoScaleStats();
    }

    void Start()
    {
        Destroy(gameObject, 5f);

        switch (attackType)
        {
            case WhipAttackType.Sweep:
                Sweep(this.transform.position);
                break;

            case WhipAttackType.Strike:
                Strike();
                break;

            default:
                break;
        }
    }
    // * DRIVER CODE
    //


    //
    // *** CORE LOGIC
    private void Sweep(Vector2 _position)
    {
        float sweepRadius = weapon.finalRadius;
        float sweepDamage = weapon.finalDamage * .1f; // damage should be very low

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll
        (
            _position,
            sweepRadius,
            LayerMask.GetMask("Enemy")
        );

        foreach (var enemyHit in enemiesHit)
        {
            BaseEnemy enemy = enemyHit.GetComponent<BaseEnemy>();

            // DEAL DMG
            PlayerController.Instance.DealDamage(sweepDamage, enemy);

            enemy.GetStatusEffectHandler().ApplyEffect(statusEffect, statusEffectPotency);
        }

        Instantiate(weapon.passiveVFX, weapon.transform)
        .Init(sweepRadius);

        if (weapon.isUtilityEvolved)
        {
            Collider2D[] pickupsHit = Physics2D.OverlapCircleAll
            (
                _position,
                sweepRadius,
                LayerMask.GetMask("PickupTrigger")
            );

            foreach (var pickupHit in pickupsHit)
            {
                if (pickupHit.GetComponent<ExpOrb>() != null)
                    pickupHit.GetComponent<ExpOrb>().StartBeingSucked();

                if (pickupHit.GetComponent<HealthOrb>() != null)
                    pickupHit.GetComponent<HealthOrb>().StartBeingSucked();

                if (pickupHit.GetComponent<TornPage>() != null)
                    pickupHit.GetComponent<TornPage>().StartBeingSucked();
            }
        }
    }

    private void Strike()
    {
        float strikeHeight = 0.75f + (weapon.finalRadius / 5f);
        float strikeLength = weapon.finalRadius * 2f;
        float strikeDamage = weapon.finalDamage * 2.0f;

        Vector2 topLeft = new Vector2(0, (strikeHeight / 2) ) * facingDirectionX;
        Vector2 lowRight = new Vector2(strikeLength, -(strikeHeight / 2) ) * facingDirectionX;

        topLeft = topLeft + (Vector2)weapon.transform.position;
        lowRight = lowRight + (Vector2)weapon.transform.position;


        Collider2D[] enemiesHit = Physics2D.OverlapAreaAll
        (
            topLeft,
            lowRight,
            LayerMask.GetMask("Enemy")
        );

        foreach (var enemyHit in enemiesHit)
        {
            BaseEnemy enemy = enemyHit.GetComponent<BaseEnemy>();

            // DEAL DMG
            PlayerController.Instance.DealDamage(strikeDamage, enemy);

            enemy.ApplyKnockback
            (
                enemy.transform.position - this.transform.position,
                weapon.finalKnockbackPower,
                0.5f
            );

        }

        Vector2 heightAndLength = new Vector2 (strikeLength, strikeHeight);
        Vector2 offsetCenter = new Vector2
            ((strikeLength / 2) * facingDirectionX,
            0);

        WhipStrikeVFX vfx = Instantiate(weapon.aimedVFX, this.transform.position, Quaternion.identity);
        vfx.Init(heightAndLength, offsetCenter);

        if (weapon.isUtilityEvolved)
        {
            Collider2D[] pickupsHit = Physics2D.OverlapAreaAll
            (
                topLeft,
                lowRight,
                LayerMask.GetMask("PickupTrigger")
            );

            foreach (var pickupHit in pickupsHit)
            {
                if (pickupHit.GetComponent<ExpOrb>() != null)
                    pickupHit.GetComponent<ExpOrb>().StartBeingSucked();

                if (pickupHit.GetComponent<HealthOrb>() != null)
                    pickupHit.GetComponent<HealthOrb>().StartBeingSucked();

                if (pickupHit.GetComponent<TornPage>() != null)
                    pickupHit.GetComponent<TornPage>().StartBeingSucked();
            }
        }
    }

    private void DoScaleStats()
    {
        statusEffectPotency = 1.0f + (weapon.finalKnockbackPower - 0.5f);
    }
    // *** CORE LOGIC
    //


    //
    // ** SUB FUNCTIONS

    // ** SUB FUNCTIONS


    //
    // TOOLS

    // TOOLS
    //


    //
    // EVENTS & LISTENERS

    // EVENTS & LISTENERS
    //



}
