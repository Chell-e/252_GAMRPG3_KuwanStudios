using System;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static StatUpgrade;
using System.Collections.Generic;

public class UpgradeCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image cardIcon; // ICON OR IMAGE 
       /*GONE*/ public Image cardBackground; // not relevant anymore

    public TextMeshProUGUI cardTitle; // TITLE
       /*GONE*/ public TextMeshProUGUI cardSubtitle; // not relevant anymore

        /*GONE*/ public TextMeshProUGUI cardFlavorText; // not relevant anymore
    public TextMeshProUGUI cardDescription; // DESCRIPTION
        /*GONE*/ public TextMeshProUGUI cardDomainText; // not relevant anymore

    [SerializeField] public Image domainBorderNormal;
    [SerializeField] public Image domainBorderHovered;

    public Transform domainPipLayoutGroup;
    public Transform rarityPipLayoutGroup;

    //public Image cardOutline;
    public Button selectButton; // button for events?

    //private UpgradeDefinition definition;
    //private Action<UpgradeDefinition> onSelected;

        /*GONE*/ [SerializeField] private Sprite[] domainBackgrounds; // not relevant anymore

    [SerializeField] private Sprite[] NewDomainBackgrounds;

    // *** Try turning these into nested lists instead.
    [SerializeField] private Sprite[] domainBars_Normal; // for hover[1] and not hover[0] 
    [SerializeField] private Sprite[] domainBars_Hover; // for hover[1] and not hover[0] 

    [SerializeField] private Sprite[] domainPips_Normal; // different colors for NON-WEAPON upgrades.

    [SerializeField] GameObject domainPipPrefab; // prefab that populates ring binder layout group
    [SerializeField] GameObject rarityPipPrefab; // 

    private BaseUpgradeData upgradeData;
    private Action<BaseUpgradeData> onSelected;

    private int colorIndex = 0;
    private int totalPips = 0;

    private List<GameObject> projectedDomainPips = new List<GameObject>();


    public void Setup(BaseUpgradeData _upgradeData, Action<BaseUpgradeData> _onSelectedCallback)
    {
        upgradeData = _upgradeData;
        onSelected = _onSelectedCallback;

        //cardBackground.sprite = domainBackgrounds[CheckDomain()];

        colorIndex = CheckDomain(); // CheckDomain() returns a number matching a specific color whtvr
        // might not be good practice

        //Debug.Log("color index is " + colorIndex);

        if (cardBackground != null) cardBackground.sprite = NewDomainBackgrounds[colorIndex];

        if (cardIcon != null) cardIcon.sprite = _upgradeData.icon;

        if (cardTitle != null) cardTitle.text = _upgradeData.title;
        if (cardSubtitle != null) cardSubtitle.text = _upgradeData.subtitle;

        if (cardFlavorText != null) cardFlavorText.text = _upgradeData.flavorText;
        if (cardDescription != null) cardDescription.text = _upgradeData.description;
        if (cardDomainText != null) cardDomainText.text = DomainIconsToTags(CheckDomain(), CheckDomainPower());
        //if (cardOutline != null) cardOutline.color = _upgradeData.cardOutlineColor;

        if (rarityPipLayoutGroup != null)
        {
            for (int i = 0; i < _upgradeData.rarityStars; i++)
            {
                GameObject rarityStar = Instantiate(rarityPipPrefab, rarityPipLayoutGroup.transform, false);
            }
        }

        // spawn pips based on current player shit
        if (domainPipLayoutGroup != null)
        {
            int playerDomainPower = 0; // player's current domain power of the given upgrade

            // first, check for the card's domain type
            switch (colorIndex)
            {
                case (0): // offense
                    playerDomainPower = PlayerController.Instance.playerStats.offenseDomainStat;
                    break;

                case (1): // survival
                    playerDomainPower = PlayerController.Instance.playerStats.survivalDomainStat;
                    break;

                case (2): //utility
                    playerDomainPower = PlayerController.Instance.playerStats.utilityDomainStat;
                    break;
                default: // idk
                    break;
            }

            for (int i = 0;
                    i < playerDomainPower;
                    i++)
            {
                if (i > 10) break;
                GameObject domainPip = Instantiate(domainPipPrefab, domainPipLayoutGroup.transform, false);
                domainPip.GetComponent<Image>().sprite = domainPips_Normal[colorIndex];

                totalPips++;
            }
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelected?.Invoke(upgradeData));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ClearProjectedDomainPips();

        //Debug.Log("hovered");

        domainBorderHovered.enabled = true;
        domainBorderNormal.enabled = false;

        if (CheckDomainPower() + totalPips > 10) return;

        for (int i = 0; i < CheckDomainPower(); i++)
        {
            GameObject projectedPip = Instantiate(domainPipPrefab, domainPipLayoutGroup.transform, false);
            projectedPip.GetComponent<Image>().sprite = domainPips_Normal[colorIndex];

            Color fadedColor = projectedPip.GetComponent<Image>().color;
            fadedColor.a = .5f;
            projectedPip.GetComponent<Image>().color = fadedColor;

            projectedDomainPips.Add(projectedPip);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(domainPipLayoutGroup.GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("exited hover");

        domainBorderHovered.enabled = false;
        domainBorderNormal.enabled = true;

        ClearProjectedDomainPips();
    }

    private void ClearProjectedDomainPips()
    {
        if (projectedDomainPips.Count == 0) return;

        foreach (GameObject pip in projectedDomainPips)
        {
            if (pip != null) Destroy(pip);
        }
        projectedDomainPips.Clear();
    }

    private void OnDestroy()
    {
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
        }
    }

    private int CheckDomain()
    {
        // 0 = grudge/red
        // 1 = guard/green
        // 2 = guide/blue
        // 3 = weapon/yellow
        // 4 = max/purple

        if (upgradeData is WeaponUnlock) return 3; // yellow
        if (upgradeData is WeaponEvolution) return 4; // purple

        StatUpgrade statUpgrade = upgradeData as StatUpgrade;
        int domainBackgroundIndex = 4; // return purple if no domain detected

        foreach (StatUpgradeData statData in statUpgrade.statsUpgraded)
        {
            switch (statData.statToUpgrade)
            {
                case StatUpgradeType.OffenseBonus:
                    domainBackgroundIndex = 0;
                    break;
                case StatUpgradeType.SurvivalBonus:
                    domainBackgroundIndex = 1;
                    break;
                case StatUpgradeType.UtilityBonus:
                    domainBackgroundIndex = 2;
                    break;
            }
        }

        return domainBackgroundIndex;
    }

    private int CheckDomainPower()
    {
        if (upgradeData is StatUpgrade == false) return 0; 

        StatUpgrade statUpgrade = upgradeData as StatUpgrade;
        int domainPower = 0;

        foreach (StatUpgradeData statData in statUpgrade.statsUpgraded)
        {
            switch (statData.statToUpgrade)
            {
                case StatUpgradeType.OffenseBonus:
                    domainPower = statData.value;
                    break;
                case StatUpgradeType.SurvivalBonus:
                    domainPower = statData.value;
                    break;
                case StatUpgradeType.UtilityBonus:
                    domainPower = statData.value;
                    break;
            }
        }

        return domainPower;
    }

    private string DomainIconsToTags(int _iconIndex, int _amount)
    {
        string iconTag = "";
        switch (_iconIndex)
        {
            case 0:
                iconTag = "<sprite name=\"grudge\">";
                break;
            case 1:
                iconTag = "<sprite name=\"guard\">";
                break;
            case 2:
                iconTag = "<sprite name=\"guide\">";
                break;

            default:
                return "";
        }

        string iconText = "";
        for (int i = 0; i < _amount; i++)
            iconText += iconTag;

        return iconText;
    }
}