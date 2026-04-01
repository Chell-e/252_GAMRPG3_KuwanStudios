using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    public bool IsDead { get; set; }
    public bool CanDie { get; set; } = true;


    public event Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // GETTERS
    public float GetCurrentHealth() { return currentHealth; }
    public float GetMaxHealth() { return maxHealth; }

    // SETTERS
    public void SetCurrentHealth(float amount) 
    { 
        currentHealth = amount; 
    }

    public void SetMaxHealth(float amount) 
    { 
        maxHealth = amount;
    }


    public void ReduceHealth(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // for manananggal
        if (currentHealth <= 0f && !CanDie)
        {
            currentHealth = 1f;
        }

        // for normal enemies and player
        if (currentHealth <= 0f && CanDie)
        {
            Die();
        }
    }

    public void Die()
    {
        IsDead = true;
        OnDeath?.Invoke();
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        IsDead = false;
    }
}
