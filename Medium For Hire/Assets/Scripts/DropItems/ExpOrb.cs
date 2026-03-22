using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [Range(0, 10)] public int maxExperienceValue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var randomExpValue = Random.Range(1, maxExperienceValue);
        
        if (collision.GetComponent<PlayerController>())
        {
            PlayerController.Instance.playerStats.GainExperience(randomExpValue);
            PoolManager.ReturnObjectToPool(gameObject);
        }
    }
}
