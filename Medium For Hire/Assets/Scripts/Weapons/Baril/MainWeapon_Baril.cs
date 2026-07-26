using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class MainWeapon_Baril : BaseWeapon
{
    public enum BarilEvolution
    {
        Base,
        Momentum,
        BulletTime,
        Ricochet
    }

    [Header("BASE STATS")]
    //[SerializeField]
    private float baseCooldown = 2f; // same for either
    private float baseDamage = 3f;

    private float baseBulletSpeed = 8f; // amount of ground covered per second?
    private float baseReloadTime = 3f;
    private float baseMaxAmmo = 4f;
    private float basePierceChance = 10f;


    [Header("EFFECTIVE STATS")]
    public float finalCooldown;
    public float finalDamage;

    public float finalBulletSpeed;
    public float finalReloadTime;
    public float finalMaxAmmo;
    public float finalPierceChance;


    [Header("DEBUG")]
    private float cooldownTimer = 99f;
    private bool isAimed = false;

    private float reloadTimer = 99f;
    private float ammo = 99f;

    private bool isBulletTimed = false;


    //==========
    [Header("EVOLUTIONS")]
        [Header("MAYARI EVO")]
    public bool isOffenseEvolved = false;
    private float baseMomentumStackDmg = 1f;
    public float finalMomentumStackDmg;

        [Header("TALA EVO")]
    public bool isSurvivalEvolved = false;
    private float baseTimeFactor = 0.85f;
    private float finalTimeFactor;

        [Header("HANAN EVO")]
    public bool isUtilityEvolved = false;
    public float utilityPierceBonus = 5f;
    // pierce bonus is lazily handled in DoScaleStats()
    
    //==========

    [Header("REFERENCES")]
    [SerializeField] public GameObject attackPrefab;
    [SerializeField] public GameObject vfxPrefab;
    //[Space]

    // * DRIVER CODE
    // mainly Start() and Update()
    private void Start()
    {
        this.Initialize(PlayerController.Instance);
        Subscribe();

        DoScaleStats();

        ammo = finalMaxAmmo;

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
        cooldownTimer += Time.deltaTime;
        
        // aim CD bonus
        if (isAimed)
            cooldownTimer += Time.deltaTime;

        // ammo check
        if (ammo <= 0)
        {
            if (isAimed || isSurvivalEvolved) // also reload if HANAN EVO
                reloadTimer += Time.deltaTime;

            if (reloadTimer >= finalReloadTime)
            {
                Reload(); // set ammo to full
                reloadTimer = 0;
            }

            return;
        }


        if (cooldownTimer >= finalCooldown)
        {
            cooldownTimer = 0f;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mouseWorldPos = new Vector3(mousePos.x, mousePos.y, 0);

            DoAttack(transform.position, mouseWorldPos);
            ammo--;
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


        // BULLETS
        // reload time reduced by half of atkSpd, projSpd, and moveSpeed
        finalBulletSpeed =
            baseBulletSpeed
            * (playerStats.GetPlayerStat(Stat.ProjectileSpeedPercent) / 100f);

        finalReloadTime =
            baseReloadTime *
            (
                +1
                +( (playerStats.GetPlayerStat(Stat.AttackSpeedPercent)      - 100f) / 100f) / 2f
                +( (playerStats.GetPlayerStat(Stat.ProjectileSpeedPercent)  - 100f) / 100f) / 2f
                +( (playerStats.GetPlayerStat(Stat.MoveSpeedPercent)        - 100f) / 100f) / 2f
            );

        finalMaxAmmo =
            baseMaxAmmo +
            Mathf.Ceil
            (
                (playerStats.GetPlayerStat(Stat.AreaPercent) / 50f)
            );

        finalPierceChance =
            basePierceChance *
            (
                +1
                +(playerStats.GetPlayerStat(Stat.AreaPercent) - 100) / 100f
            );


        // GRUDGE
        if (isOffenseEvolved)
        {
            finalMomentumStackDmg =
                baseMomentumStackDmg
                + (playerStats.GetPlayerStat(Stat.DomainOffense) / 10);
        }

        // GUARD
        if (isSurvivalEvolved)
        {
            // movespeed buff is stored in the Evolution Upgrade Card if ever!
            finalTimeFactor =
                baseTimeFactor
                - (playerStats.GetPlayerStat(Stat.DomainSurvival) * .001f); // 10 domain = 10% slower time
        }

        // GUIDE
        if (isUtilityEvolved)
        {
            float bonusPierce = utilityPierceBonus;
            float newBasePierce = basePierceChance + bonusPierce;

            finalPierceChance =
            newBasePierce
            * ((playerStats.GetPlayerStat(Stat.AreaPercent) - 100) / 100f);
        }
    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions

    private void DoAttack(Vector2 _originPos, Vector2 _targetPos)
    {
        Vector2 directionVector = (_targetPos - _originPos).normalized;

        // spawn bullet
        GameObject attackInstance = Instantiate(attackPrefab, _originPos, Quaternion.identity);
        attackInstance.transform.right = directionVector;
        attackInstance.GetComponent<BarilProjectile>().Initialize(this);

        // spawn muzzle flash
        GameObject vfxInstance = Instantiate(vfxPrefab, _originPos, Quaternion.identity);
        vfxInstance.transform.right = directionVector;
        vfxInstance.GetComponent<BarilVFX>().Init(1f);
        vfxInstance.transform.SetParent(PlayerController.Instance.transform);
    }

    private void Reload()
    {
        ammo = finalMaxAmmo;
    }

    private void DoBulletTime()
    {
        if (isAimed)
        {
            isBulletTimed = true;

            //Time.timeScale = finalTimeFactor;

            // this should probably handled by GameStateManager somehow?
            // store the "previous" time scale
            // so when returning from Paused to Unpaused, time is still slowed down
            // (instead of being overwritten as timeScale = 1)
        }
        else
        {
            isBulletTimed = false;
        }
    }
    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)
    public override float GetFillProgress()
    {
        if (ammo <= 0)
            return reloadTimer / finalReloadTime;

        return cooldownTimer / finalCooldown;
    }

    public override string GetTooltipText()
    {
        string _name = "Baril ni Lola";
        string _cooldownTimer = (cooldownTimer).ToString("0.0");
        string _finalCooldown = (finalCooldown).ToString("0.0");

        string description =
            $"{_name}" +
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

        if (isAimed && isSurvivalEvolved)
        {
            DoBulletTime();
        }
    }

    private void OnAfterGetUpgrade()
    {
        DoScaleStats();
        Debug.Log("AFTER GOT UPGRADE HEARD!");
    }
    // EVENTS & LISTENERS

}
