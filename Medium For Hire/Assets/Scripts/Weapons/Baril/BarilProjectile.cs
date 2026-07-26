using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class BarilProjectile : MonoBehaviour
{
    [Header("STATS")]
    private float travelSpeed;
    private float damage;

    [Header("DEBUG")]
    [SerializeField] private HashSet<BaseEnemy> alreadyHitEnemies = new HashSet<BaseEnemy>(); // only for chaining logic
    private float momentumStacks = 0; // increment per hit

    [Header("REFERENCES")]
    private MainWeapon_Baril weapon; // get stats from this
    private CircleCollider2D collider;
    //
    // * DRIVER CODE
    public void Initialize(MainWeapon_Baril _weapon)
    {
        this.weapon = _weapon;
        travelSpeed = weapon.finalBulletSpeed;
        damage = weapon.finalDamage;
        this.collider = this.gameObject.GetComponent<CircleCollider2D>();

        this.DoScaleStats();
    }

    void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        Travel();
    }
    // * DRIVER CODE
    //


    //
    // *** CORE LOGIC
    private void Travel()
    {
        // MAKE SURE BULLET FACES RIGHT!
        this.transform.position += transform.right * (this.travelSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseEnemy enemy = collision.GetComponent<BaseEnemy>();

        if (enemy == null)
            return;

        PlayerController.Instance.DealDamage(damage, enemy);

        if (DoPierceCheck() == false) // if failed to pierce
        {
            Debug.Log("PIERCE FAILED, SHOULD DESTROY NOW");
            Destroy(this.gameObject);
        }
        
        alreadyHitEnemies.Add(enemy);

        if (weapon.isOffenseEvolved)
        {
            momentumStacks++;
            DoScaleStats();
        }
            

        if (weapon.isUtilityEvolved)
            Ricochet();

    }

    // *** CORE LOGIC
    //


    //
    // ** SUB FUNCTIONS
    private void Ricochet()
    {
        Vector2 closestEnemyPos = Vector2.zero;

        // look thru nearby enemies
        Collider2D[] enemiesDetected = Physics2D.OverlapCircleAll
        (
            this.transform.position,
            10f,
            LayerMask.GetMask("Enemy")
        );

        float closestDistance = 99f;
        foreach (var enemyDetected in enemiesDetected)
        {
            BaseEnemy enemy = enemyDetected.GetComponent<BaseEnemy>();

            // SKIP if alreadyHit
            if (alreadyHitEnemies.Contains(enemy))
                continue;

            // check for the distance between enemy and yourself
            if (enemy != null)
            {
                float currentDistance = Vector2.Distance(enemy.transform.position, this.transform.position);

                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestEnemyPos = enemy.transform.position;
                }
            }
        }

        Vector2 newDirection = closestEnemyPos - (Vector2)this.transform.position;

        this.transform.right = newDirection.normalized;
    }

    private bool DoPierceCheck()
    {
        float pierceRoll = Random.Range(0, 100f);

        if (pierceRoll <= weapon.finalPierceChance)
            return true;
        else
            return false;
    }

    // ** SUB FUNCTIONS


    //
    // TOOLS
    private void DoScaleStats()
    {
        damage = damage
            + momentumStacks * weapon.finalMomentumStackDmg;  
    }
    // TOOLS
    //


    //
    // EVENTS & LISTENERS

    // EVENTS & LISTENERS
    //



}
