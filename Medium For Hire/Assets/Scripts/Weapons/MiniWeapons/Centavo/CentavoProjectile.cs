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
public class CentavoProjectile : MonoBehaviour
{
    [Header("STATS")]
    private float travelSpeed = 5f;
    private float damage;
    private float knockbackPower;
    private float knockbackDuration;

    [Header("DEBUG")]
    [SerializeField] private HashSet<BaseEnemy> alreadyHitEnemies = new HashSet<BaseEnemy>(); // only for chaining logic
    public float flipFrequency = 32f;
    private float originalXScale;

    [Header("REFERENCES")]
    private MiniWeapon_Centavo weapon; // get stats from this
    private CircleCollider2D collider;
    [SerializeField] private GameObject coinSprite;
    //
    // * DRIVER CODE
    public void Initialize(MiniWeapon_Centavo _weapon)
    {
        this.weapon = _weapon;
        this.collider = this.gameObject.GetComponent<CircleCollider2D>();

        originalXScale = coinSprite.transform.localScale.x;

        //originalXScale = .5f;

        this.DoScaleStats();
    }

    void Start()
    {
        Debug.Log(originalXScale);

        Destroy(gameObject, 6f);
    }

    private void FixedUpdate()
    {
        Travel();
    }

    private void Update()
    {
        float xScale = originalXScale * Mathf.Sin(Time.time * flipFrequency);
        coinSprite.transform.localScale = new Vector3(xScale, coinSprite.transform.localScale.y, coinSprite.transform.localScale.z);
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
        enemy.ApplyKnockback(this.transform.right, weapon.finalKnockbackPower, weapon.finalKnockbackDuration);


        Destroy(this.gameObject);
    }

    // *** CORE LOGIC
    //


    //
    // ** SUB FUNCTIONS

    // ** SUB FUNCTIONS


    //
    // TOOLS
    private void DoScaleStats()
    {
        travelSpeed = weapon.finalProjectileSpeed;
        damage = weapon.finalDamage;
        knockbackPower = weapon.finalKnockbackPower;
        knockbackDuration = weapon.finalKnockbackDuration;
    }
    // TOOLS
    //


    //
    // EVENTS & LISTENERS

    // EVENTS & LISTENERS
    //



}
