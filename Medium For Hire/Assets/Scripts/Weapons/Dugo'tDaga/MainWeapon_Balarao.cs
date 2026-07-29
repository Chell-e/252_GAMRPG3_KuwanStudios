using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class MainWeapon_Balarao: BaseWeapon
{
    public enum BarilEvolution
    {
        Base,
        
    }

    [Header("BASE STATS")]
    //[SerializeField]
    private float baseCooldown = 1f; // same for either
    private float baseDamage = 1f;
    private float radius = .1f;

    private float baseDashCooldown = 3f; // cooldown for dash attack


    [Header("EFFECTIVE STATS")]
    public float finalCooldown;
    public float finalDamage;

    public float finalDashCooldown;

    [Header("DEBUG")]
    private float cooldownTimer = 99f;
    private bool isAimed = false;

    private float attackOffset = .2f;


    //==========
    [Header("EVOLUTIONS")]
        [Header("MAYARI EVO")]
    public bool isOffenseEvolved = false;

        [Header("TALA EVO")]
    public bool isSurvivalEvolved = false;

        [Header("HANAN EVO")]
    public bool isUtilityEvolved = false;
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
        // tick cooldown
        cooldownTimer += Time.deltaTime;


        if (cooldownTimer >= finalCooldown)
        {
            cooldownTimer = 0f;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mouseWorldPos = new Vector3(mousePos.x, mousePos.y, 0);

            Vector3 offsetVector = GetOffsetDirection(mouseWorldPos - this.transform.position);

            DoAttack(transform.position + offsetVector, mouseWorldPos);

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


        // BULLETS
        // reload time reduced by half of atkSpd, projSpd, and moveSpeed
/*        finalBulletSpeed =
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
*/

        // GRUDGE
        if (isOffenseEvolved)
        {
        }

        // GUARD
        if (isSurvivalEvolved)
        {
        }

        // GUIDE
        if (isUtilityEvolved)
        {
        }
    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions

    private void DoAttack(Vector2 _originPos, Vector2 _targetPos)
    {
        Vector2 directionVector = (_targetPos - _originPos).normalized;

        /*// spawn bullet
        GameObject attackInstance = Instantiate(attackPrefab, _originPos, Quaternion.identity);
        attackInstance.transform.right = directionVector;
        attackInstance.GetComponent<BarilProjectile>().Initialize(this);

        // spawn muzzle flash
        GameObject vfxInstance = Instantiate(vfxPrefab, _originPos, Quaternion.identity);
        vfxInstance.transform.right = directionVector;
        vfxInstance.GetComponent<BarilVFX>().Init(1f);
        vfxInstance.transform.SetParent(PlayerController.Instance.transform);*/
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
        string _name = "Baril ni Lola";
        string _cooldownTimer = (cooldownTimer).ToString("0.0");
        string _finalCooldown = (finalCooldown).ToString("0.0");

        /*string _currentAmmo = ammo.ToString();
        string _maxAmmo = finalMaxAmmo.ToString();*/

        string description =
            $"{_name}" +
            //$"\nAmmo: {_currentAmmo}/{_maxAmmo}" +
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


    private Vector2 GetOffsetDirection(Vector2 _directionVector)
    {
        Vector2 directionOffset = _directionVector.normalized;
        directionOffset *= attackOffset;

        return directionOffset;
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
