using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [SerializeField] public PlayerStats playerStats;
    [SerializeField] public PlayerEvents Events;
    [SerializeField] public WeaponManager weaponManager;
    private void Awake() // for SINGLETON
    {
        // singleton 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    [Header("Player Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Player Movement")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] Animator animator;
    private Vector3 moveDirection;

        [Header("Player Map Boundaries")] // for limiting player to map boundaries
    [SerializeField] private Vector2 minPos;
    [SerializeField] private Vector2 maxPos;

        [Header("Aim Mechanics")]
    //[SerializeField] public bool IsAiming;
    [SerializeField] private Texture2D aimCursor; // change the cursor into a crosshair or smth
    [SerializeField] private Texture2D defaultCursor; // optional: leave null to use OS default

    //private Vector2 lastFacingDirection = Vector2.right;
    private Vector2 lastFacingDirectionX = Vector2.right;

    

    void Start()
    {
        //playerStats = GetComponent<PlayerStats>();
        
        /*exp needed for each level up(currently 10 muna per level)
        for (int i = 0; i < playerStats.maxLevel; i++)
            {
                playerStats.expToLevelUp.Add(10);
            }*/

        // initialize health
        //playerStats.currentHealth = playerStats.maxHealth;

        // update exp slider UI
        UIManager.Instance.UpdateExpUI();
    }

    void Update() // for most update logic stuff
    {
        if (Input.GetMouseButtonDown(1)) // *******MAKE SURE THAT THIS TRIGGERS UpdateFinalStats() ON ALL WEAPONS
            ToggleAimMode(); // toggle

        // get player input from Input Controller
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        if (inputX != 0) // update lastFacingDirectionX only when we move horizontally
        {
            lastFacingDirectionX = new Vector2(inputX, 0);
        }
        
        // set movement direction
        moveDirection = new Vector2(inputX, inputY).normalized;

        // flips sprite based on movement direction
        if (inputX < 0)
            spriteRenderer.flipX = true;
        if (inputX > 0)
            spriteRenderer.flipX = false;

        // animation
        if (inputX != 0 || inputY != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    void FixedUpdate() // for physics update stuff
    {
        var finalMoveSpeed = playerStats.GetPlayerStat(Stat.FinalMoveSpeed);
        var finalAimedMoveSpeed = playerStats.GetPlayerStat(Stat.FinalAimedMoveSpeed);

        if (!playerStats.isAiming) // if not aiming
        {
            rb.velocity = new Vector2(
                moveDirection.x * finalMoveSpeed,
                moveDirection.y * finalMoveSpeed
            );
        }
        else
        {
            rb.velocity = new Vector2(
                moveDirection.x * finalAimedMoveSpeed,
                moveDirection.y * finalAimedMoveSpeed
            );
        }


        // clamp player within map boundaries (wont need this if tileset map is implemented later)
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minPos.x, maxPos.x),
            Mathf.Clamp(transform.position.y, minPos.y, maxPos.y),
            transform.position.z
        );
    }

    public Vector2 GetLastFacingDirectionX() // to be called outside this class
    {
        return lastFacingDirectionX;
    }

    private void ToggleAimMode()
    {
        Events.OnAimToggle?.Invoke();

        playerStats.isAiming = !playerStats.isAiming;
        if (playerStats.isAiming)
        {
            Events.OnAimActivate?.Invoke();
            UnityEngine.Cursor.SetCursor(aimCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Events.OnAimDeactivate?.Invoke();
            UnityEngine.Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    public void TakeDamage(float damage, object damageSource = null)
    {
        DamageContext context = new DamageContext(); // context object
        context.damage = damage;

            // this below gives the enemy the option to pass itself as a target for proccing OnBeforeGetHit events.
            // simply pass "this" if so ---> TakeDamage(10, this)
        context.target = damageSource is BaseEnemy enemy
            ? enemy
            : null; 

        /*EVENT*/ Events.OnBeforeGetHit?.Invoke(context);

        //if (!context.isNulled)
        //    GetComponent<HealthComponent>().TakeDamage(damage);

        if (!context.isNulled)
            GetComponent<HealthComponent>().ReduceHealth(damage);

        UIManager.Instance.UpdateHpUI();

        /*EVENT*/
        Events.OnAfterGetHit?.Invoke(context);

    }

    // call this for proccing OnDamage effects 
    public void DealDamage(float damage, BaseEnemy enemy)
    {
        DamageContext context = new DamageContext();
        context.damage = damage;
        context.target = enemy;

            // invoke events
        /*EVENT*/ Events.OnBeforeDealDamage?.Invoke(context);

        enemy.TakeDamage(damage);

        /*EVENT*/ Events.OnAfterDealDamage?.Invoke(context);
    }
}
