using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class JuruPakalController : MonoBehaviour
{
    public enum JuruPakalEvolution
    {
        Base,
        Linger,
        Duplicate,
        Barrage // <- only one implemented right now
    }

    [Header("Knockback Applied")] // ========== new!
    public float knockbackForce = 5f;
    public float knockbackDuration = 1f;

    [Header("Weapon Base Stats")]
    [SerializeField] private float baseDamage = 2f;
    [SerializeField] private float baseRotationSpeed = -0.5f; // in rotations per second
    [SerializeField] private float baseMoveSpeed = 2f; // for moving between player and cursor

    [Header("Weapon Applied Stats")]
    [SerializeField] private bool isAimed = false;
    [SerializeField] private float finalDamage;
    [SerializeField] private float finalRotationSpeed;
    [SerializeField] private float finalMoveSpeed;

    [Header("Barrage Evolution Stats")]
    [SerializeField] private JuruPakalEvolution currentEvolution = JuruPakalEvolution.Base;

    // ======================= BARRAGE
    [SerializeField] private GameObject barrageArea; // object holding barrage evolution collider

    [SerializeField] private float barrageSpeedGain = 1f; // increments rotSpeed and moveSpeed by this each second
    private float barrageSpeedBonus; // total bonus speed acquired over time

    [SerializeField] private float barrageDuration = 5f; // barrage bonus expires after 5 seconds of reaching threshold
    private float barrageTimer;

    private float barrageDamageInterval = 0.1f; // time between dealing damage as an AoE
    private float barrageDamageTimer = 0f; // 
    // ======================= BARRAGE



    [Header("Important References")]
    [SerializeField] private GameObject weaponChaser; // object responsible for the tracking mechanic
    private float chaseStopDistance = 0.1f;

    [SerializeField] private GameObject weaponPivot; // rotate this gameObject; the sprite and circleCollision will be parented to it
    [SerializeField] private GameObject weaponArea; // object holding the collider
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] Vector2 bladeOffset; // see above

    private Vector3 mouseWorldPosition;


    [SerializeField] PlayerStats playerStats;


    // Start is called before the first frame update
    void Start()
    {
        weaponArea.transform.position = bladeOffset;

        // ensure the forwarder exists and points back to this controller
        var forwarder = weaponArea.AddComponent<HitboxForwarder>();
        forwarder.SetHitboxType(HitboxType.Hit);
        forwarder.owner = this;

        Evolve(JuruPakalEvolution.Barrage);
    }

    public void Evolve(JuruPakalEvolution evolutionChoice)
    {
        currentEvolution = evolutionChoice;

        if (evolutionChoice == JuruPakalEvolution.Barrage)
        {    
            var forwarder = barrageArea.AddComponent<HitboxForwarder>();
            forwarder.SetHitboxType(HitboxType.Interval);
            forwarder.owner = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFinalStats();


        if (Input.GetMouseButtonDown(1))
            isAimed = !isAimed; // toggle

        if (isAimed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition = new Vector3(mousePos.x, mousePos.y, 0);
        }



        // =====
        if (currentEvolution == JuruPakalEvolution.Barrage) BarrageLogic();
    }

    private void FixedUpdate()
    {
        float deltaDegrees = Time.deltaTime * (finalRotationSpeed * 360f);
        weaponPivot.transform.Rotate(0f, 0f, deltaDegrees);

        if (isAimed)
            ChaseTarget(mouseWorldPosition);
        else
            ChaseTarget(this.gameObject.transform.position);
    }

    private void UpdateFinalStats()
    {
        finalDamage = baseDamage * (playerStats.dmgPercent / 100f);
        finalMoveSpeed = baseMoveSpeed * (playerStats.projectileSpeedPercent / 100f);
        finalRotationSpeed = baseRotationSpeed * (playerStats.projectileSpeedPercent / 100f);
        
        if (currentEvolution == JuruPakalEvolution.Barrage)
        {
            finalRotationSpeed += barrageSpeedBonus;
            finalMoveSpeed += barrageSpeedBonus;
        }

        // add a buffer to stopDistance based on movespeed haha
        if (isAimed)
            chaseStopDistance = Mathf.Max(0.1f, finalMoveSpeed * .05f);
        else
            chaseStopDistance = Mathf.Min(0.25f, finalMoveSpeed * .025f); ;

    }

    private void ChaseTarget(Vector3 targetPosition)
    {
        if (Vector2.Distance(targetPosition, weaponChaser.transform.position) <= chaseStopDistance) // distance checker
        {
            //weaponChaser.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return;
        }

        Vector2 direction = (targetPosition - weaponChaser.transform.position).normalized;
        weaponChaser.GetComponent<Rigidbody2D>().MovePosition(
            weaponChaser.transform.position
            + (Vector3)(Time.deltaTime * (direction * finalMoveSpeed) )
            );

        /*weaponChaser.GetComponent<Rigidbody2D>().velocity = new Vector2(
            direction.x * finalMoveSpeed,
            direction.y * finalMoveSpeed
            );*/
    }

    private void BarrageLogic()
    {
        if (barrageTimer <= 0) // if out of duration, reset everything
        {
            barrageSpeedBonus = 0f;
            barrageTimer = barrageDuration;
        }
        barrageSpeedBonus += Time.deltaTime * barrageSpeedGain; // increment by speed gain
        barrageDamageTimer -= Time.deltaTime; // reduce timer


        if (Mathf.Abs(finalRotationSpeed) >= 5f)
        {
            barrageArea.GetComponent<SpriteRenderer>().enabled = true;
            barrageArea.GetComponent<CircleCollider2D>().enabled = true;

            weaponSprite.enabled = false;
            weaponArea.GetComponent<CircleCollider2D>().enabled = false;

            barrageTimer -= Time.deltaTime;
        }
        else
        {
            barrageArea.GetComponent<SpriteRenderer>().enabled = false;
            barrageArea.GetComponent<CircleCollider2D>().enabled = false;

            weaponSprite.enabled = true;
            weaponArea.GetComponent<CircleCollider2D>().enabled = true;
        }

    }

    public void HandleHitboxTriggerEnter(Collider2D collision)
    {
        //if (collision != null)
        //{
        //    if (!collision.GetComponent<EnemyAI>()) return; // return if not enemy
        //    collision.GetComponent<EnemyAI>().ApplyDamage(finalDamage * (playerStats.dmgPercent / 100f));
        //    //Debug.Log(finalDamage * (playerStats.dmgPercent / 100f));
        //}

        if (collision == null) return;

        BaseEnemy enemyHit = collision.gameObject.GetComponent<BaseEnemy>();

        if (enemyHit == null) return;

        float damage = finalDamage * (playerStats.dmgPercent / 100f);
        enemyHit.TakeDamage(damage);

        // APPLY KNOCKBACK
        Vector2 direction = (enemyHit.transform.position - weaponArea.transform.position).normalized;
        enemyHit.ApplyKnockback(direction, knockbackForce, knockbackDuration); 
            //Debug.Log("apply knockback, direction: " + direction);
    }

    public void HandleHitboxTriggerStay(List<Collider2D> collision)
    {
        bool hitEnemy = false;

        if (collision == null) return;
        
        foreach (var col in collision)
        {
            if (col.GetComponent<BaseEnemy>())
            {
                if (barrageDamageTimer <= 0f)
                {
                    //col.GetComponent<HealthComponent>().TakeDamage(0.1f * (playerStats.dmgPercent / 100f));
                    col.GetComponent<BaseEnemy>().TakeDamage(0.1f * (playerStats.dmgPercent / 100f));
                    hitEnemy = true;
                }
            }
        }

        if (hitEnemy) barrageDamageTimer = barrageDamageInterval / (playerStats.projectileSpeedPercent / 100f);

        
    }

}


// Small helper that forwards trigger callbacks from the weaponArea GameObject to the controller
public class HitboxForwarder : MonoBehaviour
{
    public JuruPakalController owner;
    public HitboxType hitboxType;

    private List<Collider2D> objectsInTrigger = new List<Collider2D>(); // foreach OnTriggerStay2D

    public void SetHitboxType(HitboxType hitboxType)
    {
        this.hitboxType = hitboxType;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        owner.HandleHitboxTriggerEnter(other);

        if (hitboxType == HitboxType.Interval)
        {
            objectsInTrigger.Add(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (hitboxType == HitboxType.Interval)
        {
            owner.HandleHitboxTriggerStay(objectsInTrigger);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (hitboxType == HitboxType.Interval)
        {
            objectsInTrigger.Remove(other);
        }
    }

}