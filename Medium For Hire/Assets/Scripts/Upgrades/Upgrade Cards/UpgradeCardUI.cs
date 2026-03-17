using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image cardOutline;
    public Button selectButton;

    //private UpgradeDefinition definition;
    //private Action<UpgradeDefinition> onSelected;

    private BaseUpgradeData upgradeData;
    private Action<BaseUpgradeData> onSelected;

    public void Setup(BaseUpgradeData _upgradeData, Action<BaseUpgradeData> onSelectedCallback)
    {
        upgradeData = _upgradeData;
        onSelected = onSelectedCallback;

        if (iconImage != null) iconImage.sprite = _upgradeData.icon;
        if (titleText != null) titleText.text = _upgradeData.title;
        if (descriptionText != null) descriptionText.text = _upgradeData.description;
        if (cardOutline != null) cardOutline.color = _upgradeData.cardOutlineColor;

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
}