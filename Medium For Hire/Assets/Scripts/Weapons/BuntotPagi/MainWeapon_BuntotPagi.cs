using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using static WhipAttack;

public class MainWeapon_BuntotPagi: BaseWeapon
{
    public enum BuntotPagiEvolutions
    {
        Base,
        Twinned,
        Whirlwind,
        Collector
    }

    [Header("BASE STATS")]
    //[SerializeField]
    private float baseCooldown = 2f; // same for either
    private float baseDamage = 1f;
    private float baseKnockbackPower = 0.5f;
    private float baseRadius = 1.5f;

    [Header("EFFECTIVE STATS")]
    public float finalCooldown;
    public float finalDamage;
    public float finalKnockbackPower;
    public float finalRadius; // should scale with area

    [Header("DEBUG")]
    private float cooldownTimer = 99f;
    private bool isAimed = false;

    //==========
    [Header("EVOLUTIONS")]
        [Header("MAYARI EVO")]
    public bool isOffenseEvolved = false;

        [Header("TALA EVO")]
    public bool isSurvivalEvolved = false;

        [Header("HANAN EVO")]
    public bool isUtilityEvolved = false;

    //==========

    [Header("REFERENCES")]
    [SerializeField] public GameObject attackPrefab;
    [Space]
    [SerializeField] public WhipSweepVFX passiveVFX;
    [SerializeField] public WhipStrikeVFX aimedVFX;


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
        //isAimed = PlayerStats.Instance.isAiming ? true : false;
    }

    // * DRIVER CODE


    // *** CORE LOGIC
    private void DoTick() // to be called per update
    {
        // increment cooldownTimer until it hits finalCooldown threshold
        cooldownTimer += Time.deltaTime;
        
        if (cooldownTimer >= finalCooldown)
        {
            cooldownTimer = 0f;

            if (isAimed)
                DoAimedAttack(transform.position);
            else
                DoPassiveAttack(transform.position);
        }

    }

    [ContextMenu("Update Stats")]
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


        // AOE & KB
        finalKnockbackPower =
            baseKnockbackPower
            * (playerStats.GetPlayerStat(Stat.ProjectileSpeedPercent) / 100f);

        finalRadius =
            baseRadius
            * (playerStats.GetPlayerStat(Stat.AreaPercent) / 100f);


        // GRUDGE
        if (isOffenseEvolved)
        {
            finalCooldown = finalCooldown / 2f;
        }

        // GUARD
        if (isSurvivalEvolved)
        {
            finalKnockbackPower = finalKnockbackPower * 5f;
        }

        // GUIDE
        if (isUtilityEvolved)
        {
            finalRadius = finalRadius * 1.5f;
        }
    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions
    private void DoPassiveAttack(Vector2 _position)
    {
        GameObject passiveAttack = Instantiate(attackPrefab, _position, Quaternion.identity);
        passiveAttack.GetComponent<WhipAttack>().Initialize(this, WhipAttackType.Sweep);
    }

    private void DoAimedAttack(Vector2 _position)
    {
        GameObject aimedAttack = Instantiate(attackPrefab, _position, Quaternion.identity);
        aimedAttack.GetComponent<WhipAttack>().Initialize
            (this,
                WhipAttackType.Strike,
                playerController.GetLastFacingDirectionX().x
            );

        // offense evolution: cast in opposite direction as well
        if (isOffenseEvolved)
        {
            GameObject oppositeAttack = Instantiate(attackPrefab, _position, Quaternion.identity);

            oppositeAttack.GetComponent<WhipAttack>().Initialize
            (this,
                WhipAttackType.Strike,
                playerController.GetLastFacingDirectionX().x * -1f
            );
        }
        
        /*if (playerController.GetLastFacingDirectionX().x > 0)
        {
            aimedAttack.GetComponent<WhipAttack>().Initialize
            (this,
                WhipAttackType.Strike,
                playerController.GetLastFacingDirectionX().x
            );
        }
        else
        {
            aimedAttack.GetComponent<WhipAttack>().Initialize
            (this,
                WhipAttackType.Strike,
                playerController.GetLastFacingDirectionX().x
            );
        }*/

    }
    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)
    public override float GetFillProgress()
    {
        return cooldownTimer / finalCooldown;
    }

    public override string GetTooltipText()
    {
        string name = "Buntot Pagi";
        string _cooldownTimer = (cooldownTimer).ToString("0.00");
        string _finalCooldown = (finalCooldown).ToString("0.00");

        string description =
            $"{name}" +
            $"\nCooldown: {_cooldownTimer}/{_finalCooldown}s";


        return description;
    }


    public override void EvolveOffense()
    {
        isOffenseEvolved = true;
        DoScaleStats();
    }
    public override void EvolveSurvival()
    {
        isSurvivalEvolved = true;
        DoScaleStats();
    }
    public override void EvolveUtility()
    {
        isUtilityEvolved = true;
        DoScaleStats();
    }

    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here
    protected override void Subscribe()
    {
        playerEvents.OnAimToggle += OnAimToggle;
        playerEvents.OnAfterGetUpgrade += OnAfterGetUpgrade;
    }

    private void OnAimToggle(AimContext aimContext)
    {
        isAimed = !isAimed;
    }

    private void OnAfterGetUpgrade()
    {
        DoScaleStats();
        Debug.Log("AFTER GOT UPGRADE HEARD!");
    }
    // EVENTS & LISTENERS

}
