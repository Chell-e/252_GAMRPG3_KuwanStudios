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
public class TiradorProjectile : MonoBehaviour
{
    [Header("STATS")]
    private float travelSpeed = 5f;
    private float damage;

    [Header("DEBUG")]
    [SerializeField] private HashSet<BaseEnemy> alreadyHitEnemies = new HashSet<BaseEnemy>(); // only for chaining logic
    private int bounces = 0;

    [Header("REFERENCES")]
    private MiniWeapon_Tirador weapon; // get stats from this
    private CircleCollider2D collider;
    //
    // * DRIVER CODE
    public void Initialize(MiniWeapon_Tirador _weapon)
    {
        this.weapon = _weapon;
        this.collider = this.gameObject.GetComponent<CircleCollider2D>();

        this.DoScaleStats();
    }

    void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void FixedUpdate()
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

        if (bounces >= weapon.finalMaxBounces)
        {
            Destroy(this.gameObject);
            Debug.Log("max bounces hit");
        }
        else
        {
            Ricochet();
            bounces++;
            Debug.Log("RICOCHETED!");
        }



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

            Debug.Log($"detected: {enemy}");

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

    // ** SUB FUNCTIONS


    //
    // TOOLS
    private void DoScaleStats()
    {
        travelSpeed = weapon.finalProjectileSpeed;
        damage = weapon.finalDamage;
    }
    // TOOLS
    //


    //
    // EVENTS & LISTENERS

    // EVENTS & LISTENERS
    //



}
