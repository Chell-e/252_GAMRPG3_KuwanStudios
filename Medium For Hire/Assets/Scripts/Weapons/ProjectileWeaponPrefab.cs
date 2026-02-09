using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponPrefab : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    public ProjectileWeapon projectileWeapon;
    public PlayerController playerController;

    private static float lastFacingDirectionX = 1f;

    void Start()
    {
        playerController = PlayerController.Instance;
        projectileWeapon = GetComponentInParent<ProjectileWeapon>();
        rb = GetComponent<Rigidbody2D>();

        // if projectile moves left &right only --------------- (Option 1)
        Vector2 currentFacingDirection = playerController.GetLastFacingDirection();
        if (currentFacingDirection.x < 0)
        {
            lastFacingDirectionX = -1f;
        }
        else if (currentFacingDirection.x > 0)
        {
            lastFacingDirectionX = 1f;
        }
        rb.velocity = new Vector2(lastFacingDirectionX * projectileWeapon.projectileSpeed, 0);

        //// omnidirectional projectile movement (if want naten) --------------- (Option 2)
        //rb.velocity = playerController.GetLastFacingDirection() * projectileWeapon.projectileSpeed;
        //float angle = (Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg) - 90f;
        //transform.rotation = Quaternion.Euler(0, 0, angle);

        // rotate projectile to face movement direction --------------- (Option 1)
        float angle = (lastFacingDirectionX == -1f) ? 90f : -90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, projectileWeapon.lifespan);
    }
    void Update()
    {

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyAI>())
        {
            // kills enemy
            collision.gameObject.GetComponent<EnemyAI>().TakeDamage(projectileWeapon.damage);
            Destroy(gameObject);
        }
    }
}
