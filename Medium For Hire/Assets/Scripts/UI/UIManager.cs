using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("EXP UI")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text levelText;

    [Header("HP UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;

    private void Awake()
    {
        // singleton 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        var health = PlayerController.Instance.GetComponent<HealthComponent>();
        float currentHealth = health.GetCurrentHealth();
        float maxHealth = health.GetMaxHealth();

        if (health != null)
        {
            health.OnHealthChanged += UpdateHpUI;
            UpdateHpUI(currentHealth, maxHealth);
        }
    }

    public void UpdateExpUI()
    {
        expSlider.maxValue = PlayerController.Instance.playerStats.GetPlayerStat(Stat.ExpToLevel);
        expSlider.value = PlayerController.Instance.playerStats.GetPlayerStat(Stat.CurrentExp);

        expText.text = expSlider.value + " / " + expSlider.maxValue;

        levelText.text = "Level " + PlayerController.Instance.playerStats.GetPlayerStat(Stat.CurrentLevel);

        //expSlider.maxValue = PlayerController.Instance.playerStats.expToLevelUp[PlayerController.Instance.playerStats.currentLevel - 1];
        //expSlider.value = PlayerController.Instance.playerStats.currentEXP;
        //expText.text = expSlider.value + " / " + expSlider.maxValue;    
    }

    public void UpdateHpUI(float current, float max)
    {
        hpSlider.maxValue = max;
        hpSlider.value = current;
        hpText.text = current + " / " + max;
    }

    //public void UpdateHpSlider()
    //{
    //    //hpSlider.maxValue = PlayerController.Instance.playerStats.maxHealth;
    //    //hpSlider.value = PlayerController.Instance.playerStats.currentHealth;
    //    var health = PlayerController.Instance.GetComponent<HealthComponent>();
    //    hpSlider.maxValue = health.GetMaxHealth();
    //    hpSlider.value = health.GetCurrentHealth();

    //    hpText.text = hpSlider.value + " / " + hpSlider.maxValue;
    //}
}
