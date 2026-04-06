using System;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static StatUpgrade;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("UI References")]
    public Image cardIcon;
    public Image cardBackground;

    public TextMeshProUGUI cardTitle;
    public TextMeshProUGUI cardSubtitle;

    public TextMeshProUGUI cardFlavorText;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI cardDomainText;

    //public Image cardOutline;
    public Button selectButton;

    //private UpgradeDefinition definition;
    //private Action<UpgradeDefinition> onSelected;

    [SerializeField] private Sprite[] domainBackgrounds; 

    private BaseUpgradeData upgradeData;
    private Action<BaseUpgradeData> onSelected;



    public void Setup(BaseUpgradeData _upgradeData, Action<BaseUpgradeData> _onSelectedCallback)
    {
        upgradeData = _upgradeData;
        onSelected = _onSelectedCallback;

        cardBackground.sprite = domainBackgrounds[CheckDomain()];
        if (cardIcon != null) cardIcon.sprite = _upgradeData.icon;

        if (cardTitle != null) cardTitle.text = _upgradeData.title;
        if (cardSubtitle != null) cardSubtitle.text = _upgradeData.subtitle;

        if (cardFlavorText != null) cardFlavorText.text = _upgradeData.flavorText;
        if (cardDescription != null) cardDescription.text = _upgradeData.description;
        cardDomainText.text = DomainIconsToTags(CheckDomain(), CheckDomainPower());
        //if (cardOutline != null) cardOutline.color = _upgradeData.cardOutlineColor;


        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelected?.Invoke(upgradeData));
        }
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
        // 3 = weapon/white
        // 4 = max/yellow

        if (upgradeData is WeaponUnlock) return 3; // white
        if (upgradeData is WeaponEvolution) return 4; // white

        StatUpgrade statUpgrade = upgradeData as StatUpgrade;
        int domainBackgroundIndex = 4; // return yellow if no domain detected

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