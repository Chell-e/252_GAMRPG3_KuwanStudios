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
    public Button selectButton;

    private UpgradeDefinition definition;
    private Action<UpgradeDefinition> onSelected;

    public void Setup(UpgradeDefinition def, Action<UpgradeDefinition> onSelectedCallback)
    {
        definition = def;
        onSelected = onSelectedCallback;

        if (iconImage != null) iconImage.sprite = def.icon;
        if (titleText != null) titleText.text = def.title;
        if (descriptionText != null) descriptionText.text = def.description;

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelected?.Invoke(definition));
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