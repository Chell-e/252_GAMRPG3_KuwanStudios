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

    [Header("Player Map Boundaries")]
    [SerializeField] private Vector2 minPos;
    [SerializeField] private Vector2 maxPos;

    public PlayerStats playerStats;
    private Vector2 lastFacingDirection = Vector2.right;
    private void Awake()
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

        // exp needed for each level up (currently 10 muna per level)
        //for (int i = 0; i < playerStats.maxLevel; i++)
        //{
        //    playerStats.expToLevelUp.Add(10);
        //}

        // initialize health
        playerStats.currentHealth = playerStats.maxHealth;

        // update exp slider UI
        UIManager.Instance.UpdateExpSlider();
    }

    void Update()
    {
        // get player input
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        if (inputX != 0 || inputY != 0)
        {
            lastFacingDirection = new Vector2(inputX, inputY).normalized;
        }

        // set movement direction
        moveDirection = new Vector2(inputX, inputY).normalized;

        // flips sprite based on movement direction
        if (inputX < 0)
            spriteRenderer.flipX = true;
        if (inputX > 0)
            spriteRenderer.flipX = false;
    }

    void FixedUpdate()
    {
        // move player
        rb.velocity = new Vector2(
            moveDirection.x * playerStats.moveSpeed,
            moveDirection.y * playerStats.moveSpeed
        );

        // clamp player within map boundaries (wont need this if tileset map is implemented later)
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minPos.x, maxPos.x),
            Mathf.Clamp(transform.position.y, minPos.y, maxPos.y),
            transform.position.z
        );
    }
    public void GainExperience(int amount)
    {
        playerStats.currentEXP += amount;
        UIManager.Instance.UpdateExpSlider();

        if (playerStats.currentEXP >= playerStats.expToLevel)
        {
            playerStats.currentEXP = 0;
            playerStats.currentLevel++;
            UIManager.Instance.UpdateExpSlider();
        }

        //int index = playerStats.currentLevel - 1;
        //if (playerStats.currentEXP >= playerStats.expToLevelUp[index])
        //{
        //    playerStats.expToLevelUp[index]++;
        //}
    }

    public Vector2 GetLastFacingDirection()
    {
        return lastFacingDirection;
    }

    public void TakeDamage(int damage)
    {
        playerStats.currentHealth -= damage;
        if (playerStats.currentHealth <= 0)
        {
            //Die();
            gameObject.SetActive(false);
        }
    }
}
