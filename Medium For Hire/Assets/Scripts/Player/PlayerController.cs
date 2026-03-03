using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour, IDamageable
{
    public static PlayerController Instance;

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
    [SerializeField] private Texture2D aimCursor; // change the cursor into a crosshair or smth
    [SerializeField] private Texture2D defaultCursor; // optional: leave null to use OS default

    public PlayerStats playerStats;
    //private Vector2 lastFacingDirection = Vector2.right;
    private Vector2 lastFacingDirectionX = Vector2.right;

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

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        
        /*exp needed for each level up(currently 10 muna per level)
        for (int i = 0; i < playerStats.maxLevel; i++)
            {
                playerStats.expToLevelUp.Add(10);
            }*/

        // initialize health
        //playerStats.currentHealth = playerStats.maxHealth;

        // update exp slider UI
        UIManager.Instance.UpdateExpSlider();
        UIManager.Instance.UpdateHpSlider();
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
        // move player
        if (!playerStats.isAiming) // if not aiming
        {
            rb.velocity = new Vector2(
                moveDirection.x * playerStats.GetFinalMovespeed(),
                moveDirection.y * playerStats.GetFinalMovespeed()
            );
        }
        else
        {
            rb.velocity = new Vector2(
                moveDirection.x * playerStats.GetFinalAimedMovespeed(),
                moveDirection.y * playerStats.GetFinalAimedMovespeed()
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

    //public void UpdateHealth(int damage)
    //{
    //    GetComponent<HealthComponent>().TakeDamage(damage);
    //    UIManager.Instance.UpdateHpSlider();
    //}

    private void ToggleAimMode()
    {
        playerStats.isAiming = !playerStats.isAiming;
        if (playerStats.isAiming)
        {
            UnityEngine.Cursor.SetCursor(aimCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            UnityEngine.Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    public void ApplyDamage(float damage)
    {
        GetComponent<HealthComponent>().TakeDamage(damage);
        UIManager.Instance.UpdateHpSlider();
    }
}
