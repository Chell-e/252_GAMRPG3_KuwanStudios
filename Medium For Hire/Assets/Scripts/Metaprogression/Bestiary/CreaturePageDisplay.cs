using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CreaturePageDisplay : MonoBehaviour
{
    public PageEntry pageEntry;

    [Header("UI References")]
    public Image creatureSprite;
    public TextMeshProUGUI creatureProgress;
    public TextMeshProUGUI creatureName;
    public TextMeshProUGUI creatureLore;

    public void SetCreaturePage(PageEntry pageEntry)
    {
        bool isNameUnlocked = BestiaryManager.Instance.IsNameUnlocked(pageEntry);
        bool isImageUnlocked = BestiaryManager.Instance.IsImageUnlocked(pageEntry);
        bool isLoreUnlocked = BestiaryManager.Instance.IsLoreUnlocked(pageEntry);
        float progress = BestiaryManager.Instance.GetUnlockProgress(pageEntry);

        creatureName.text = isNameUnlocked ? pageEntry.name : "???";

        creatureLore.text = isLoreUnlocked ? pageEntry.entryLore : "???";

        creatureSprite.sprite = pageEntry.entrySprite;
        creatureSprite.color = isImageUnlocked ? Color.white : new Color(0f, 0f, 0f, 255f);

        creatureProgress.text = $"{progress:F0}%";
    }
}
