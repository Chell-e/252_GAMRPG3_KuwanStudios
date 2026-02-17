using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Pool & UI")]
    [SerializeField] public UpgradeDefinition[] upgradePool;
    public GameObject cardPrefab;
    public Transform cardContainer;

    [Header("Options")]
    [SerializeField] public int cardsToShow = 3;



    [Header("Debug")]
    private bool isOpen = false;
    private List<GameObject> spawnedCards = new List<GameObject>();


    private void Awake() 
    {
        // for making SINGLETON
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void ShowUpgradeOptions()
    {
        // avoid duplicating UI
        if (isOpen) return;

        // return if null
        if (upgradePool == null || upgradePool.Length == 0 || cardPrefab == null || cardContainer == null)
        {
            Debug.LogWarning("UpgradeManager: missing references or empty pool.");
            return;
        }


        // clamp the upgrades shown between 3 and remaining pool
        int shownCards = Mathf.Clamp(cardsToShow, 0 , upgradePool.Length);

        // pick unique random upgrades
        var available = upgradePool.ToList();
        Debug.Log(available);

        var chosen = new List<UpgradeDefinition>();
        for (int i = 0; i < shownCards; i++)
        {
            int idx = Random.Range(0, available.Count);
            chosen.Add(available[idx]);
            available.RemoveAt(idx);
        }

        // Pause game
        Time.timeScale = 0f;
        isOpen = true;

        // instantiate cards
        foreach (var def in chosen)
        {
            var go = Instantiate(cardPrefab, cardContainer);
            // ensure correct scale so layout groups size/position correctly
            go.transform.localScale = Vector3.one;
            var ui = go.GetComponent<UpgradeCardUI>();
            if (ui != null) ui.Setup(def, OnCardSelected);
            spawnedCards.Add(go);
        }

        // Force the layout system to update immediately (important when Time.timeScale == 0)
        var rt = cardContainer as RectTransform;
        if (rt != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }
    }

    private void OnCardSelected(UpgradeDefinition def)
    {
        ApplyUpgrade(def);
        CloseAndCleanup();
    }

    private void ApplyUpgrade(UpgradeDefinition def)
    {
        if (PlayerController.Instance == null || PlayerController.Instance.playerStats == null)
        {
            Debug.LogWarning("UpgradeManager: no player stats to apply upgrade to.");
            return;
        }

        var stats = PlayerController.Instance.playerStats;

        switch (def.upgradeType)
        {
            case UpgradeType.MaxHealthIncrease:
                stats.maxHealth += def.intValue;
                stats.currentHealth += def.intValue; // also heal by same amount
                UIManager.Instance.UpdateHpSlider();
                break;

            case UpgradeType.Heal:
                stats.currentHealth = Mathf.Min(stats.maxHealth, stats.currentHealth + def.intValue);
                UIManager.Instance.UpdateHpSlider();
                break;

            case UpgradeType.MoveSpeedIncrease:
                stats.movespeedPercent += def.intValue;
                break;

            case UpgradeType.DamageIncrease:
                stats.dmgPercent += def.intValue;
                break;

            case UpgradeType.ProjectileSpeedIncrease:
                stats.projectileSpeedPercent += def.intValue;
                break;

            default:
                Debug.Log($"UpgradeManager: unhandled upgrade type {def.upgradeType}");
                break;
        }

        // update any relevant UI
        UIManager.Instance.UpdateExpSlider();
        UIManager.Instance.UpdateHpSlider();

    }

    private void CloseAndCleanup()
    {
        // destroy spawned card GameObjects
        foreach (var go in spawnedCards) if (go != null) Destroy(go);
        spawnedCards.Clear();

        // unpause
        Time.timeScale = 1f;
        isOpen = false;
    }
}
