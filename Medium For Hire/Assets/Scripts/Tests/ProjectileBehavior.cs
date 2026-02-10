using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for projectile behavior, attached to projectile gameobject
public class ProjectileBehavior : MonoBehaviour
{
    public WeaponData weaponData;
    public PlayerController playerController;

    [SerializeField] private Rigidbody2D rb;

    private static float lastFacingDirectionX = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (playerController == null)
            playerController = PlayerController.Instance;

        if (weaponData.isAimed)
        {
            AimAtCursor();
        }
        else
        {
            AimAtPlayerDirection();
        }

        rb.velocity = transform.up * weaponData.projectileSpeed;

        Destroy(gameObject, weaponData.duration);
    }

    private void AimAtCursor()
    {
        Vector3 mouseScreen = Input.mousePosition;
        Camera cam = Camera.main;

        if (cam != null)
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);
            // Ensure same Z-plane as spawner (2D game)
            mouseWorld.z = transform.position.z;

            Vector3 dir = mouseWorld - transform.position;
            if (dir.sqrMagnitude > 0.000001f)
            {
                // Angle in degrees where 0 points along +X. If your projectile faces up in its sprite,
                // add/subtract 90 degrees accordingly.
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }
        }
    }

    private void AimAtPlayerDirection()
    {
        Vector2 currentFacingDirection = playerController.GetLastFacingDirection();
        if (currentFacingDirection.x < 0)
        {
            lastFacingDirectionX = -1f;
        }
        else if (currentFacingDirection.x > 0)
        {
            lastFacingDirectionX = 1f;
        }
        float angle = (lastFacingDirectionX == -1f) ? 90f : -90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetData(WeaponData data, PlayerController pc)
    {
        weaponData = data;
        playerController = pc;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyAI>())
        {
            // kills enemy
            collision.gameObject.GetComponent<EnemyAI>().TakeDamage(weaponData.damage);
            Destroy(gameObject);
        }
    }
}
