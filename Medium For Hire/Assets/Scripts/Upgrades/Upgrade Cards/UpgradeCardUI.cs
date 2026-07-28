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
    [Header("STUFF TO RENDER")]
    public Image cardIcon; // ICON OR IMAGE 
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    [Header("CARD UI REFERENCES")]
    public Image cardGraphic; // the main body of the card
    public Image rarityFrame;
    [Space]
    public Slider domainSlider;
    public Image domainFill;
    [Space]
    public Image cardHighlight; // for hovering
    public Button selectButton; // button for events?

    [Header("VARIETIES")]
    public Sprite[] cardColors;
    public Sprite[] domainBars;
    [Space]
    public Sprite[] rarityBars;
    public string[] rarityTexts;



    [Header("DEBUG")]
    private int colorIndex = 0;
    [Space]
    private BaseUpgradeData upgradeData;
    private Action<BaseUpgradeData> onSelected;

    // * DRIVER CODE
    public void Setup(BaseUpgradeData _upgradeData, Action<BaseUpgradeData> _onSelectedCallback)
    {
        upgradeData = _upgradeData;
        onSelected = _onSelectedCallback;


        colorIndex = CheckDomain(); // CheckDomain() returns a number matching a specific color whtvr
        if (colorIndex == 0
            || colorIndex == 1
            || colorIndex == 2)
        {
            if (domainFill != null) domainFill.sprite = domainBars[colorIndex];
        }
        else
        {
            // disable slider for yellow or white
            int newWidth = 513; // these were trial-and-errored...
            int newHeight = 730;
            int offsetX = -23;
            int offsetY = 20;
            Vector3 offset = new Vector3(offsetX, offsetY, 0);

            domainSlider.gameObject.SetActive(false);
            cardGraphic.rectTransform.SetWidth(newWidth);
            cardGraphic.rectTransform.SetHeight(newHeight);
            cardGraphic.rectTransform.localPosition = offset;

            cardIcon.transform.localPosition -= offset;

            rarityFrame.transform.localPosition -= offset; // adjust only the frame; the text is already centered
            titleText.transform.localPosition -= offset;
            descriptionText.transform.localPosition -= offset;

        }


        if (cardGraphic != null) cardGraphic.sprite = cardColors[colorIndex];

        if (cardIcon != null) cardIcon.sprite = _upgradeData.icon;

        
        if (rarityText != null) rarityText.text = rarityTexts[colorIndex];
        if (rarityFrame != null) rarityFrame.sprite = rarityBars[colorIndex];

        if (titleText != null) titleText.text = _upgradeData.title;
        if (descriptionText != null) descriptionText.text = _upgradeData.description;



        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelected?.Invoke(upgradeData));
        }
    }
    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below

    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions

    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)
    private int CheckDomain()
    {
        // 0 = grudge/red
        // 1 = guard/green
        // 2 = guide/blue
        // 3 = weapon/yellow
        // 4 = max/white

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

    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here
    public void OnPointerEnter(PointerEventData eventData)
    {
        cardHighlight.enabled = true;
        Debug.Log("DETECTED POINTER ENTER, SHOULD ENABLE OUTLINE!");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardHighlight.enabled = false;
        Debug.Log("EXITED POINTER, SHOULD DISABLE!");
    }

    private void OnDestroy()
    {
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
        }
    }

    // EVENTS & LISTENERS



    //
    // Works but no longer in use:
    /*private int CheckDomainPower()
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
    }*/
}