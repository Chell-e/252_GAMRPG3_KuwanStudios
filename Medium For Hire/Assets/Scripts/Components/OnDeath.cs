 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeath : MonoBehaviour
{
    public void HandleDeath()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        EnemyAI enemyAI = GetComponent<EnemyAI>();

        if (playerController != null)
        {
            gameObject.SetActive(false);
            UIManager.Instance.UpdateHpSlider();
        }
        else if (enemyAI != null)
        {

            // Find a way to make HitFlash reset properly
            // since ResetFlash() is private
            // enemyAI.GetComponent<HitFlash>().ResetFlash();

            PoolManager.ReturnObjectToPool(gameObject);
            PoolManager.SpawnObject(enemyAI.orbPrefab, transform.position, transform.rotation, PoolManager.PoolType.ExpOrb);
        }
    }
}
