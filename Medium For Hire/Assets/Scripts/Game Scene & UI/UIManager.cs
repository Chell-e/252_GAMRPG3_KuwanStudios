using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

        [Header("Superstition UI")]
    [SerializeField] private TMP_Text superstitionText;
    [SerializeField] private Image[] antingAntingImages;
    [SerializeField] private Sprite[] unbrokenAntingSprite;
    [SerializeField] private Sprite[] crackedAntingSprites;

        [Header("Information Tab Panel ")]
    [SerializeField] private GameObject infoTabPanel;
    [SerializeField] private TMP_Text superstitionDescriptionText;
    [SerializeField] private TMP_Text superstitionFlavorText;

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

    private void OnEnable()
    {
        SuperstitionManager.OnSuperstitionBroken += CrackAntingAnting;
    }

    private void OnDisable()
    {
        SuperstitionManager.OnSuperstitionBroken -= CrackAntingAnting;
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
        //UpdateHpUI();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInfoTab();
        }
    }

    public void ToggleInfoTab()
    {
        if (!infoTabPanel.activeSelf)
        {
            UpdateInfoTabStat();

            infoTabPanel.SetActive(true);
            ShowPlayerStatsInfo();
            Time.timeScale = 0f; // pause
        }
        else
        {
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

    public void SetSuperstitionText(string name, string description, string flavorText)
    {
        superstitionText.text = name;
        superstitionDescriptionText.text = description;
        superstitionFlavorText.text = flavorText;

        // reset broken sprites
        for (int i = 0; i < antingAntingImages.Length; i++)
        {
            antingAntingImages[i].sprite = unbrokenAntingSprite[i];
        }
    }

    private void CrackAntingAnting(int violationCount)
    {
        int indexToCrack = violationCount - 1;

        if (indexToCrack >= 0 && indexToCrack < antingAntingImages.Length)
        {
            antingAntingImages[indexToCrack].sprite = crackedAntingSprites[indexToCrack];

            StartCoroutine(ShakeAmulet(antingAntingImages[indexToCrack].rectTransform));
            antingAntingImages[indexToCrack].color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
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
