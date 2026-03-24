using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEditor.PackageManager;
using UnityEngine;

public class MiniWeapon_MortarAndPestle : BaseWeapon
{
        [Header("Base Weapon Stats")]
            [Tooltip("Base cooldown between each blast. Affected by Player's Stats.")]
    public float baseCooldownTime = 5f;


            [Tooltip("Blast damage.")]
    public float blastDamage = 3f;
            [Tooltip("Blast lifetime.")]
    public float blastLifetime = 0.2f;

        [Header("Effective Weapon Stats")]
            [Tooltip("Effective recharge CD per Charge.")]
    [SerializeField] private float cooldownTime;


        [Header("Runtime Stats")]
            [Tooltip("Recharge timer.")]
    [SerializeField] private float cooldownTimer;


        [Header("References")]
            [Tooltip("Prefab generated on M&P proc.")]
    [SerializeField] GameObject mortarAndPestleBlastPrefab;
            [Tooltip("Radius for proccing AoE.")]
    [SerializeField] CircleCollider2D mortarAndPestleRadius; // keeping it a circleCollider for visualization


    protected override void Subscribe()
    {
        playerEvents.OnAfterKillEnemy += OnAfterKillEnemy;

    }

    protected override void Unsubscribe()
    {

    }

    private void OnAfterKillEnemy(DamageContext context)
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


        RunMortarAndPestleRechargeLogic();
    }


    private void RunMortarAndPestleRechargeLogic()
    {
        if (cooldownTimer >= cooldownTime)
        {
            Debug.Log("Mortar&Pestle blast");
            cooldownTimer = 0;

            SpawnMortarAndPestleBlast( DetectMortarAndPestleTarget() );
        }

        cooldownTimer += Time.deltaTime;


    }

    // return a reference to a target gameobject
    private GameObject DetectMortarAndPestleTarget()
    {
        GameObject target = null;

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, this.GetComponent<CircleCollider2D>().radius);
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

        Debug.Log(target);

        return target;
    }

    private void SpawnMortarAndPestleBlast(GameObject target)
    {
        GameObject blastObject = Instantiate(mortarAndPestleBlastPrefab, target.transform.position, Quaternion.identity);

        CrucifixBlast blastStats = blastObject.GetComponent<CrucifixBlast>();
        blastStats.lifetime = blastLifetime;
        blastStats.blastDamage = blastDamage;
    }
}
