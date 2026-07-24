using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // ***  singleton stuff
    public static PlayerController Instance;
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
    // ***  singleton stuff

    [Header("SETTINGS")]
    [SerializeField] public List<GameState> actionableGameStates;

    [Header("REFERENCES")]
    [SerializeField] public PlayerStats playerStats;
    [SerializeField] public PlayerEvents Events;
    [SerializeField] public WeaponManager weaponManager;


    [Header("Player Sprite")]
    [SerializeField] public SpriteRenderer spriteRenderer;

        [Header("Player")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] Animator animator;
    private Vector3 moveDirection;

        [Header("Dashing Ability")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 1f;
    private Vector2 dashDir;
    private bool isDashing;
    private bool canDash = true;
    TrailRenderer trailRenderer;

        [Header("Player Map Boundaries")] // for limiting player to map boundaries
    [SerializeField] private Vector2 minPos;
    [SerializeField] private Vector2 maxPos;

    [Header("Current Shrine I'm Standing On")]
    [SerializeField] private BaseShrine currentShrine;

    [Header("Aim Mechanics")]
    //[SerializeField] public bool IsAiming;
    [SerializeField] private Texture2D aimCursor; // change the cursor into a crosshair or smth
    [SerializeField] private Texture2D defaultCursor; // optional: leave null to use OS default

    //private Vector2 lastFacingDirection = Vector2.right;
    private Vector2 lastFacingDirectionX = Vector2.right;

    // * DRIVER CODE
    // mainly Start() and Update()
    void Start()
    {
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        //playerStats = GetComponent<PlayerStats>();

        /*exp needed for each level up(currently 10 muna per level)
        for (int i = 0; i < playerStats.maxLevel; i++)
            {
                playerStats.expToLevelUp.Add(10);
            }*/

        // initialize health
        //playerStats.currentHealth = playerStats.maxHealth;

        // update exp slider UI
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateExpUI();
    }

    void Update() // for most update logic stuff
    {
        // ----- COMMENTED THIS OUT 4 THE TUTORIAL 
        if (CheckIsGameStateActionable() == false)
            return;

        // dashing
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            isDashing = true;
            canDash = false;

            dashDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (dashDir == Vector2.zero)
            {
                dashDir = new Vector2(spriteRenderer.flipX ? -1f : 1f, 0f);
            }
            StartCoroutine(DashCoroutine());
        }

        if (isDashing)
        {
            return;
        }


        // shrine interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentShrine != null)
            {
                if (currentShrine.CurrentType == ShrineType.Spirit && SuperstitionManager.Instance.hasSuperstition) return;

                currentShrine.Interact();
            }
        }


        RunAimToggleLogic();

        RunMoveLogic();
    }

    void FixedUpdate() // for physics update stuff
    {
        if (isDashing)
        {
            rb.velocity = dashDir.normalized * dashSpeed;
            return;
        }

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

    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below
    private void RunAimToggleLogic()
    {
        if (Input.GetMouseButtonDown(1)) // *******MAKE SURE THAT THIS TRIGGERS UpdateFinalStats() ON ALL WEAPONS
            ToggleAimMode(); // 
    }

    private void RunMoveLogic()
    {
        // get player input from Input Controller
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        AnimatePlayerSprite(inputX, inputY);


        if (inputX != 0) // update lastFacingDirectionX only when we move horizontally
        {
            lastFacingDirectionX = new Vector2(inputX, 0);
        }

        // set movement direction
        moveDirection = new Vector2(inputX, inputY).normalized;


        // invoke OnAfterMove after player movement inputs
        if (inputX != 0 || inputY != 0)
        {
            MovementContext movementContext = new MovementContext();
            movementContext.inputAxes = new Vector2(inputX, inputY);

            Events.OnAfterMove?.Invoke(movementContext);
        }
    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions
    private bool CheckIsGameStateActionable()
    {
        // evaluates whether the current GameState matches actionableGameStates 

        if (GameStateManager.Instance == null)
        {
            // IF GameStateManager DOES NOT EXIST
            // LET US THROUGH!!!
            return true;
        }


        foreach (GameState actionableState in actionableGameStates)
        {
            // FOR EVERY LISTED GameState under actionableGameStates...
            if (GameStateManager.Instance.currentState == actionableState)
            {
                // IF CURRENT GAME STATE MATCHES ANY
                // LET US THROUGH!!!
                return true;
            }
        }

        // OR ELSE, BREAK!
        return false;
    }

    private void ToggleAimMode()
    {
        playerStats.isAiming = !playerStats.isAiming;

        AimContext aimContext = new AimContext();
        aimContext.isAiming = playerStats.isAiming;
        Events.OnAimToggle?.Invoke(aimContext);

        if (playerStats.isAiming)
        {
            Events.OnAimActivate?.Invoke();

            Vector2 hotspotCenter = new Vector2(36/2, 39/2);
            UnityEngine.Cursor.SetCursor(aimCursor, hotspotCenter, CursorMode.Auto);
        }
        else
        {
            Events.OnAimDeactivate?.Invoke();

            Vector2 hotspotDefault = new Vector2(5, 1);
            UnityEngine.Cursor.SetCursor(defaultCursor, hotspotDefault, CursorMode.Auto);
        }
    }

    private void AnimatePlayerSprite(float inputX, float inputY)
    {
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
    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)
    public Vector2 GetLastFacingDirectionX() // to be called outside this class
    {
        return lastFacingDirectionX;
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

        /*EVENT*/
        Events.OnBeforeGetHit?.Invoke(context);

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
        /*EVENT*/
        Events.OnBeforeDealDamage?.Invoke(context);

        enemy.TakeDamage(damage);

        /*EVENT*/
        Events.OnAfterDealDamage?.Invoke(context);
    }
    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here

    // EVENTS & LISTENERS




    private IEnumerator DashCoroutine()
    {
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        trailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void SetCurrentShrine(BaseShrine shrine)
    {
        currentShrine = shrine;
    }

    public void ClearCurrentShrine(BaseShrine shrine)
    {
        if (currentShrine == shrine)
        {
            currentShrine = null;
        }
    }


   


}
