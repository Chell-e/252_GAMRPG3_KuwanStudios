using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class MiniWeapon_Tirador : BaseWeapon
{

    [Header("BASE STATS")]
    //[SerializeField]
    private float baseCooldown = 6f; // same for either
    private float baseDamage = .5f;

    private float baseMaxBounces = 3;
    private float baseProjectileSpeed = 10f;


    [Header("EFFECTIVE STATS")]
    public float finalCooldown;
    public float finalDamage;

    public float finalMaxBounces;
    public float finalProjectileSpeed;


    [Header("DEBUG")]
    private float cooldownTimer = 99f;
    private float attackOffset = .2f;


    [Header("REFERENCES")]
    [SerializeField] public GameObject attackPrefab;

    // * DRIVER CODE
    // mainly Start() and Update()
    private void Start()
    {
        this.Initialize(PlayerController.Instance);
        Subscribe();

        DoScaleStats();
    }

    private void Update()
    {
        DoTick();
    }

    // * DRIVER CODE


    // *** CORE LOGIC
    private void DoTick() // to be called per update
    {
        // tick cooldown
        cooldownTimer += Time.deltaTime;


        if (cooldownTimer >= finalCooldown)
        {
            cooldownTimer = 0f;

            GameObject closestEnemy = DetectTiradorTarget();

            Vector3 offsetVector = GetOffsetDirection(closestEnemy.transform.position - this.transform.position);

            DoAttack(transform.position + offsetVector, closestEnemy.transform.position);

            // negate ammo loss if Survival Evolution
        }

    }

    private void DoScaleStats()
    {
        // DMG
        finalDamage =
            baseDamage
            * (playerStats.GetPlayerStat(Stat.DamagePercent) / 100f);

        // CDs
        finalCooldown =
            baseCooldown
            / (playerStats.GetPlayerStat(Stat.AttackSpeedPercent) / 100f);

        finalMaxBounces =
            Mathf.Ceil( // round up
                baseMaxBounces
                * (playerStats.GetPlayerStat(Stat.AreaPercent) / 100f)
            );

        finalProjectileSpeed =
            baseProjectileSpeed
            * (playerStats.GetPlayerStat(Stat.ProjectileSpeedPercent) / 100f) / 2f;

    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions

    private void DoAttack(Vector2 _originPos, Vector2 _targetPos)
    {
        Vector2 directionVector = (_targetPos - _originPos).normalized;

        GameObject attackInstance = Instantiate(attackPrefab, _originPos, Quaternion.identity);
        attackInstance.transform.right = directionVector;
        attackInstance.GetComponent<TiradorProjectile>().Initialize(this);

    }


    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)
    public override float GetFillProgress()
    {
        /*if (cooldownTimer <= 0)
            return reloadTimer / finalReloadTime;*/

        return cooldownTimer / finalCooldown;
    }

    public override string GetTooltipText()
    {
        string _name = "Centavo";
        string _cooldownTimer = (cooldownTimer).ToString("0.0");
        string _finalCooldown = (finalCooldown).ToString("0.0");

        string _maxBounces = (finalMaxBounces).ToString();

        string description =
            $"{_name}" +
            $"\nCooldown: {_cooldownTimer}/{_finalCooldown}s" +
            $"\nMax Bounces: {_maxBounces}";


        return description;
    }

    private Vector2 GetOffsetDirection(Vector2 _directionVector)
    {
        Vector2 directionOffset = _directionVector.normalized;
        directionOffset *= attackOffset;

        return directionOffset;
    }

    private GameObject DetectTiradorTarget()
    {
        GameObject target = null;

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, 10f);
        float closestDistance = Mathf.Infinity;

        foreach (var enemyHit in enemiesHit)
        {
            if (enemyHit.GetComponent<BaseEnemy>() == null) continue;


            BaseEnemy enemy = enemyHit.GetComponent<BaseEnemy>();

            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = enemy.gameObject;
            }
        }

        return target;
    }
    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here
    protected override void Subscribe()
    {
        playerEvents.OnAfterGetUpgrade += OnAfterGetUpgrade;
    }

    private void OnAfterGetUpgrade()
    {
        DoScaleStats();
    }
    // EVENTS & LISTENERS

}
