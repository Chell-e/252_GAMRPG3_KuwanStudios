using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    public float maxHealthValue {  get; private set; }
    public float currentHealthValue { get; private set; }

    [SerializeField] private RectTransform topBar;
    //[SerializeField] private RectTransform bottomBar;
    [SerializeField] private float animationSpeed = 10f;

    private BaseEnemy enemy;
    private HealthComponent health;

    private float fullWidth;
    //private float targetWidth => maxHealthValue > 0 ? currentHealthValue * fullWidth / maxHealthValue : 0f;

    private Coroutine AdjustBarWidthCoroutine;

    private void Awake()
    {
        enemy = GetComponentInParent<BaseEnemy>();
        if (enemy != null)
        {
            health = enemy.GetComponent<HealthComponent>();
        }
    }

    private void Start()
    {
        fullWidth = topBar.rect.width;

        InitializeHealthValues();
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnEliteHealthChanged += HandleHealthChanged;
            StartCoroutine(DelayedInit());
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnEliteHealthChanged -= HandleHealthChanged;
        }

        if (AdjustBarWidthCoroutine != null)
        {
            StopCoroutine(AdjustBarWidthCoroutine);
            AdjustBarWidthCoroutine = null;
        }
    }

    private IEnumerator DelayedInit()
    {
        yield return new WaitForEndOfFrame();
        InitializeHealthValues();
    }

    private void InitializeHealthValues()
    {
        if (health != null)
        {
            maxHealthValue = health.GetMaxHealth();
            currentHealthValue = health.GetCurrentHealth();

            if (topBar != null) // && bottomBar != null
            {
                float initialWidth = CalculateWidthForHealth(currentHealthValue);
                topBar.SetWidth(initialWidth);
                //bottomBar.SetWidth(initialWidth);
            }    
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

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    private IEnumerator AdjustBarWidth(float oldHealth, float newHealth)
    {
        //var suddenChangeBar = amount >= 0 ? bottomBar : topBar;
        //var slowChangeBar = amount >= 0 ? topBar : bottomBar;
        //suddenChangeBar.SetWidth(targetWidth);

        //while (Mathf.Abs(suddenChangeBar.rect.width - slowChangeBar.rect.width) > 1f)
        //{
        //    slowChangeBar.SetWidth(
        //        Mathf.Lerp(slowChangeBar.rect.width, targetWidth, Time.deltaTime * animationSpeed));
        //    yield return null;
        //}
        //slowChangeBar.SetWidth(targetWidth);

        float startWidth = CalculateWidthForHealth(oldHealth);
        float endWidth = CalculateWidthForHealth(newHealth);

        // TAKING DAMAGE
        if (newHealth < oldHealth)
        {
            topBar.SetWidth(endWidth);

            //while (Mathf.Abs(bottomBar.rect.width - endWidth) > 0.5f)
            //{
            //    bottomBar.SetWidth(Mathf.Lerp(bottomBar.rect.width, endWidth, Time.deltaTime * animationSpeed));
            //    yield return null;
            //}
            //bottomBar.SetWidth(endWidth);
        }
        else // HEALING
        {
            //bottomBar.SetWidth(endWidth);

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
    }

}
