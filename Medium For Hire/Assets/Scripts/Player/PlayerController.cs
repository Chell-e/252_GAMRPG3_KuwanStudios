using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Player Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Player Movement")]
    [SerializeField] private Rigidbody2D rb;
    private Vector3 moveDirection;

    [Header("Player Map Boundaries")] // for limiting player to map boundaries
    [SerializeField] private Vector2 minPos;
    [SerializeField] private Vector2 maxPos;

    [Header("Aim Mechanics")]
    [SerializeField] private bool isAiming;
    [SerializeField] private Texture2D aimCursor; // change the cursor into a crosshair or smth
    [SerializeField] private Texture2D defaultCursor; // optional: leave null to use OS default

    [Header("Weapons Equipped")]
    public List<WeaponController> weaponList; // we need a List to support having multiple weapons later on

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
        weaponList.Add(GetComponentInChildren<WeaponController>());

        /*exp needed for each level up(currently 10 muna per level)
        for (int i = 0; i < playerStats.maxLevel; i++)
            {
                playerStats.expToLevelUp.Add(10);
            }*/

        // initialize health
        playerStats.currentHealth = playerStats.maxHealth;

        // update exp slider UI
        UIManager.Instance.UpdateExpSlider();
        UIManager.Instance.UpdateHpSlider();
    }

    void Update() // for most update logic stuff
    {
        if (Input.GetMouseButtonDown(1))
        { 
            ToggleAimForAllWeapons();
        }


        // get player input from Input Controller
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");


        /*if (inputX != 0 || inputY != 0) // update lastFacingDirection whenever moving
        {
            lastFacingDirection = new Vector2(inputX, inputY).normalized;
            Debug.Log(lastFacingDirection);
        }*/

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
    }

    void FixedUpdate() // for physics update stuff
    {
        // move player
        rb.velocity = new Vector2(
            moveDirection.x * playerStats.GetFinalMovespeed(),
            moveDirection.y * playerStats.GetFinalMovespeed()
        );

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

    public void TakeDamage(int damage)
    {
        playerStats.currentHealth -= damage;
        if (playerStats.currentHealth <= 0)
        {
            //Die();
            gameObject.SetActive(false);
        }

        UIManager.Instance.UpdateHpSlider();
    }

    public void ToggleAimForAllWeapons() 
    {
        Debug.Log("aim toggle");
        isAiming = !isAiming;
        Texture2D tex = isAiming ? aimCursor : defaultCursor;
        UnityEngine.Cursor.SetCursor(tex, Vector2.zero, CursorMode.Auto);

        foreach (var weapon in weaponList)
        {
            weapon.weaponData.isAimed = isAiming;
        }
    }

    /*public void ToggleAimForAllWeapons() // may be better to just serialize the weapons
    {
        Debug.Log("aim toggle");
        isAiming = !isAiming;
        Texture2D tex = isAiming ? aimCursor : defaultCursor;
        UnityEngine.Cursor.SetCursor(tex, Vector2.zero, CursorMode.Auto);

        ProjectileWeapon[] weapons = GetComponentsInChildren<ProjectileWeapon>();
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].isAimed = !weapons[i].isAimed;
        }
    }*/
}
