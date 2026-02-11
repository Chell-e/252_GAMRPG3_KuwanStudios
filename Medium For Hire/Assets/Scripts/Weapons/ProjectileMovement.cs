using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public WeaponData weaponData;
    [SerializeField] private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * weaponData.projectileSpeed;
        Destroy(gameObject, weaponData.duration);
    }

    public void AimAtCursor()
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

    public void AimAtPlayerDirection(Vector2 direction)
    {
        float angle = (direction.x == -1f) ? 90f : -90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
