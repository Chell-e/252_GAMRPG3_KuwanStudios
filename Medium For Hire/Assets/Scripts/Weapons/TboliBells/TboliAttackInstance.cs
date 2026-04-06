using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using static Unity.VisualScripting.Member;

public class TboliAttackInstance : MonoBehaviour
{
    public MainWeapon_TboliBells weapon; // get stats from this
    
    [SerializeField] private HashSet<BaseEnemy> alreadyHitEnemies = new HashSet<BaseEnemy>(); // only for chaining logic
    public int currentChains = 0;

    public float damageMultiplier = 1f; // maybe for decaying dmg on chain
    public float radiusMultiplier = 1f; // on chain too

    public void Initialize(MainWeapon_TboliBells _weapon)
    {
        this.weapon = _weapon;

    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
        StartCoroutine( PulseRoutine(this.transform.position) );
    }

    IEnumerator PulseRoutine(Vector2 _position)
    {
        //Debug.Log("DID IT CHAIN? " + currentChains);

        int totalPulses = 1; // 1 by default

        // if evolved, set to weapon's stat
        if (weapon.isGuardEvolved) totalPulses += weapon.finalExtraRings;

        for (int i = 0; i < totalPulses; i++)
        {
            Pulse(_position);
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void Pulse(Vector2 _position)
    {
        float pulseRadius = weapon.finalRadius;/* * radiusMultiplier;*/
        float pulseDamage = weapon.finalDamage;/* * damageMultiplier;*/

        Instantiate(weapon.vfxPrefab, _position, Quaternion.identity)
            .Init(pulseRadius);

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(
            _position,
            pulseRadius,
            LayerMask.GetMask("Enemy")
        );

        foreach (var enemyHit in enemiesHit)
        {
            BaseEnemy enemy = enemyHit.GetComponent<BaseEnemy>();

            // DEAL DMG
            PlayerController.Instance.DealDamage(pulseDamage, enemy );

            if (weapon.isGrudgeEvolved)
            {
                enemy.SetIncomingDamageModifier(weapon.finalDamageStatus); // permanent debuff
            }

            // APPLY KNOCKBACK
            Vector2 direction = (enemy.transform.position - this.transform.position).normalized;
            enemy.ApplyKnockback(direction, weapon.finalKnockbackPower, 0.1f);

            // TRY CHAINING
            if (weapon.isGuideEvolved
                && currentChains < weapon.finalMaxChains)
            {
                if (alreadyHitEnemies.Contains(enemy) == false)
                {
                    alreadyHitEnemies.Add(enemy.GetComponent<BaseEnemy>()); // enemies cannot be chained more than once

                    currentChains++; // ***make sure is placed before StartCoroutine

                    /*radiusMultiplier *= .8f;
                    damageMultiplier *= 1.1f;*/

                    StartCoroutine( PulseRoutine(enemy.transform.position) );
                    Debug.Log("Should chain now: " + enemy);
                    Debug.Log("At position: " + enemy.transform.position);

                }

            }

            alreadyHitEnemies.Add(enemy.GetComponent<BaseEnemy>());
        }

    }


}
