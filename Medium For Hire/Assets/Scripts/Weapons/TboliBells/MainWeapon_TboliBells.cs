using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class MainWeapon_TboliBells : BaseWeapon
{
    public enum TboliBellsEvolution
    {
        Base,
        Linger,
        Duplicate,
        Barrage 
    }

        [Header("Base Weapon Stats")]
    //[SerializeField]
    private float basePassiveCooldownTime = 4f; //4s for passive ringing
    private float baseActiveCooldownTime = 5f; //5s for activated ringing

    private float baseDamage = 1f;
    private float baseKnockbackPower = 0.5f;
    private float baseRadius = 1.5f;

        [Header("Effective Weapon Stats")]
    public float finalDamage;

    public float passiveCooldownTime; // should scale with atkspeed
    public float activeCooldownTime; // should scale w/ atkspeed

    public float finalKnockbackPower;
    public float finalRadius; // should scale with area

        [Header("Runtime Stats")]
    private bool isAimed = false;

    private float passiveCooldownTimer = 99f;
    private float activeCooldownTimer = 99f;

    //==========
        [Header("Evolutions")]
        [Header("Grudge Evolution")]

    public bool isGrudgeEvolved = false;
    //[SerializeField] public BaseStatusEffect tboliBellsStatus;
    private float baseDamageStatus = 1.1f;
    public float finalDamageStatus;

        [Header("Guard Evolution")]
    public bool isGuardEvolved = false;
    private int baseExtraRings = 1;
    public int finalExtraRings;

        [Header("Guide Evolution")]
    public bool isGuideEvolved = false;
    private int baseMaxChains = 1;
    public int finalMaxChains;

    //==========

        [Header("References")]
    [SerializeField] public GameObject attackPrefab;
    [SerializeField] public TboliVFX vfxPrefab;
    //private HashSet<BaseEnemy> hitThisPulse = new HashSet<BaseEnemy>();

    //Whether a ring applies a status effect.
    //How many "rings" a ring generates.
    //Whether a ring "chains".

    private void Start()
    {
        this.Initialize(PlayerController.Instance);
        Subscribe();

        DoScaleStats();
    }

    protected override void Subscribe()
    {
        playerEvents.OnAimToggle += OnAimToggle;
        playerEvents.OnAfterGetUpgrade += OnAfterGetUpgrade;
    }

    private void OnAimToggle()
    {
        isAimed = !isAimed;

        Debug.Log("tboli aim toggled");
        if (activeCooldownTimer >= activeCooldownTime)
        {
            activeCooldownTimer = 0;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mouseWorldPos = new Vector3(mousePos.x, mousePos.y, 0);
            DoRingAttack(mouseWorldPos); // toggle
        }
    }

    private void OnAfterGetUpgrade()
    {
        DoScaleStats();
    }




    private void Update()
    {
        DoTick();
        //isAimed = PlayerStats.Instance.isAiming ? true : false;
    }

    private void DoScaleStats()
    {
        // DMG
        finalDamage =
            baseDamage
            * (playerStats.GetPlayerStat(Stat.DamagePercent) / 100f);

        // CDs
        passiveCooldownTime =
            basePassiveCooldownTime
            / (playerStats.GetPlayerStat(Stat.AttackSpeedPercent) / 100f);

        activeCooldownTime =
            baseActiveCooldownTime
            / (playerStats.GetPlayerStat(Stat.AttackSpeedPercent) / 100f);


        // AOE & KB
        finalKnockbackPower =
            baseKnockbackPower
            * (playerStats.GetPlayerStat(Stat.ProjectileSpeedPercent) / 100f);

        finalRadius =
            baseRadius
            * (playerStats.GetPlayerStat(Stat.AreaPercent) / 100f);


        // GRUDGE
        if (isGrudgeEvolved)
        {
            finalDamage =
            (baseDamage
            + (playerStats.GetPlayerStat(Stat.DomainOffense) / 3)
                )

            * (playerStats.GetPlayerStat(Stat.DamagePercent) / 100f);

            finalDamageStatus =
                baseDamageStatus
                + (playerStats.GetPlayerStat(Stat.DomainOffense) * .05f); // +5% incoming dmg per Grudge
        }

        // GUARD
        if (isGuardEvolved)
        {
            finalExtraRings =
                baseExtraRings
                + (int)( ((playerStats.GetPlayerStat(Stat.DomainSurvival) - 10) / 5f) );
        }

        // GUIDE
        if (isGuideEvolved)
        {
            finalMaxChains =
                baseMaxChains
                + (int)( ((playerStats.GetPlayerStat(Stat.DomainUtility) - 10) / 5f));
        }
    }

    private void DoTick() // to be called per update
    {
        if (activeCooldownTimer < activeCooldownTime)
            activeCooldownTimer += Time.deltaTime;

        // 

        passiveCooldownTimer += Time.deltaTime;
        if (isAimed) passiveCooldownTimer += Time.deltaTime; // passive goes down twice as fast if aimed

        if (passiveCooldownTimer >= passiveCooldownTime)
        {
            passiveCooldownTimer = 0f;
            DoRingAttack(this.transform.position);
        }
    }

    

    private void DoRingAttack(Vector2 _position)
    {
        // note for:
        // - radius
        // - damage
        // - knockback
        // 
        // - ring visual
        // 
        // - number of "extra rings"
        // - number of max chains
        // - already hit enemies

        GameObject attack = Instantiate(attackPrefab, _position, Quaternion.identity);
        attack.GetComponent<TboliAttackInstance>().Initialize(this);
        
    }



    // TOOLTIPS
    public override float GetFillProgress()
    {
        return activeCooldownTimer / activeCooldownTime;
    }

    public override string GetTooltipText()
    {
        string name = "T'boli Bells";
        string _passiveCooldownTimer = (passiveCooldownTimer).ToString("0.00");
        string _passiveCooldownTime = (passiveCooldownTime).ToString("0.00");

        string description =
            $"{name}";

        description +=
            $"\nAutomatic Blast: {(int)passiveCooldownTimer}/{(int)passiveCooldownTime}s";

        description +=
            isGrudgeEvolved
            ? $"\n<style=\"grudge\">+Base DMG <sprite name=\"grudge\">: {(playerStats.GetPlayerStat(Stat.DomainOffense) / 3)}</style>"
            : "";

        description +=
            isGuardEvolved
            ? $"\n<style=\"guard\">Echoes <sprite name=\"guard\">: {finalExtraRings}</style>"
            : "";

        description +=
            isGuideEvolved
            ? $"\n<style=\"guide\">Chains <sprite name=\"guide\">: {finalMaxChains}</style>"
            : "";


        return description;
    }
    // TOOLTIPS


    // EVOLUTION
    public override void EvolveOffense()
    {
        isGrudgeEvolved = true;
        DoScaleStats();
    }
    public override void EvolveSurvival()
    {
        isGuardEvolved = true;
        DoScaleStats();
    }
    public override void EvolveUtility()
    {
        isGuideEvolved = true;
        DoScaleStats();
    }
    // EVOLUTION

}
