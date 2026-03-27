using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrucifixBlast : MonoBehaviour
{
    [SerializeField] public float blastDamage;
    [SerializeField] public float knockbackPower;
    [SerializeField] public float lifetime;

    //=======

    PlayerController playerController;


    void Start()
    {
        Destroy(gameObject,lifetime);

        playerController = PlayerController.Instance;

        BlastEnemiesInRadius();
    }

    private void BlastEnemiesInRadius()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, this.GetComponent<CircleCollider2D>().radius);
        foreach (var enemyHit in enemiesHit)
        {
            if (enemyHit.GetComponent<BaseEnemy>() != null)
            {
                BaseEnemy enemy = enemyHit.GetComponent<BaseEnemy>();
                playerController.DealDamage(blastDamage, enemy);

                Vector2 direction = enemy.transform.position - this.transform.position;
                enemy.ApplyKnockback(direction, knockbackPower, .2f);

                Debug.Log("Crucifix blasted " + enemy);


                // ffffffff
                StatusEffect_Vulnerable vulnerable = new StatusEffect_Vulnerable();
                vulnerable.Initialize(10f, 2.0f);
                enemy.ApplyStatusEffect(vulnerable);
            }
            
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        /*if (collision.GetComponent<BaseEnemy>())
        {
            BaseEnemy enemy = collision.GetComponent<BaseEnemy>();
            playerController.DealDamage(0, enemy);

            Vector2 direction = enemy.transform.position - this.transform.position;
            enemy.ApplyKnockback(direction, knockbackPower, .2f);

            Debug.Log("Crucifix blasted " + enemy);
        }*/
    }


}
