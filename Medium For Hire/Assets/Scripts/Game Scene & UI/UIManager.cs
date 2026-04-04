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

        [Header("MainWeapon UI")]
    [SerializeField] private Slider grudgeProgress;
    [SerializeField] private Slider guardProgress;
    [SerializeField] private Slider guideProgress;


    [Header("MiniWeapon UI")]
    [SerializeField] private Transform slotContainer;
    [SerializeField] private UI_WeaponSlot weaponSlotPrefab;

        [Header("Tooltips")]
    [SerializeField] public TooltipUI tooltip;

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
        float maxValue = PlayerController.Instance.playerStats.GetPlayerStat(Stat.MaxHealth);
        float currentvalue = PlayerController.Instance.playerStats.GetPlayerStat(Stat.CurrentHealth);

        hpSlider.maxValue = maxValue;
        //hpSlider.value = currentvalue;
        hpSlider.value = maxValue - currentvalue; // HP bar is inverted; bottom-to-top

        hpText.text = currentvalue + " / " + maxValue;
    }

    public void UpdateDomainProgress()
    {
        grudgeProgress.value = PlayerStats.Instance.GetPlayerStat(Stat.DomainOffense);
        guardProgress.value = PlayerStats.Instance.GetPlayerStat(Stat.DomainSurvival);
        guideProgress.value = PlayerStats.Instance.GetPlayerStat(Stat.DomainUtility);
    }

    public void AddWeaponSlot(WeaponUnlock _weaponUnlock, BaseWeapon _weaponData)
    {
        var newSlot = Instantiate(weaponSlotPrefab, slotContainer);
        newSlot.SetupSlot(_weaponUnlock, _weaponData);
        newSlot.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
    }

    private void Update()
    {
        // dont do this
        UpdateHpUI();
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
