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
    //[SerializeField] private float moveSpeed;
    private Vector3 moveDirection;

    [Header("Player Map Boundaries")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private PlayerStats playerStats;

    //public int experience;
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
    }

    void Update()
    {
        // get player input
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

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
            Mathf.Clamp(transform.position.x, minX, maxX),
            Mathf.Clamp(transform.position.y, minY, maxY),
            transform.position.z
        );
    }
    public void GainExperience(int amount)
    {
        playerStats.currentEXP += amount;
    }
}
