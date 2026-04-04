using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEditor.PackageManager;
using UnityEngine;

public class MiniWeapon_Crucifix : BaseWeapon
{
        [Header("Base Weapon Stats")]
            [Tooltip("Flat base charges.")]
    public int baseMaxCharges = 1;
            [Tooltip("Base recharge time. Affected by Player's Stats.")]
    public float baseRechargeTime = 10f;


            [Tooltip("Blast damage.")]
    public float blastDamage = 1f;
            [Tooltip("Blast knockback power.")]
    public float blastKnockbackPower = 1f;
            [Tooltip("Blast lifetime.")]
    public float blastLifetime = 0.2f;

        [Header("Effective Weapon Stats")]
            [Tooltip("Effective recharge CD per Charge.")]
    [SerializeField] private float rechargeTime;
            [Tooltip("Max charges.")]
    [SerializeField] private float maxCharges = 0;


        [Header("Runtime Stats")]
            [Tooltip("Effective recharge CD per Charge.")]
    [SerializeField] private float rechargeTimer;
    [SerializeField] private float currentCharges;


        [Header("References")]
            [Tooltip("GameObject displaying shield?")]
    [SerializeField] GameObject shieldAura;
            [Tooltip("Prefab generated on Crucifix proc.")]
    [SerializeField] GameObject crucifixBlastPrefab;

    protected override void Subscribe()
    {
        playerEvents.OnBeforeGetHit += OnBeforeGetHit;


    }

    protected override void Unsubscribe()
    {
        playerEvents.OnBeforeGetHit -= OnBeforeGetHit;
    }

    private void OnBeforeGetHit(DamageContext context)
    {
        if (currentCharges > 0)
        {
            currentCharges--;

            context.isNulled = true;
            context.damage = 0;

            /*context.target.ApplyKnockback
                (
                    context.target.transform.position - transform.position,
                    100,
                    1
                );*/
            SpawnCrucifixBlast();

            Debug.Log("Crucifix blocked damage! ");
        }
    }

    private void Start()
    {
        Subscribe();

        rechargeTime = baseRechargeTime;
        maxCharges = baseMaxCharges;

        rechargeTimer = 0;
        currentCharges = 0; // start without shield?
    }

    private void Update()
    {
        // for now, let's put the stat scaling in Update()
        maxCharges = baseMaxCharges + (playerStats.GetPlayerStat(Stat.MaxHealth) / 50);

        RunCrucifixVfxLogic();
        CrucifixStatsScaling();
        RunCrucifixRechargeLogic();
    }

    private void RunCrucifixRechargeLogic()
    {
        // return if at max charge
        if (currentCharges >= maxCharges)
        {
            currentCharges = maxCharges;
            return;
        }

        // otherwise keep incrementing rechargeTimer
        rechargeTimer += Time.deltaTime;
        if (rechargeTimer >= rechargeTime)
        {
            currentCharges++;
            rechargeTimer = 0f;
            Debug.Log("Crucifix Charges at " + currentCharges);
        }
    }

    private void CrucifixStatsScaling()
    {
        maxCharges =
            baseMaxCharges
            + ((float)playerStats.GetPlayerStat(Stat.MaxHealth) / 50f);

    }

    private void RunCrucifixVfxLogic()
    {
        if (currentCharges > 0)
            shieldAura.SetActive(true);
        else
            shieldAura.SetActive(false);
    }

    private void SpawnCrucifixBlast()
    {
        GameObject blastObject = Instantiate(crucifixBlastPrefab, transform.position, Quaternion.identity);

        CrucifixBlast blastStats = blastObject.GetComponent<CrucifixBlast>();
        blastStats.lifetime = blastLifetime;
        blastStats.knockbackPower = blastKnockbackPower;
        blastStats.blastDamage = blastDamage;
    }


    public override float GetFillProgress()
    {
        return rechargeTimer / rechargeTime;
    }

    public override string GetTooltipText()
    {
        string name = "Crucifix";

        /*float _maxCharges = 0f;
        _maxCharges = this.maxCharges;

        float _rechargeTimer = 0f;
        _rechargeTimer = this.rechargeTimer;

        float _rechargeTime = 0f;
        _rechargeTime = this.rechargeTime;*/

        string description =
            $"Name: \"{name}\"" +
            $"\nCharges: {currentCharges}/{maxCharges}" +
            $"\nCooldown: {(int)rechargeTimer}/{rechargeTime}s"
            ;

        return description;

    }
}
