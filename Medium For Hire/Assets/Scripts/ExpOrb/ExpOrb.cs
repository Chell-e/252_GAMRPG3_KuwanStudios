using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    public int experienceValue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            PlayerController.Instance.GainExperience(experienceValue);
            PoolManager.ReturnObjectToPool(gameObject);
        }
    }
}
