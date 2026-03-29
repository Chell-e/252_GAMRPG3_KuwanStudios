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
            UpdateHpUI();
        }
    }

    public void UpdateExpUI()
    {
        expSlider.maxValue = PlayerController.Instance.playerStats.GetPlayerStat(Stat.ExpToLevel);
        expSlider.value = PlayerController.Instance.playerStats.GetPlayerStat(Stat.CurrentExp);

        expText.text = expSlider.value + " / " + expSlider.maxValue;

        levelText.text = "Level " + PlayerController.Instance.playerStats.GetPlayerStat(Stat.CurrentLevel);
    }

    public void UpdateHpUI()
    {
        // update player stats
        hpSlider.maxValue = PlayerController.Instance.playerStats.GetPlayerStat(Stat.MaxHealth);
        hpSlider.value = PlayerController.Instance.playerStats.GetPlayerStat(Stat.CurrentHealth);
        
        // update health component too
        hpSlider.maxValue = PlayerController.Instance.GetComponent<HealthComponent>().GetMaxHealth();
        hpSlider.value = PlayerController.Instance.GetComponent<HealthComponent>().GetCurrentHealth();

        hpText.text = hpSlider.value + " / " + hpSlider.maxValue;
    }
}
