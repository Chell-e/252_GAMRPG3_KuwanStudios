using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    public bool IsDead { get; set; }

    //public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public float GetCurrentHealth() { return currentHealth; }
    public float GetMaxHealth() { return maxHealth; }

    public void SetCurrentHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
    }

    // new!
    public void TriggerDeath()
    {
        OnDeath?.Invoke();
    }

    //public void TakeDamage(float damage)
    //{
    //    if (IsDead) return;

    //    currentHealth -= damage;
    //    OnHealthChanged?.Invoke(currentHealth, maxHealth);

    //    // TRIGGER HIT FLASH HERE
    //    var hitFlash = GetComponent<HitFlash>();
    //    if (hitFlash != null)
    //    {
    //        hitFlash.TriggerHitFlash();
    //    }

    //    if (currentHealth <= 0)
    //    {
    //        currentHealth = 0;
    //        IsDead = true;
    //        OnDeath?.Invoke();
    //    }
    //}

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
    }

    public void Heal(float amount)
    {
        if (IsDead)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        IsDead = false;
    }
}
