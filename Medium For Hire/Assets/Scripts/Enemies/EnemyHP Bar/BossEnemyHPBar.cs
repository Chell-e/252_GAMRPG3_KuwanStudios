using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyHPBar : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private GameObject bossEnemyHpBar;
    [SerializeField] private RectTransform topBar;
    [SerializeField] private float animationSpeed = 10f;

    public float maxHealthValue { get; private set; }
    public float currentHealthValue { get; private set; }

    private HealthComponent health;
    private float fullWidth;
    private Coroutine AdjustBarWidthCoroutine;

    private void Awake()
    {
        if (topBar != null) fullWidth = topBar.rect.width;
        if (bossEnemyHpBar != null) bossEnemyHpBar.SetActive(false);
    }

    public void DisplayBossHPBar(HealthComponent bossHealth)
    {
        if (health != null)
        {
            health.OnBossHealthChanged -= HandleHealthChanged;
        }

        health = bossHealth;

        if (health != null)
        {
            health.OnBossHealthChanged += HandleHealthChanged;

            maxHealthValue = health.GetMaxHealth();
            currentHealthValue = health.GetMaxHealth();

            float initialWidth = CalculateWidthForHealth(currentHealthValue);
            if (topBar != null) topBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initialWidth);
            if (bossEnemyHpBar != null) bossEnemyHpBar.SetActive(true);
        }
    }

    public void HideBossHPBar()
    {
        if (bossEnemyHpBar != null) bossEnemyHpBar.SetActive(false);
    }

    private void OnDisable()
    {

        if (health != null)
        {
            health.OnBossHealthChanged -= HandleHealthChanged;
        }

        if (AdjustBarWidthCoroutine != null)
        {
            StopCoroutine(AdjustBarWidthCoroutine);
            AdjustBarWidthCoroutine = null;
        }

    }

    private float CalculateWidthForHealth(float healthValue)
    {
        if (maxHealthValue <= 0) return 0f;
        return (healthValue / maxHealthValue) * fullWidth;
    }

    private void HandleHealthChanged(float amount)
    {
        Change(amount);
    }

    private IEnumerator AdjustBarWidth(float oldHealth, float newHealth)
    {
        float startWidth = CalculateWidthForHealth(oldHealth);
        float endWidth = CalculateWidthForHealth(newHealth);

        // TAKING DAMAGE
        if (newHealth < oldHealth)
        {
            topBar.SetWidth(endWidth);
        }
        else
        {
            while (Mathf.Abs(topBar.rect.width - endWidth) > 0.5f)
            {
                topBar.SetWidth(Mathf.Lerp(topBar.rect.width, endWidth, Time.deltaTime * animationSpeed));
                yield return null;
            }
            topBar.SetWidth(endWidth);
        }
    }

    public void Change(float amount)
    {
        float previousHealth = currentHealthValue;
        currentHealthValue = Mathf.Clamp(currentHealthValue + amount, 0, maxHealthValue);

        if (AdjustBarWidthCoroutine != null)
        {
            StopCoroutine(AdjustBarWidthCoroutine);
        }
        AdjustBarWidthCoroutine = StartCoroutine(AdjustBarWidth(previousHealth, currentHealthValue));
    
        if (currentHealthValue <= 0) HideBossHPBar();
    }


}
