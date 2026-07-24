using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using Unity.VisualScripting;
using System.Linq.Expressions;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Superstition UI")]
    [SerializeField] private TMP_Text superstitionText;

    [Header("Pause Panel")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;
    public bool IsPausePanelActive { get; private set; }
    public bool IsOptionsPanelActive { get; private set; }
    public bool IsTabPanelActive { get; private set; }

    [Header("Information Tab Panel ")]
    [SerializeField] private GameObject infoTabPanel;
    [SerializeField] private TMP_Text superstitionNameText;
    [SerializeField] private TMP_Text superstitionDescriptionText;
    [SerializeField] private TMP_Text superstitionRewardText;
    [SerializeField] private TMP_Text superstitionPenaltyText;

    [Header("Info Tab - Player Stats")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TMP_Text healthStatsText;
    [SerializeField] private TMP_Text damageStatsText;
    [SerializeField] private TMP_Text attackSpeedStatsText;
    [SerializeField] private TMP_Text projectileSpeedStatsText;
    [SerializeField] private TMP_Text moveSpeedStatsText;
    [SerializeField] private TMP_Text pickupRangeStatsText;

    [Header("Info Tab - Hovered Item")]
    [SerializeField] private GameObject hoveredItemPanel;
    [SerializeField] private TMP_Text hoveredItemNameText;
    [SerializeField] private TMP_Text hoveredItemDescriptionText;

    [Header("EXP UI")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TMP_Text levelText;


    [Header("BOSS UI")]
    [SerializeField] private Image bossTimerForeground;
    [SerializeField] private Image bossTimerBackground;
    [SerializeField] private TMP_Text bossHpText;

    public Image BossTimerForeground => bossTimerForeground;
    public Image BossTimerBackground => bossTimerBackground;

        [Header("HP UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;

        [Header("Upgrade Screen")]
    [SerializeField] private Image upgradeBackground;

        [Header("MainWeapon UI")]
    [SerializeField] private UI_WeaponSlot mainSlot;

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

    private void OnEnable()
    {
        //SuperstitionManager.OnSuperstitionBroken += CrackAntingAnting;
    }

    private void OnDisable()
    {
        //SuperstitionManager.OnSuperstitionBroken -= CrackAntingAnting;
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

        levelText.text = "Level " + PlayerController.Instance.playerStats.GetPlayerStat(Stat.CurrentLevel);
    }

    public void UpdateHpUI()
    {
        float maxValue = PlayerController.Instance.GetComponent<HealthComponent>().GetMaxHealth();
        float currentvalue = PlayerController.Instance.GetComponent<HealthComponent>().GetCurrentHealth();

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

    public void SetupMainWeaponSlot(WeaponUnlock _weaponUnlock, BaseWeapon _weaponData)
    {
        mainSlot.SetupSlot(_weaponUnlock, _weaponData);
    }

    public void AddWeaponSlot(WeaponUnlock _weaponUnlock, BaseWeapon _weaponData)
    {
        var newSlot = Instantiate(weaponSlotPrefab, slotContainer);
        newSlot.SetupSlot(_weaponUnlock, _weaponData);
        newSlot.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
    }

    private void Update()
    {
        // dont do this
        //UpdateHpUI();

        if (StageManager.Instance == null) return;
        if (StageManager.Instance.isGameOver) return;

        if (Input.GetKeyDown(KeyCode.Tab) && IsPausePanelActive == false)
        {
            ToggleInfoTab();
        }
    }

    public void ToggleInfoTab()
    {
        if (!infoTabPanel.activeSelf)
        {
            UpdateInfoTabStat();

            IsTabPanelActive = true;
            infoTabPanel.SetActive(true);
            ShowPlayerStatsInfo();
            Time.timeScale = 0f; // pause
        }
        else
        {
            IsTabPanelActive = false;
            infoTabPanel.SetActive(false);

            // check if they're in the upgrade screen
            if (!upgradeBackground.gameObject.activeSelf)
            {
                Time.timeScale = 1f;
            }
        }
    }

    private void UpdateInfoTabStat()
    {
        var stats = PlayerStats.Instance;

        healthStatsText.text = stats.maxHealthPercent.ToString() + "%";
        damageStatsText.text = stats.dmgPercent.ToString() + "%";
        attackSpeedStatsText.text = stats.atkSpeedPercent.ToString() + "%";
        projectileSpeedStatsText.text = stats.projectileSpeedPercent.ToString() + "%";
        moveSpeedStatsText.text = stats.movespeedPercent.ToString() + "%";
        pickupRangeStatsText.text = stats.pickupRangePercent.ToString() + "%";
    }

    public void ShowHoveredItemInfo(ITooltipProvider provider)
    {
        statsPanel.SetActive(false);
        hoveredItemPanel.SetActive(true);

        hoveredItemNameText.text = provider.GetName();
        hoveredItemDescriptionText.text = provider.GetDescription();
    }

    public void ShowPlayerStatsInfo()
    {
        hoveredItemPanel.SetActive(false);
        statsPanel.SetActive(true);

        UpdateInfoTabStat();
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

    public void TogglePauseScreen(bool isDisplayed)
    {
        if (isDisplayed)
        {
            IsPausePanelActive = true;
            pausePanel.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            IsPausePanelActive = false;
            pausePanel.gameObject.SetActive(false);

            if (IsTabPanelActive) return;
            Time.timeScale = 1f;
        }
    }

    public void ToggleOptionsPanel(bool isDisplayed)
    {
        if (isDisplayed)
        {
            IsOptionsPanelActive = true;
            optionsPanel.gameObject.SetActive(true);
            pausePanel.gameObject.SetActive(false);
        }
        else
        {
            IsOptionsPanelActive = false;
            optionsPanel.gameObject.SetActive(false);
        }
    }

    public void UpdateSuperstitionUI(string name, string description, string rewardText, string penaltyText)
    {
        superstitionText.text = name;

        superstitionNameText.text = name;
        superstitionDescriptionText.text = description;
        superstitionRewardText.text = rewardText;
        superstitionPenaltyText.text = penaltyText;
    }

    public void UpdateBossHpText(GameObject boss)
    {
        bossHpText.text = boss.name;
    }

    private IEnumerator ShakeAmulet(RectTransform rt)
    {
        Vector3 originalPos = rt.localPosition;
        float elapsed = 0f;
        float duration = 0.2f;
        float magnitude = 5f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            rt.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rt.localPosition = originalPos;
    }
}
