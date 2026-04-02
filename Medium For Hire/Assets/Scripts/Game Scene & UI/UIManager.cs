using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
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

        [Header("Upgrade Screen")]
    [SerializeField] private Image upgradeBackground;

    [Header("End Run Screen")]
    [SerializeField] private GameObject endRunScreen;

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

    public void ToggleUpgradeGraphics(bool isOn)
    {
        if (isOn) 
            upgradeBackground.gameObject.SetActive(true);
        else
            upgradeBackground.gameObject.SetActive(false);
    }

    public void DisplayEndRunScreen()
    {
        endRunScreen.gameObject.SetActive(true);
    }
}
