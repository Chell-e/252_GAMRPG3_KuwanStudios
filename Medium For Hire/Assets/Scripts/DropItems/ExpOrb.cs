using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExpOrb : MonoBehaviour
{
    // ==== EVENTS ====

        // static --> accessible globally
        // Action --> delegate method returns void
            // can use generic Action<T> to pass data with the event

    public static event Action OnExpOrbExpire;    

    // ==== EVENTS ====

    [Range(0, 10)] public int maxExperienceValue;
    [SerializeField] private float expireTime = 5f;

    private float spawnTimeElaped = 0f;


    private void OnEnable()
    {
        // track how much time has passed since it spawned
        // Time.timeSinceLevelLoad --> pauses when game is paused, so orbs don't expire when on upgrade screen

        spawnTimeElaped = Time.timeSinceLevelLoad;
    }

    private void Update()
    {
        // stop expiring when player's dead
        if (StageManager.Instance != null && StageManager.Instance.isGameOver)
            return;

        if (Time.timeSinceLevelLoad - spawnTimeElaped >= expireTime)
        {
            ExpireOrb();
        }
    }

    private void ExpireOrb()
    {
        // ? --> null check
        // event broadcast
        OnExpOrbExpire?.Invoke();

        PoolManager.ReturnObjectToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var randomExpValue = UnityEngine.Random.Range(1, maxExperienceValue + 1);
        
        if (collision.GetComponent<PlayerController>())
        {
            PlayerController.Instance.playerStats.GainExperience(randomExpValue);
            PoolManager.ReturnObjectToPool(gameObject);
        }
    }
}
